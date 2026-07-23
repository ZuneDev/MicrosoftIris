# UIXrender.dll — architecture log

Append-only. Do not edit previous entries.

---

## 2026-07-21 — Schema extractor written and run: `tools/extract_splash_schema.py`

**Task:** implement the codegen decision from the entry directly below. Wrote a Python
script, `tools/extract_splash_schema.py`, that parses every `*.cs` file under
`UIX.RenderApi/Microsoft/Iris/Render/Protocols/Splash/` and produces
`tools/splash-schema.json`: for each `Remote*`/`Local*Callback` class, its namespace,
base class, source file, and every `Msg<N>_<Name>` struct's fields (name + C# type),
whether it carries a `BLOBREF` (variable-length payload), and whether the message is
instance-scoped (`this.m_renderHandle`) or class/static-scoped. Neither the script nor
its output is wired into a build yet — this is groundwork only, no native code
generated.

**Results:**
- 74 source files parsed, **56 classes** with message structs found, **283 total
  messages**.
- **Coverage check passed**: every `Remote*.cs`/`Local*.cs` file under that directory
  (56 of them) produced at least one parsed class — nothing silently dropped by the
  regex-based parser. (`I*.cs` interface files and the two `ProtocolSplash*.cs` binder
  files correctly produce no message structs, since they don't declare any.)
- **Resolves open question #1 from the entry below**: the wire class-name strings for
  all 44 remote classes (not just the 4 `Splash::Messaging::*` ones read by hand
  earlier) are now known, extracted from every `_priv_remoteClass_X =
  port.InitRemoteClass("Wire::Name")` assignment across the `ProtocolSplash*.cs` binder
  classes. Full list in `tools/splash-schema.json`'s `wireClassNameBindings`; confirms
  the naming scheme is `Splash::<Area>::<Class>` (`Splash::Messaging::*`,
  `Splash::Rendering::*`, `Splash::Desktop::*`) with a platform-backend segment inserted
  for Nt/Xenon-specific classes (e.g. `Splash::Rendering::Nt::GdiDevice`,
  `Splash::Rendering::Xenon::XeDevice`).
- **New finding, not previously noted**: message IDs are **not** guaranteed contiguous
  within a class — 8 of the 56 classes have gaps (e.g. `RemoteContext` is `[0, 2, 3,
  4]`, no 1; `RemoteAnimation` has gaps at 28, 30, 38, 40, 42). This is consistent with
  messages having been removed/deprecated across the engine's lifetime without
  renumbering the survivors, which makes sense given the same engine shipped across
  Zune and Xbox 360 over several versions. **Implication for codegen**: the native
  dispatch table generated per class must be a sparse `{msgid: handler}` map, not a
  dense array indexed by position — using array position instead of the literal
  `_priv_msgid` value would silently misroute messages for any of these 8 classes.
- Spot-checked extracted output against source by hand for `RemoteBroker`,
  `RemoteContext`, and `RemoteDx9Device` (all three read in full during the prior
  session) — exact field-for-field match, including the `_priv_objcbOwner`/
  `_priv_ctxcbOwner` callback-reference field pair on `RemoteDx9Device.Msg8_CreateVideoPool`
  and the base-class-continues-numbering behavior (`RemoteDx9Device : RemoteDevice`
  starts at msgid 8, meaning `RemoteDevice` itself owns 0–7 — confirmed separately by
  checking `RemoteDevice`'s own entry in the schema).

**Not yet done (deliberately stopped here to check in before going further):** the
schema doesn't yet capture each base class's *inherited* message range merged into its
subclasses (each class's `messages` list currently only holds what's declared in that
one file — a consumer needs to walk `baseClass` to get the full set for e.g.
`RemoteDx9Device`), and nothing consumes `splash-schema.json` yet to emit actual C++.
Those are the next two concrete steps once resumed.

---

## 2026-07-21 — Decision: generate the native message/dispatch layer instead of hand-transcribing it

**Context:** follow-up to the same-day architecture entry below. The user asked how to
implement the ~60-class `Protocols/Splash/**` message proxy layer on the native
(`UIXrender.dll`) side, and specifically whether an original IDL/schema exists that we
could reuse.

**Finding:** no. `Cn.CodeGenNameAttribute` (`UIX.RenderApi/Cn/CodeGenNameAttribute.cs`)
is `[Conditional("NEVER")]` — compiled out of the shipped assembly — which means we only
ever had its *output* (the decompiled `Remote*.cs`/`Local*.cs` proxy classes), never the
schema/IDL that presumably generated them at Microsoft. That source is not recoverable.

**Decision:** rather than inventing a new IDL syntax and hand-transcribing schema into
it, or hand-writing each native `Remote*` counterpart directly from its C# by eye, we
will mechanically extract a schema (JSON) from the already-decompiled, already-correct
`Remote*.cs`/`Local*.cs` files themselves (class wire-name, per-method `_priv_msgid`,
field names/types, which fields are `BLOBREF`), then use that schema to generate the
native-side message structs and msgid dispatch tables for the new `UIXrender.dll`
implementation, with stub bodies to fill in incrementally per the project's normal
stub-then-real-logic convention. The managed side (`UIX.RenderApi.dll`) is unchanged —
this only affects how the new native implementation's internals get built.

**Rationale:** the pattern across all ~60 classes is extremely uniform (confirmed in the
prior entry via `RemoteBroker.cs`), so hand-transcribing several hundred near-identical
message layouts is where a slipped field order or msgid would silently corrupt wire
format at runtime instead of failing to compile — mechanical extraction from source we
already know is correct removes that risk class entirely. Tradeoff accepted: this is
more upfront tooling investment than hand-writing the first few classes directly.

---

## 2026-07-21 — Reconnaissance: what UIXrender.dll is and how it talks to managed code

**Task:** starting reverse engineering of `UIXrender.dll` (the native backing library for
the `Microsoft.Iris` / Zune UI engine, "UIX"), following the same stage-1 procedure used
for `ZuneDBApi`. This entry is architecture reconnaissance only — no reimplementation
code written yet. Scope was explicitly limited this session to understanding the
transport, message format, and primitive concepts, per user direction.

**New project location (decided with the user, not yet created):** a new reimplementation
project will live inside this repo (`ZuneDev/MicrosoftIris`, checked out as the nested
submodule `libs/MicrosoftIris` under `ZuneDev/ZuneUIXTools`), alongside the existing
`UIX`, `UIX.RenderApi`, `UIXcontrols` projects — not in `ZuneShell.dll`/`ZuneDBApi`. This
log therefore lives at `logs/UIXrender/` at *this* repo's root, mirroring the
`logs/<Namespace>/<Topic>.md` convention already used in `ZuneShell.dll`.

### Method

`UIXrender.dll` (`UIXrender.dll` at the MicrosoftIris repo root, PE32+ x86-64, 1,752,288
bytes) is a **native, unmanaged DLL** — no managed metadata, so ILSpy can't see inside it
and this is genuinely stage-1 (Ghidra territory) for its actual implementation. However,
almost the entire *public surface and wire format* is recoverable without touching Ghidra
at all, because two managed assemblies in this same repo consume it and were themselves
decompiled straight from the original Microsoft assemblies (ILSpy-quality, per
CLAUDE.md's stage-1 rule that pure-managed decompilations are accurate enough to use
directly):

- `UIX/Microsoft/Iris/OS/NativeApi.cs` (from `UIX.dll`) — low-level OS-ish helpers
  (tracing, download, resource/DLL loading, memory, DPI, IME, registry-change
  notification), 119 `[DllImport("UIXRender.dll")]` entries.
- `UIX.RenderApi/Microsoft/Iris/Render/**` (from `UIX.RenderApi.dll`) — the actual
  render-engine client: `Protocol/EngineApi.cs` (init/transport primitives, 15 exports),
  `Extensions/ExtensionsApi.cs` (bitmap/sound asset loading, 7 exports),
  `Internal/FormApi.cs` (GDI+ init, 2 exports) live in the `Protocol`/`Internal`/
  `Extensions`/`Protocols/Splash` folders and account for most of the rest.
- `UIX.RenderApi/Microsoft/Iris/Render/Extensions/eDebugApi.cs` and
  `UIX/Microsoft/Iris/Debug/DebugHelpers.cs` P/Invoke a **second** native DLL,
  `UIXsup.dll` (5 exports: `DebugDisplayErrorStack`, `DebugSetTimedWriteLines`,
  `DebugSetWriteLinePrefix`, `DebugGetCategoryLevel`, `DebugSetCategoryLevel`,
  `DebugBreak`) — out of scope for `UIXrender.dll` itself but worth noting as a sibling
  native DLL this repo doesn't have a copy of yet (not present in the repo tree; only
  referenced by name).

Cross-checked every `Sp*`/`Debug*` P/Invoke name found across the whole repo (`grep -rh
DllImport --include=*.cs`, 199 unique names) against `UIXrender.dll`'s real export table
(`objdump -x`, 225 named exports incl. `DllMain`) — every referenced name is present in
the export table (sanity check passed: `comm -13` diff is empty), and roughly 26 exports
have no managed caller found yet in this repo tree (e.g. `SpRemoteClientInit`,
`SpRemoteCreateClientStreams`, `SpAnimationInputConsumerConnect`,
`SpDynamicSurfaceConnect`, `SpCreateObject`/`SpDestroyObject`/`SpFindClass`,
`SpAttachWndProc`/`SpDetachWndProc`, `SpGetMessageA`/`W`, `SpPeekMessageA`/`W`,
`SpDx9SoundDeviceCheckCaps`) — these are presumably called from C++ code inside
`UIXrender.dll` itself (internal use) or from a managed consumer not present in this
repo checkout; flagged as an open question, not guessed at further.

### Codename and platform scope

The namespace `Microsoft.Iris.Render.Protocols.Splash.*` and class names like
`ProtocolSplashMessaging`, `RemoteBroker` (bound to the wire class name
`"Splash::Messaging::Broker"`), and the `Sp` prefix on every native export strongly
indicate the internal codename for this engine is **"Splash"** — `Microsoft.Iris` is the
managed-facing brand name, "Splash" is what the wire protocol and (presumably) the C++
implementation call themselves internally. This is a direct reading of the code, not an
inference.

The protocol tree has two platform backends alongside the shared core:
`Protocols/Splash/Desktop/Nt/*` and `Protocols/Splash/Rendering/Nt/*` (Windows desktop —
"Nt" = Windows NT), and `Protocols/Splash/Desktop/Xenon/*` /
`Protocols/Splash/Rendering/Xenon/*` ("Xenon" is Microsoft's public codename for the
Xbox 360 CPU/platform). This confirms the Iris/Splash engine was shared between the Zune
desktop software and the Xbox 360 dashboard (Xbox 360's "New Xbox Experience" UI is
publicly known to have used this engine) — noted for context, not something we need to
implement (only the `Nt`/Windows desktop backend matters for reviving Zune).

### Core primitive types (all confirmed from `UIX.RenderApi/Microsoft/Iris/Render/Protocol/*.cs`)

- **`ContextID`** (`ContextID.cs`) — opaque `uint`, identifies one endpoint of a
  connection (`NULL`, `CURRENT` = `uint.MaxValue`, or an allocated id). Each side of a
  render-port connection (client vs. the Splash engine) has its own `ContextID`.
- **`RENDERHANDLE`** (`RENDERHANDLE.cs`) — opaque `uint` "pointer" to a remote object,
  valid only within one render port. Encoding (`HandleTable.cs`) is a packed bitfield
  given a `MessageCookieLayout`: `[unique/generation bits][group bits][object-index
  bits]`, low-to-high. **Default layout is 4 group bits + 20 object bits**
  (`MessageCookieLayout.Default`), leaving 8 bits for a generation/uniqueness counter
  that's incremented every time a handle slot is reused (use-after-free / stale-handle
  detection — a slot index can be reused but a stale handle referencing the old
  generation will be rejected).
