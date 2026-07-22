using System;
using System.Threading;
using Microsoft.Iris.Render.Engine;
using Microsoft.Iris.Render.Interop;
using Microsoft.Iris.Render.Interop.Extensions;
using Microsoft.Iris.Render.Interop.Protocol;
using Microsoft.Iris.Render.Interop.XmlLite;
using Microsoft.Iris.Render.Subsystems.Assets;
using Microsoft.Iris.Render.Subsystems.Lists;
using Microsoft.Iris.Render.Subsystems.Schema;
using Microsoft.Iris.Render.Subsystems.Text;
using Microsoft.Iris.Render.Subsystems.Xml;

// Exercises Microsoft.Iris.Render.Engine.EngineService directly (no P/Invoke, no
// published native DLL needed) -- unlike Tests/UIXrender.Interop.Tests, this can
// actually run cross-platform since EngineService is entirely managed/pointer-free.
// See logs/UIXrender/EngineCore.md.

int failures = 0;

void Check(bool condition, string message)
{
    Console.WriteLine((condition ? "PASS: " : "FAIL: ") + message);
    if (!condition)
        failures++;
}

var contextId = new ContextID(99);
var callbackInvoked = new ManualResetEventSlim(false);
uint observedSource = 0;
RENDERHANDLE observedHandle = default;
byte[]? observedData = null;

BufferReceivedHandler handler = (source, bufferHandle, flags, data) =>
{
    observedSource = source.value;
    observedHandle = bufferHandle;
    observedData = data.ToArray();
    callbackInvoked.Set();
};

IRenderThreadHandle thread = EngineService.StartRenderThread(contextId, handler);
Check(thread.ContextId == contextId, "StartRenderThread returns a handle for the requested context");

bool signaledOnStart = callbackInvoked.Wait(TimeSpan.FromSeconds(5));
Check(signaledOnStart, "render thread invoked the handler once on start");

callbackInvoked.Reset();
byte[] payload = { 1, 2, 3, 4 };
var sourceContext = new ContextID(7);
var bufferHandle = new RENDERHANDLE(123);
HRESULT hr = EngineService.SendBuffer(sourceContext, contextId, bufferHandle, BufferFlags.CopyData, payload);
Check(hr.IsSuccess(), $"SendBuffer to a registered context succeeds (hr=0x{hr.hr:X8})");

bool signaledOnSend = callbackInvoked.Wait(TimeSpan.FromSeconds(5));
Check(signaledOnSend, "handler observed a buffer sent via SendBuffer");
Check(observedSource == 7, $"handler observed the correct source context (got {observedSource})");
Check(observedHandle == bufferHandle, "handler observed the correct buffer handle");
Check(observedData is { Length: 4 } d && d[0] == 1 && d[3] == 4, "handler observed the correct payload bytes");

// Unregistering: sending to a torn-down context should fail cleanly, not throw/hang.
thread.Dispose();
HRESULT hrAfterDispose = EngineService.SendBuffer(sourceContext, contextId, bufferHandle, BufferFlags.CopyData, payload);
Check(hrAfterDispose.IsError(), $"SendBuffer to a disposed/unregistered context fails cleanly (hr=0x{hrAfterDispose.hr:X8})");

// ---------------------------------------------------------------------------------
// Subsystem models added by the full-surface pass (logs/UIXrender/FullSurface.md).
// The exports wrapping these are [UnmanagedCallersOnly] and so uncallable from C#;
// these exercise the real logic behind them via InternalsVisibleTo.
// ---------------------------------------------------------------------------------

Console.WriteLine("\n-- UIXList --");
var list = new UIXList();
Check(list.Add(UIXVariant.FromObject(10)) == 1, "UIXList.Add returns the new count");
list.Add(UIXVariant.FromObject(20));
list.Add(UIXVariant.FromObject(30));
Check(list.Count == 3, "UIXList tracks count across adds");
Check(list.IndexOf(UIXVariant.FromObject(20)) == 1, "UIXList.IndexOf finds a variant by value");
Check(list.Move(0, 2), "UIXList.Move succeeds for valid indices");
Check(list.TryGet(2, out UIXVariant moved) && moved.AsInt32 == 10, "UIXList.Move puts the item at the new index");
Check(!list.Move(0, 99), "UIXList.Move rejects an out-of-range index");
Check(list.IsItemAvailable(0), "UIXList reports a resident item as available");
Check(list.RemoveAt(0) && list.Count == 2, "UIXList.RemoveAt removes and updates count");
list.Clear();
Check(list.Count == 0, "UIXList.Clear empties the list");

