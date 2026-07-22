#!/usr/bin/env python3
"""
Extracts a machine-readable schema of the Splash wire protocol from the
decompiled Microsoft.Iris.Render.Protocols.Splash.* proxy classes, so the
native UIXrender.dll message structs and msgid dispatch tables can be
generated instead of hand-transcribed.

Source of truth: UIX.RenderApi/Microsoft/Iris/Render/Protocols/Splash/**/*.cs
(decompiled from the original UIX.RenderApi.dll; not hand-written, so its
Msg*_Name structs and _priv_msgid values are exactly what shipped).

See logs/UIXrender/Architecture.md for the reasoning behind this approach.
"""
import json
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
SPLASH_DIR = ROOT / "UIX.RenderApi/Microsoft/Iris/Render/Protocols/Splash"

RE_NAMESPACE = re.compile(r"^namespace\s+([\w.]+)", re.MULTILINE)
RE_CLASS = re.compile(
    r"^\s*(?:internal|public)\s+(?:sealed\s+)?class\s+(\w+)\s*(?::\s*([\w<>,.\s]+?))?\s*$",
    re.MULTILINE,
)
RE_MSG_STRUCT = re.compile(
    r"private struct (Msg(\d+)_(\w+))\s*\{(.*?)\}", re.DOTALL
)
RE_FIELD = re.compile(r"public\s+([\w.<>\[\]*]+)\s+(\w+);")
RE_INIT_REMOTE_CLASS = re.compile(r'InitRemoteClass\("([^"]+)"\)')
RE_BIND_ASSIGN = re.compile(
    r"_priv_remoteClass_(\w+)\s*=\s*port\.InitRemoteClass\(\"([^\"]+)\"\)"
)
RE_MSGID_ASSIGN = re.compile(r"->_priv_msgid\s*=\s*(\d+)U;")
RE_INSTANCE_SCOPED = re.compile(r"this\.m_renderHandle")

FIXED_HEADER_FIELDS = {"_priv_size", "_priv_msgid", "_priv_idObjectSubject"}


def parse_file(path: Path):
    text = path.read_text(encoding="utf-8-sig")
    ns_match = RE_NAMESPACE.search(text)
    namespace = ns_match.group(1) if ns_match else None

    classes = []
    for cls_match in RE_CLASS.finditer(text):
        cls_name = cls_match.group(1)
        base = cls_match.group(2)
        base = base.strip() if base else None

        # Slice out this class's body (up to the next top-level class or EOF)
        start = cls_match.end()
        next_match = RE_CLASS.search(text, start)
        body = text[start : next_match.start() if next_match else len(text)]

        messages = []
        for msg_match in RE_MSG_STRUCT.finditer(body):
            struct_name, msgid, msg_name, field_block = msg_match.groups()
            fields = [
                {"type": ftype, "name": fname}
                for ftype, fname in RE_FIELD.findall(field_block)
                if fname not in FIXED_HEADER_FIELDS
            ]
            # Find the Build* method for this message to see whether it's
            # instance-scoped (uses this.m_renderHandle as the subject) or
            # class/static-scoped (uses a resolved *_ClassHandle field).
            build_method_match = re.search(
                r"Build" + re.escape(msg_name) + r"\s*\((.*?)\)\s*\{(.*?)\n        \}",
                body,
                re.DOTALL,
            )
            instance_scoped = bool(
                build_method_match
                and RE_INSTANCE_SCOPED.search(build_method_match.group(2))
            )
            has_blob = any(f["type"] == "BLOBREF" for f in fields)
            messages.append(
                {
                    "msgid": int(msgid),
                    "name": msg_name,
                    "fields": fields,
                    "instanceScoped": instance_scoped,
                    "hasVariableLengthData": has_blob,
                }
            )
        messages.sort(key=lambda m: m["msgid"])

        # Class-name -> wire-name bindings only appear in the owning
        # ProtocolInstance's Init() (e.g. ProtocolSplashMessaging.Init()),
        # not in the Remote* file itself, so this stays empty here and is
        # cross-referenced in a second pass (see collect_wire_names).
        if messages or cls_name.startswith(("Remote", "Local")):
            classes.append(
                {
                    "namespace": namespace,
                    "class": cls_name,
                    "baseClass": base,
                    "sourceFile": str(path.relative_to(ROOT)),
                    "messages": messages,
                }
            )
    return classes


def collect_wire_names(all_text: str):
    """Map short binding name (e.g. 'Broker') -> wire class name string,
    from every '_priv_remoteClass_X = port.InitRemoteClass("Wire::Name")'
    assignment found across all ProtocolInstance-derived binder classes."""
    return {m.group(1): m.group(2) for m in RE_BIND_ASSIGN.finditer(all_text)}


def main():
    if not SPLASH_DIR.is_dir():
        print(f"error: {SPLASH_DIR} not found", file=sys.stderr)
        sys.exit(1)

    all_classes = []
    all_text_parts = []
    cs_files = sorted(SPLASH_DIR.rglob("*.cs"))
    for path in cs_files:
        text = path.read_text(encoding="utf-8-sig")
        all_text_parts.append(text)
        all_classes.extend(parse_file(path))

    wire_names = collect_wire_names("\n".join(all_text_parts))

    total_messages = sum(len(c["messages"]) for c in all_classes)
    classes_with_messages = [c for c in all_classes if c["messages"]]

    schema = {
        "generatedFrom": str(SPLASH_DIR.relative_to(ROOT)),
        "sourceFileCount": len(cs_files),
        "wireClassNameBindings": wire_names,
        "classes": classes_with_messages,
    }

    out_path = ROOT / "Tools" / "splash-schema.json"
    out_path.write_text(json.dumps(schema, indent=2), encoding="utf-8")

    print(f"Parsed {len(cs_files)} source files")
    print(f"Found {len(classes_with_messages)} classes with message structs")
    print(f"Found {total_messages} total messages")
    print(f"Resolved {len(wire_names)} wire class-name bindings")
    print(f"Wrote {out_path.relative_to(ROOT)}")


if __name__ == "__main__":
    main()
