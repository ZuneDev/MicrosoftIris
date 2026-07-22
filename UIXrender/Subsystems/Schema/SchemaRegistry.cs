using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Iris.Render.Interop;

namespace Microsoft.Iris.Render.Subsystems.Schema;

// Process-wide type-ID assignment for the schema subsystem. Several exports return a
// *type ID* rather than a schema pointer (SpQueryBaseType, SpQueryPropertyType,
// SpQueryMethodReturnType, SpGetMethodParameterTypes, SpGetTypeID), and the managed side
// round-trips those IDs across unrelated calls -- so they have to be stable and unique
// process-wide, not per-schema. An incrementing counter keyed by CLR Type gives exactly
// that.
//
// IDs 0-15 are reserved for the primitives that UIXVariant can carry directly, so a
// caller can recognise "this property is an int" without a schema lookup. That mirrors
// what the original must have done to make UIXVariant's VariantType tag meaningful, and
// is the only part of this mapping that isn't an arbitrary counter.
internal static class SchemaRegistry
{
    public const uint TypeIdNone = 0;

    private static readonly Dictionary<Type, uint> s_wellKnown = new()
    {
        [typeof(void)] = 0,
        [typeof(bool)] = (uint)VariantType.Bool,
        [typeof(byte)] = (uint)VariantType.Byte,
        [typeof(int)] = (uint)VariantType.Int32,
        [typeof(long)] = (uint)VariantType.Int64,
        [typeof(float)] = (uint)VariantType.Single,
        [typeof(double)] = (uint)VariantType.Double,
    };

    private const uint FirstDynamicTypeId = 16;

    private static readonly ConcurrentDictionary<Type, uint> s_typeIds = new();
    private static readonly ConcurrentDictionary<uint, Type> s_typesById = new();
    private static uint s_nextTypeId = FirstDynamicTypeId;

    public static uint GetTypeId(Type type)
    {
        if (type == null)
            return TypeIdNone;

        if (s_wellKnown.TryGetValue(type, out uint wellKnown))
            return wellKnown;

        return s_typeIds.GetOrAdd(type, static t =>
        {
            uint id = System.Threading.Interlocked.Increment(ref s_nextTypeId);
            s_typesById[id] = t;
            return id;
        });
    }

    public static Type GetType(uint typeId) => s_typesById.TryGetValue(typeId, out Type type) ? type : null;

    // Backs SpGetMarshalAs: "what does this type look like to the interop layer". For
    // anything UIXVariant can carry natively that's the VariantType tag; for everything
    // else it's UIXObject, i.e. "marshal it as an opaque object handle".
    public static uint GetMarshalAs(Type type)
    {
        if (type == null)
            return (uint)VariantType.Empty;
        if (type == typeof(bool)) return (uint)VariantType.Bool;
        if (type == typeof(byte)) return (uint)VariantType.Byte;
        if (type == typeof(int)) return (uint)VariantType.Int32;
        if (type == typeof(long)) return (uint)VariantType.Int64;
        if (type == typeof(float)) return (uint)VariantType.Single;
        if (type == typeof(double)) return (uint)VariantType.Double;
        if (type == typeof(string)) return (uint)VariantType.UIXString;
        if (type.IsEnum) return (uint)VariantType.Enum;
        return (uint)VariantType.UIXObject;
    }
}