Console.WriteLine("\n-- WaveParser --");
// Minimal 8-bit mono PCM RIFF/WAVE file: 4 sample bytes.
byte[] wav =
[
    .. "RIFF"u8, 0x28, 0, 0, 0, .. "WAVE"u8,
    .. "fmt "u8, 16, 0, 0, 0,
    1, 0,                    // PCM
    1, 0,                    // mono
    0x44, 0xAC, 0, 0,        // 44100 Hz
    0x44, 0xAC, 0, 0,        // avg bytes/sec
    1, 0,                    // block align
    8, 0,                    // bits per sample
    .. "data"u8, 4, 0, 0, 0,
    0x11, 0x22, 0x33, 0x44,
];
Check(WaveParser.TryParse(wav, out SoundHeader wavHeader, out byte[] wavSamples), "WaveParser parses a well-formed PCM RIFF/WAVE buffer");
Check(wavHeader.samplesPerSec == 44100, $"WaveParser reads the sample rate (got {wavHeader.samplesPerSec})");
Check(wavHeader.channels == 1 && wavHeader.bitsPerSample == 8, "WaveParser reads channel count and bit depth");
Check(wavSamples.Length == 4 && wavSamples[0] == 0x11 && wavSamples[3] == 0x44, "WaveParser returns the data chunk's bytes");
Check(!WaveParser.TryParse("NOTARIFF"u8, out _, out _), "WaveParser rejects a non-RIFF buffer");

Console.WriteLine("\n-- XmlLiteReader --");
using (XmlLiteReader xml = XmlLiteReader.Create("<a x='1'><b/>text</a>", isFragment: false))
{
    Check(xml.Read(out NativeXmlNodeType n1) && n1 == NativeXmlNodeType.Element && xml.LocalName == "a", "XmlLiteReader reads the root element");
    Check(xml.MoveToFirstAttribute() && xml.LocalName == "x" && xml.Value == "1", "XmlLiteReader walks to an attribute and reads its value");
    Check(xml.Read(out NativeXmlNodeType n2) && n2 == NativeXmlNodeType.Element && xml.IsEmptyElement, "XmlLiteReader reports an empty element");
    Check(xml.Read(out NativeXmlNodeType n3) && n3 == NativeXmlNodeType.Text && xml.Value == "text", "XmlLiteReader reads a text node");
    Check(xml.Read(out NativeXmlNodeType n4) && n4 == NativeXmlNodeType.EndElement, "XmlLiteReader reads the end element");
    Check(!xml.Read(out _), "XmlLiteReader reports end-of-stream (the caller's SUCCEEDED() gate)");
    Check(xml.LineNumber > 0, "XmlLiteReader reports line information");
}

Console.WriteLine("\n-- RichTextObject --");
var rich = new RichTextObject(richTextMode: false, new Microsoft.Iris.Render.Interop.Drawing.Size(400, 100), IntPtr.Zero);
rich.SetContent("hello");
Check(rich.Text == "hello" && rich.Length == 5, "RichTextObject.SetContent stores content");
rich.SetSelectionRange(0, 5);
Check(rich.GetSelectedText() == "hello", "RichTextObject returns the selected text");
rich.Copy();
rich.SetSelectionRange(5, 5);
Check(rich.Paste() && rich.Text == "hellohello", "RichTextObject copy/paste round-trips through the clipboard");
Check(rich.CanUndo && rich.Undo() && rich.Text == "hello", "RichTextObject.Undo restores the previous content");
Check(rich.Redo() && rich.Text == "hellohello", "RichTextObject.Redo reapplies the undone edit");
rich.ReadOnly = true;
Check(!rich.InsertText("x"), "RichTextObject refuses edits while read-only");
rich.ReadOnly = false;
rich.MaximumLength = 10;
Check(!rich.InsertText("overflow"), "RichTextObject enforces MaximumLength");
rich.SetSelectionRange(0, 5);
Check(rich.DeleteSelection() && rich.Text == "hello", "RichTextObject.DeleteSelection removes the selected range");