- **`RENDERGROUP`** (`RENDERGROUP.cs`) — same opaque-`uint` shape as `RENDERHANDLE`;
  handles are allocated within a group (`HandleTable`'s `HandleGroup` bucket), and a
  group's handles can be destroyed as a batch (`RemoteContext.SendDestroyGroup`) — this
  is how e.g. an entire subtree of UI objects gets torn down in one message instead of
  one message per object.
- **`Message`** (`Message.cs`) — the base wire header for every request:
  `{ uint cbSize; uint nMsg; RENDERHANDLE idObjectSubject; }` (12 bytes). `nMsg` is a
  **per-class message index** (0, 1, 2, ... — see below), not a global message ID.
  `idObjectSubject` is the handle of the *class* or *instance* the message targets.
- **`CallbackMessage`** (`CallbackMessage.cs`) — the reverse-direction header (engine →
  client): `{ uint cbSize; uint nMsg; RENDERHANDLE idObjectSubject; RENDERHANDLE
  hTarget; }` (16 bytes) — same shape plus an explicit `hTarget` naming which client-side
  object/callback-handler the message is for.
- **`BLOBREF`** (`BLOBREF.cs`) — opaque `uint` offset/reference to a variable-length blob
  (string, byte buffer, or nested sub-message) appended after a fixed-size message
  struct. Used for anything of non-fixed size (see `RemoteBroker.Msg2_CreateClass`
  below).

### Message batching format

Individual messages are usually sent one at a time (`SendRemoteMessage` →
`SendMessageBuffer` → `SpBufferOpen`), but `RenderPort.BeginMessageBatch`/
`EndMessageBatch` switch to accumulating multiple messages into a **`MessageHeap`**
(`MessageHeap.cs`) — an arena allocator that carves fixed-size (default 8 OS pages) or
oversized "large" blocks out of `Marshal.AllocCoTaskMem`, each block prefixed by a
**`MessageBatchHeader`** (`{ RENDERHANDLE idPredicateBuffer; uint uOffsetFirstEntry; }`)
and containing a singly-linked list of **`MessageBatchEntry`**
(`{ uint uOffsetNextEntry; }`) records, each immediately followed by one `Message`-shaped
payload. `idPredicateBuffer` chains separate memory blocks together server-side when a
batch spans more than one block (each block after the first is sent as its own buffer,
with the previous block's `RENDERHANDLE` as its "predicate"). This is purely a
client-side batching/IPC-efficiency optimization — the underlying per-message wire
format inside a batch entry is identical to a standalone message.

### The one real send primitive: `SpBufferOpen`

Every outbound path — a single message, a batch, or a raw data buffer (e.g. image
pixels via `RenderPort.SendDataBuffer`) — ultimately funnels through
`EngineApi.SpBufferOpen(BufferInfo* phdrData, void* pvData)`:

```
struct BufferInfo {
    ContextID    idContextSrc;
    ContextID    idContextDest;
    RENDERHANDLE idBuffer;      // NULL unless this is one block of a multi-block batch
    BufferFlags  nFlags;        // IsBatch=1, CopyData=2
    uint         cbSizeBuffer;
}
```

`SpBufferOpen` is the single P/Invoke that crosses from managed into native code to
*deliver* something — there is no separate "receive" P/Invoke call; delivery is a
**callback**, registered once via `SpWrapBufferProc` (wraps a managed
`MessageBufferEventHandler(IntPtr pData, uint hContext, BufferInfo* pBufferInfo, void*
pvBufferData) : int` delegate into a raw native function pointer) and handed to the
engine at `SpInit`/`SpRenderThreadInit` time via `InitArgs.pfnProcessBuffer`. So the
actual flow is: native engine thread receives/produces a buffer → invokes the
managed callback synchronously on its own thread → managed side dispatches by
`BufferInfo.idContextSrc`/`idBuffer` back into `RenderPort.ProcessMessageBuffer` →
casts the buffer as a `CallbackMessage*` → looks up the registered `PortCallback`
handler by `idObjectSubject` (offset by 2 — indices 0/1 are reserved) and invokes it.
This is a thin, synchronous, non-blocking-IPC style API — **not** a classic
send/blocking-recv pump for the local case.

### Local vs. Remote channel (two ways `RenderPort` can be connected)

- **`LocalChannel`** (`LocalChannel.cs`) — same process. `Connect()` just calls
  `SpRenderThreadInit`, which spins the entire Splash engine up **on its own native
  thread inside the current process** (not a separate process) and returns an opaque
  thread handle; teardown is `SpRenderThreadUninit`. All communication after that is via
  the `SpBufferOpen`/callback mechanism above — cross-thread, not cross-process. This is
  what the normal Zune desktop UI uses.
- **`RemoteChannel`** (`RemoteChannel.cs`) — genuinely out-of-process (or out-of-machine).
  Uses a distinct set of `EngineApi` entry points: `SpRemoteCreateServerStreams(session,
  TransportProtocol, out send, out receive)` creates named send/receive stream handles,
  `SpRemoteWaitServerStreamsConnected` blocks until a client attaches, then
  `SpRemoteServerInit` wraps the connected streams into an opaque `pSession` used for
  actual traffic; `SpRemoteServerUninit` tears it down. `TransportProtocol.cs` enum:
  `VC` (1), `TCP` (2), `UDP` (3), `PIPE` (4) — `VC` is unidentified (possibly "virtual
  channel", an RDP-style transport; not confirmed, flagged as open question, not
  guessed further). The export table also has `SpRemoteClientInit`/
  `SpRemoteClientUninit`/`SpRemoteCreateClientStreams` (client-side counterparts) that no
  managed code in this repo calls yet — presumably used by a remote-debugging/mirroring
  tool, not the main Zune shell.

For reviving Zune, **only the `LocalChannel` path matters** — the remote/IPC path is
real functionality Zune shipped with (likely for a remote UI debugger/mirroring tool)
but is not required for a drop-in-replacement desktop client.

### RPC dispatch: how a "method call" becomes a message

`Protocols/Splash/Messaging/RemoteBroker.cs` is a fully-worked example (decompiled
intact, not reimplemented) of the pattern used by every `Remote*` class under
`Protocols/Splash/**` (~60 files: `RemoteContext`, `RemoteDx9Device`, `RemoteSprite`,
`RemoteAnimation`, `RemoteWindow`, etc.):

1. Each wire-visible native C++ class is given a **name string**
   (`"Splash::Messaging::Broker"`, `"Splash::Messaging::Context"`,
   `"Splash::Rendering::Dx9::Device"`-style, etc. — pattern inferred from
   `"Splash::Messaging::Broker"`, not all names confirmed yet) that gets resolved once,
   client-side, to a `RENDERHANDLE` via `RenderPort.InitRemoteClass` →
   `RemoteBroker.SendCreateClass` (message id 2 on the well-known root "Broker" object,
   itself always handle `_rootHandle`, allocated at `RenderPort` construction with no
   round-trip needed).
2. Each **method** on that remote class is a private nested struct laid out as
   `{ uint _priv_size; uint _priv_msgid; RENDERHANDLE _priv_idObjectSubject; <params...>
   }` — i.e. exactly the `Message` header followed by fixed-size parameters, with
   `_priv_msgid` a small per-class integer (0, 1, 2, ... — `RemoteBroker` has
   `DestroyObject=0`, `CreateObject=1`, `CreateClass=2`) and `_priv_idObjectSubject` set
   to either the resolved class handle (static/class-level messages, e.g.
   `CreateObject`/`CreateClass`/`DestroyObject` all target the Broker class handle) or a
   specific instance's `RENDERHANDLE` (instance methods).
3. Variable-length parameters (strings, nested message blobs) go through `BlobInfo`/
   `BLOBREF`: `BlobInfo` is constructed with the fixed struct size, `.Add(value)` appends
   the variable data after the struct and returns a `BLOBREF` (byte offset) to store in
   the fixed struct's field, `.AdjustedTotalSize` gives the real allocation size, and
   `.Attach(Message*)` finalizes it once the struct is filled in.
4. The filled struct is allocated from the port's current `MessageHeap`
   (`RenderPort.AllocMessageBuffer`), optionally byte-swapped in place for
   `ForeignByteOrder` connections (cross-endian remote scenarios — irrelevant for a pure
   x86/x64 revival), and handed to `SendRemoteMessage`.
5. Every C# proxy method has a matching **`Build*`** (constructs the message, doesn't
   send) and **`Send*`** (`Build*` + `SendRemoteMessage`) pair — `Build*` exists
   separately so a caller can batch several `Build*` calls under one
   `BeginMessageBatch`/`EndMessageBatch` without each one triggering its own send.

`Cn.CodeGenNameAttribute` (`UIX.RenderApi/Cn/CodeGenNameAttribute.cs`) exists but is
`[Conditional("NEVER")]` (i.e. compiled out of the shipped assembly, metadata-only) and
the extremely uniform `_priv_`-prefixed field/method naming across every `Remote*`/
`Local*` class in `Protocols/Splash/**` strongly suggests this entire proxy layer —
**and its native C++ counterpart inside `UIXrender.dll`** — was originally
machine-generated from some IDL-like class/message schema, not hand-written. This is an
inference from naming-convention evidence, not a directly-stated fact; flagged as such.
Practically: expect the ~60 `Remote*` classes under `Protocols/Splash/**` to be a
reliable, mechanical map of message IDs and payload struct layouts we can read straight
off the managed side for whichever native handler functions we end up decompiling in
Ghidra — we should not need to guess wire formats there.

### Native export catalog (grouped by subsystem; 199 names confirmed called from managed code in this repo)

Not yet decompiled — this is purely a naming/grouping pass from the P/Invoke call sites,
to plan decompilation order for follow-up sessions:

- **Tracing/logging**: `SpInitializeTracing`, `SpUninitializeTracing`,
  `SpUpdateTraceSettings`, `SpLogTrace`, `SpRegisterTraceCallback` (export-only).
- **Memory**: `SpMemAlloc`, `SpMemFree`, `SpFreeDib`.
- **Download / HTTP**: `SpFileDownload`, `SpDownloadGetBuffer`, `SpDownloadClose`,
  `SpHttpDownload`, `SpHttpStartup`, `SpHttpShutdown`, `SpHttpFlushProxyCache`.
- **Resource / DLL loading (markup assembly loading)**: `SpLoadDll`, `SpFreeDll`,
  `SpLoadBinaryResource`, `SpLoadFontResource`, `SpCreateDllLoadResultFactory`,
  `SpCreateDllLoadResult`, `SpSendDllSchemaUnloadNotification`.
- **Misc Win32/OS**: `SpGetDpi`, `SpExtractDroppedFileNames`, `SpGetMouseCursorInfo`,
  `SpRegNotifyChangeKey`, `SpRegRevokeNotifyChangeKey`, `SpPostDeferredImeMessage`,
  `SpRegisterImeCallbacks`, `SpUnregisterImeCallbacks`, `SpCreateNotifyWindow`,
  `SpDestroyNotifyWindow`, `SpAttachWndProc`/`SpDetachWndProc` (export-only),
  `SpGetMessageA`/`W`, `SpPeekMessageA`/`W` (export-only, distinct from the
  `InitArgs`-driven `EngineApi.SpPeekMessage`/`SpWaitMessage`).
- **Engine core / threading / transport** (`EngineApi.cs`, detailed above): `SpInit`,
  `SpUninit`, `SpBufferOpen`, `SpWrapBufferProc`, `SpPeekMessage`, `SpWaitMessage`,
  `SpInvoke`, `SpRenderThreadInit`, `SpRenderThreadUninit`,
  `SpRemoteCreateServerStreams`, `SpRemoteWaitServerStreamsConnected`,
  `SpRemoteServerInit`, `SpRemoteServerUninit`, `SpObjectRelease`,
  `SpCallDeferredInvokeProc`; export-only: `SpRemoteClientInit`,
  `SpRemoteClientUninit`, `SpRemoteCreateClientStreams`, `SpCreateObject`,
  `SpDestroyObject`, `SpFindClass` (native side of the class-name→handle resolution
  described above).
- **Direct3D 9 / graphics**: `SpDx9CompileEffect`, `SpGdiplusInit`, `SpGdiplusUninit`,
  `SpGetStateCache`, `SpSetStateCache`; export-only: `SpDx9SoundDeviceCheckCaps`,
  `SpAnimationInputConsumerConnect`, `SpAnimationInputPublisherConnect`,
  `SpDynamicSurfaceConnect` (video/live-surface hookup, e.g. `VideoElement`).
- **Native reflection / type-schema system** (large, self-contained subsystem — this is
  almost certainly the runtime binding layer that lets markup (`.uix`/`.uib`) reference
  native and managed gadget classes/properties/events by name): `SpGetTypeID`,
  `SpSetSchemaID`, `SpQueryTypeCount`, `SpQueryTypeName`, `SpGetTypeSchema`,
  `SpQueryBaseType`, `SpQueryPropertyCount`, `SpQueryPropertyName`,
  `SpQueryPropertyType`, `SpQueryPropertyCanRead`, `SpQueryPropertyCanWrite`,
  `SpQueryPropertyIsStatic`, `SpQueryPropertyNotifiesOnChange`, `SpGetPropertySchema`,
  `SpGetPropertyValue`, `SpSetPropertyValue`, `SpQueryMethodCount`,
  `SpQueryMethodName`, `SpQueryMethodIsStatic`, `SpQueryMethodParameterCount`,
  `SpQueryMethodReturnType`, `SpGetMethodParameterTypes`, `SpGetMethodSchema`,
  `SpInvokeMethod`, `SpQueryConstructorCount`, `SpQueryConstructorParameterCount`,
  `SpGetConstructorParameterTypes`, `SpGetConstructorSchema`, `SpInvokeConstructor`,
  `SpQueryEventCount`, `SpQueryEventName`, `SpQueryEventIsStatic`, `SpGetEventSchema`,
  `SpQueryEnumCount`, `SpQueryEnumName`, `SpQueryEnumIsFlags`, `SpQueryEnumValueCount`,
  `SpGetEnumNameValue`, `SpGetEnumSchema`, `SpInvokeEnumToString`, `SpInvokeToString`,
  `SpIsRuntimeImmutable`, `SpGetMarshalAs`, `SpQueryForMarshalAsInterface`,
  `SpCreateNativeString`, `SpCreateNativeImage`, `SpConvertStringToManaged`,
  `SpConvertImageToManaged`, `SpCopyString`, `SpGetStringHandle`, `SpGetImageHandle`.
- **Rich text editing** (`SpRichText*`, ~25 exports): full native text-box engine —
  `SpRichTextBuildObject`/`DestroyObject`, `Measure`, `Rasterize`, `SetContent`,
  `GetSimpleContent`/`Length`, `Copy`/`Cut`/`Paste`/`Delete`/`Undo`/`Redo`(`CanUndo`),
  `Scroll`/`ScrollToPosition`, `SetSelectionRange`, `SetFocus`, `SetReadOnly`,
  `SetWordWrap`, `SetScale`, `SetOversampleMode`, `SetMaximumLength`, `SetDetectUrls`,
  `SetScrollbars`, `OnTimerTick`, `ForwardKeyCharacter`/`ForwardKeyState`/
  `ForwardMouseInput`/`ForwardImeMessage`, `GetNaturalBounds`,
  `DestroyGlyphRunInfo`.
- **Simple text** (measurement/rendering only, no editing): `SpSimpleTextBuildObject`,
  `SpSimpleTextDestroyObject`, `SpSimpleTextMeasure`, `SpSimpleTextMeasurePossible`,
  `SpSimpleTextIsAvailable`.
- **Virtualized list backing store** (`SpUIXList*`, ~15 exports — native side of a
  data-bound/virtualized ListBox): `Add`, `Insert`, `Remove`, `RemoveAt`, `Move`,
  `Clear`, `GetItem`, `SetItem`, `GetCount`, `IndexOf`, `IsItemAvailable`,
  `FetchSlowData`, `WantSlowDataRequests`, `RegisterCallbacks`,
  `UnregisterCallbacks`, `NotifyVisualsCreated`, `NotifyVisualsReleased`.
- **Data binding / query system** (`SpData*` — likely how markup-declared bindings reach
  into a query result, plausibly the eventual bridge to `ZuneDBApi`'s query/property-bag
  system reimplemented at a different layer of this same revival project; not confirmed,
  flagged as a hypothesis to revisit): `SpDataProviderConstructQuery`,
  `SpDataProviderReportDataMapping`, `SpDataQueryRefresh`,
  `SpDataQueryGetEnabledProperty`/`SetEnabledProperty`,
  `SpDataQueryGetResultProperty`/`SetResultProperty`,
  `SpDataQueryGetStatusProperty`/`SetStatusProperty`, `SpDataQueryNotifyInitialized`,
  `SpDataBaseObjectGetProperty`/`SetProperty`,
  `SpDataBaseObjectGetInternalHandle`/`SetInternalHandle`,
  `SpDataBaseObjectGetTypeHandle`.
- **XML parsing** (`SpXmlLite*` — thin wrapper around Windows' XmlLite COM API; almost
  certainly how `.uix` markup files get parsed): `SpXmlLiteCreateXmlReader`,
  `SpXmlLiteDeleteXmlReader`, `SpXmlLiteRead`, `SpXmlLiteGetLocalName`,
  `SpXmlLiteGetPrefix`, `SpXmlLiteGetQualifiedName`, `SpXmlLiteGetValue`,
  `SpXmlLiteIsEmptyElement`, `SpXmlLiteMoveToFirstAttribute`/`MoveToNextAttribute`,
  `SpXmlLiteGetLineNumber`/`GetLinePosition`.
- **Asset loading** (`Extensions/ExtensionsApi.cs`): `SpBitmapLoadFile`,
  `SpBitmapLoadRaw`, `SpBitmapLoadResource`, `SpBitmapLoadBuffer`, `SpBitmapDelete`,
  `SpSoundLoadBuffer`, `SpSoundDispose`.
- **Debug helpers, separate DLL `UIXsup.dll`** (not `UIXrender.dll` — out of scope here,
  noted for later): `DebugDisplayErrorStack`, `DebugSetTimedWriteLines`,
  `DebugSetWriteLinePrefix`, `DebugGetCategoryLevel`, `DebugSetCategoryLevel`,
  `DebugBreak`.

### Error space

`Protocol/EngineApi.cs`'s `IFC` (If-Failed-Cleanup — same convention name used
elsewhere in this project already) helper hardcodes ~40 specific HRESULT values in the
`-2147221xxx` range with human-readable messages (e.g. `-2147221493` = "object is busy",
`-2147221383` = "message not found during class registration", `-2147221354` = "unable
to connect to remote renderer") — this is effectively a free, already-decompiled HRESULT
error table for this custom facility; no need to reverse it from the native binary.

### Open questions (logged per CLAUDE.md's uncertainty procedure — not guessed at)

1. What native class-name strings exist beyond `"Splash::Messaging::Broker"` /
   `"Splash::Messaging::Context"` / `"Splash::Messaging::DataBuffer"` /
   `"Splash::Messaging::ContextRelay"` (seen in `ProtocolSplashMessaging.Init`) — the
   `Protocols/Splash/Rendering/**` classes almost certainly have their own
   `"Splash::Rendering::..."`-style names but none were read this session. Needed before
   `SpFindClass`/`SpCreateObject` can be meaningfully decompiled.
2. What `TransportProtocol.VC` (value 1) stands for — not stated anywhere in the
   decompiled managed code.
3. Which ~26 exports with no managed caller in this repo are called from (native-internal
   use inside `UIXrender.dll`, or a managed consumer not checked into this repo).
4. Full native struct layout / calling convention details (this DLL is native x64, so
   this is genuinely Ghidra work for a future session) — everything above is the
   managed-visible contract, not the C++ implementation.

### Next steps (not started)

Decompilation via Ghidra of `UIXrender.dll`'s exports, starting with the smallest/most
self-contained subsystems (tracing, memory, DPI — mirroring the "start at the top, work
down" plan discussed with the user) once this session's architecture pass is reviewed.
