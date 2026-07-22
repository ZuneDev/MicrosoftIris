using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.Iris.Render.Interop;

namespace Microsoft.Iris.Render.Subsystems.Schema;

// The managed model behind the ~50 Sp{Query,Get,Invoke}* "native reflection" exports in
// UIX/Microsoft/Iris/OS/NativeApi.cs.
//
// Design decision (deliberate substitution, not a stub -- see logs/UIXrender/FullSurface.md):
// the original exports projected a *C++* type system (the native gadget classes compiled
// into UIXrender.dll) so that markup could bind to them by name. Those C++ classes do not
// exist in this reimplementation and never will. What this layer does instead is project
// ordinary **CLR** types through the exact same export surface, using System.Reflection.
// Every accessor keeps its original signature and semantics ("give me the Nth property of
// this type schema, and its ID"), so the managed caller cannot tell the difference -- but
// the thing on the other side is a registered .NET type rather than a C++ one.
//
// IDs are stable per-schema indices, not pointers: the managed side round-trips them back
// to us (SpGetPropertyValue(typeSchema, obj, propertyID, ...)), so they only need to be
// unique and stable within one schema, which an array index is.

internal sealed class TypeSchema
{
    // The trimmer/AOT compiler can't see which members are needed, because the types come
    // from assemblies loaded at runtime via SpLoadDll -- that is the entire point of this
    // subsystem. Annotating the parameter keeps every member of anything that reaches
    // here rooted, instead of silently trimming the properties/methods markup binds to.
    public TypeSchema([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type, uint id)
    {
        Type = type;
        ID = id;

        // BindingFlags deliberately include static + instance public members only:
        // the original surface exposes IsStatic as a queryable trait of properties,
        // methods and events, which only makes sense if both kinds are enumerated.
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

        Properties = type.GetProperties(flags);
        Methods = type.GetMethods(flags).Where(m => !m.IsSpecialName).ToArray();
        Events = type.GetEvents(flags);
        Constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
    }

    public Type Type { get; }
    public uint ID { get; }
    public PropertyInfo[] Properties { get; }
    public MethodInfo[] Methods { get; }
    public EventInfo[] Events { get; }
    public ConstructorInfo[] Constructors { get; }

    public string Name => Type.Name;

    // "Runtime immutable" in the original means a value that can never change once
    // constructed -- the closest faithful CLR reading is a type with no writable
    // instance state at all.
    public bool IsRuntimeImmutable =>
        Type.IsPrimitive || Type == typeof(string) || Properties.All(p => !p.CanWrite);
}

internal sealed class EnumSchema
{
    public EnumSchema(Type type, uint id)
    {
        Type = type;
        ID = id;
        Names = Enum.GetNames(type);

        // GetValuesAsUnderlyingType rather than GetValues(Type): the latter is
        // RequiresDynamicCode (IL3050) because it has to construct a T[] of the enum type
        // at runtime, which can genuinely fail under NativeAOT -- and this project
        // publishes AOT. The underlying-type overload returns a boxed primitive array, no
        // dynamic array construction involved.
        Array underlying = Enum.GetValuesAsUnderlyingType(type);
        Values = new int[underlying.Length];
        for (int i = 0; i < underlying.Length; i++)
            Values[i] = Convert.ToInt32(underlying.GetValue(i));
    }

    public Type Type { get; }
    public uint ID { get; }
    public string[] Names { get; }
    public int[] Values { get; }

    public string Name => Type.Name;
    public bool IsFlags => Type.IsDefined(typeof(FlagsAttribute), false);
}

// One registered schema (the unit SpSetSchemaID/SpQueryTypeCount/SpGetTypeSchema operate
// on) -- a set of types and enums that were loaded together, e.g. from one markup-visible
// assembly loaded via SpLoadDll.
internal sealed class SchemaRegistration
{
    private readonly List<TypeSchema> _types = new();
    private readonly List<EnumSchema> _enums = new();

    public ushort ID { get; set; }

    public IReadOnlyList<TypeSchema> Types => _types;
    public IReadOnlyList<EnumSchema> Enums => _enums;

    public void Add([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type)
    {
        if (type.IsEnum)
            _enums.Add(new EnumSchema(type, (uint)_enums.Count));
        else
            _types.Add(new TypeSchema(type, (uint)_types.Count));
    }

    // Unavoidably trim-unsafe by design, and suppressed rather than left to warn so a real
    // future warning isn't lost in the noise: this projects types out of an assembly the
    // host chose at *runtime* (SpLoadDll), which the trimmer cannot see into by
    // definition. Anything markup binds to must therefore be kept alive by the host's own
    // trimming configuration (e.g. a TrimmerRootAssembly entry for each markup-visible
    // assembly), not by static analysis of UIXrender.
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Types come from assemblies loaded at runtime via SpLoadDll; the host must root them. See comment above.")]
    [UnconditionalSuppressMessage("Trimming", "IL2072", Justification = "Types come from assemblies loaded at runtime via SpLoadDll; the host must root them. See comment above.")]
    public static SchemaRegistration FromAssembly(Assembly assembly)
    {
        var registration = new SchemaRegistration();
        foreach (Type type in assembly.GetExportedTypes())
            registration.Add(type);
        return registration;
    }
}

// A live instance handed back through SpInvokeConstructor and passed into
// SpGetPropertyValue/SpInvokeMethod. Refcounted because the original surface exposes
// SpAddRefExternalObject/SpReleaseExternalObject as an explicit COM-style pair.
internal sealed class SchemaObject
{
    public SchemaObject(object instance, TypeSchema schema)
    {
        Instance = instance;
        Schema = schema;
        RefCount = 1;
    }

    public object Instance { get; }
    public TypeSchema Schema { get; }
    public int RefCount { get; set; }


    // Backs SpGetStateCache/SpSetStateCache -- an opaque 64-bit slot the caller uses to
    // memoise its own per-object state; UIXrender only stores and returns it.
    public ulong StateCache { get; set; }

    // Backs SpDataBaseObjectGet/SetInternalHandle -- the framework-side handle for this
    // object, likewise opaque to us.
    public ulong InternalHandle { get; set; }
}