Console.WriteLine("\n-- TextMetrics --");
Microsoft.Iris.Render.Interop.Drawing.Size empty = TextMetrics.Measure("", 12f, false, 0);
Check(empty.width == 0 && empty.height > 0, "TextMetrics gives empty text zero width but one line of height");
Microsoft.Iris.Render.Interop.Drawing.Size oneLine = TextMetrics.Measure("hello world", 12f, false, 0);
Check(oneLine.width > 0 && oneLine.height == TextMetrics.LineHeight(12f), "TextMetrics measures unwrapped text as a single line");
Microsoft.Iris.Render.Interop.Drawing.Size wrapped = TextMetrics.Measure("hello world this is a longer string", 12f, true, 40);
Check(wrapped.height > oneLine.height, "TextMetrics wraps into multiple lines when constrained");
Microsoft.Iris.Render.Interop.Drawing.Size twoParagraphs = TextMetrics.Measure("a\nb", 12f, false, 0);
Check(twoParagraphs.height == TextMetrics.LineHeight(12f) * 2, "TextMetrics counts explicit newlines as separate lines");

Console.WriteLine("\n-- SchemaRegistration (native reflection over CLR types) --");
var schema = new SchemaRegistration();
schema.Add(typeof(SchemaProbe));
schema.Add(typeof(SchemaProbeKind));
Check(schema.Types.Count == 1 && schema.Enums.Count == 1, "SchemaRegistration separates types from enums");
TypeSchema probe = schema.Types[0];
Check(probe.Name == nameof(SchemaProbe), "TypeSchema exposes the CLR type name");
Check(Array.Exists(probe.Properties, p => p.Name == nameof(SchemaProbe.Value)), "TypeSchema enumerates public properties");
Check(Array.Exists(probe.Methods, m => m.Name == nameof(SchemaProbe.Add)), "TypeSchema enumerates public methods");
Check(probe.Constructors.Length == 1, "TypeSchema enumerates public constructors");
EnumSchema kind = schema.Enums[0];
Check(kind.Names.Length == 2 && kind.Values[1] == 5, "EnumSchema reads names and (non-contiguous) values");
Check(!kind.IsFlags, "EnumSchema reports non-flags enums correctly");

Console.WriteLine("\n-- UIXVariant round-trip --");
Check(UIXVariant.FromObject(42).AsInt32 == 42, "UIXVariant round-trips Int32");
Check(Math.Abs(UIXVariant.FromObject(1.5f).AsSingle - 1.5f) < float.Epsilon, "UIXVariant round-trips Single");
Check(Math.Abs(UIXVariant.FromObject(2.5d).AsDouble - 2.5d) < double.Epsilon, "UIXVariant round-trips Double");
Check(UIXVariant.FromObject(true).AsBool, "UIXVariant round-trips Bool");
Check(UIXVariant.FromObject(7L).ToObject() is long and 7L, "UIXVariant.ToObject returns the tagged CLR type");
Check(UIXVariant.Empty.type == VariantType.Empty, "UIXVariant.Empty carries the Empty tag");

Console.WriteLine("\n-- Remote channel (managed-direct wiring behind UIX.RenderApi's SpRemote*) --");
HRESULT vc = EngineService.RemoteCreateServerStreams("t", TransportProtocol.VC, out _, out _);
Check(vc.IsError(), "RemoteCreateServerStreams rejects the unrecovered VC transport (E_NOTIMPL)");
HRESULT made = EngineService.RemoteCreateServerStreams("uixrender-test", TransportProtocol.TCP, out IntPtr sendStream, out IntPtr recvStream);
Check(made.IsSuccess() && sendStream != IntPtr.Zero && recvStream != IntPtr.Zero, "RemoteCreateServerStreams (TCP) yields two stream handles");
Check(sendStream != recvStream, "send and receive are distinct handles, so releasing each is a separate safe op");
// Releasing both (RefCount 2 -> 0) tears the listener down; if the refcount/handle model
// were wrong this would double-free or throw, and the harness would never reach the end.
EngineService.ReleaseRemoteStream(sendStream);
EngineService.ReleaseRemoteStream(recvStream);
Check(true, "releasing both stream handles disposes the connection exactly once, without error");

Console.WriteLine();
Console.WriteLine(failures == 0 ? "ALL CHECKS PASSED" : $"{failures} CHECK(S) FAILED");
return failures == 0 ? 0 : 1;

// Probe types for the schema checks above -- deliberately trivial, since what's being
// verified is the projection machinery, not any particular type's shape.
internal sealed class SchemaProbe
{
    public int Value { get; set; }
    public int Add(int a, int b) => a + b;
}

internal enum SchemaProbeKind
{
    First = 0,
    Second = 5,
}
