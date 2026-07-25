#if NETCOREAPP

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Iris.Interop;
using Microsoft.Iris.Render.Engine;
using Microsoft.Iris.Render.Interop;

namespace Microsoft.Iris.Render.Subsystems.Schema;

// [UnmanagedCallersOnly] exports for the "native reflection / type-schema" family in
// UIX/Microsoft/Iris/OS/NativeApi.cs (~48 entry points). See SchemaModel.cs for why these
// project CLR types instead of the original's C++ ones.
//
// Two marshaling conventions to keep straight, both read off the original DllImport
// declarations rather than assumed:
//  * every `out bool` is a 4-byte Win32 BOOL (no [MarshalAs] override on the managed
//    side), so it's `int*` here;
//  * every `uint[] IDs` is a caller-allocated array, so it's `uint*` + count here.
public static unsafe class SchemaApi
{
    private static uint OK => (uint)HRESULT.S_OK.hr;
    private static uint Fail => unchecked((uint)HRESULT.E_FAIL.hr);
    private static uint InvalidArg => unchecked((uint)HRESULT.E_INVALIDARG.hr);

    // ---- schema-level ----------------------------------------------------------------

    [UnmanagedCallersOnly(EntryPoint = "SpSetSchemaID")]
    public static uint SpSetSchemaID(IntPtr schema, ushort id)
    {
        if (!HandleTable.TryGet(schema, out SchemaRegistration registration))
            return InvalidArg;
        registration.ID = id;
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpQueryTypeCount")]
    public static uint SpQueryTypeCount(IntPtr schema, uint* typeCount)
    {
        if (typeCount == null || !HandleTable.TryGet(schema, out SchemaRegistration registration))
            return InvalidArg;
        *typeCount = (uint)registration.Types.Count;
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpQueryEnumCount")]
    public static uint SpQueryEnumCount(IntPtr schema, uint* enumCount)
    {
        if (enumCount == null || !HandleTable.TryGet(schema, out SchemaRegistration registration))
            return InvalidArg;
        *enumCount = (uint)registration.Enums.Count;
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpGetTypeSchema")]
    public static uint SpGetTypeSchema(IntPtr schema, uint index, IntPtr* type, uint* id)
    {
        if (type == null || id == null || !HandleTable.TryGet(schema, out SchemaRegistration registration))
            return InvalidArg;
        if (index >= registration.Types.Count)
            return InvalidArg;

        TypeSchema typeSchema = registration.Types[(int)index];
        *type = HandleTable.Alloc(typeSchema);
        *id = typeSchema.ID;
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpGetEnumSchema")]
    public static uint SpGetEnumSchema(IntPtr schema, uint index, IntPtr* enumType, uint* id)
    {
        if (enumType == null || id == null || !HandleTable.TryGet(schema, out SchemaRegistration registration))
            return InvalidArg;
        if (index >= registration.Enums.Count)
            return InvalidArg;

        EnumSchema enumSchema = registration.Enums[(int)index];
        *enumType = HandleTable.Alloc(enumSchema);
        *id = enumSchema.ID;
        return OK;
    }

    // ---- type-level ------------------------------------------------------------------

    [UnmanagedCallersOnly(EntryPoint = "SpQueryTypeName")]
    public static uint SpQueryTypeName(IntPtr typeSchema, char** name)
    {
        if (name == null || !HandleTable.TryGet(typeSchema, out TypeSchema schema))
            return InvalidArg;
        *name = NativeString.InternUni(schema.Name);
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpIsRuntimeImmutable")]
    public static uint SpIsRuntimeImmutable(IntPtr typeSchema, int* isRuntimeImmutable)
    {
        if (isRuntimeImmutable == null || !HandleTable.TryGet(typeSchema, out TypeSchema schema))
            return InvalidArg;
        *isRuntimeImmutable = schema.IsRuntimeImmutable ? 1 : 0;
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpQueryBaseType")]
    public static uint SpQueryBaseType(IntPtr typeSchema, uint* baseTypeID)
    {
        if (baseTypeID == null || !HandleTable.TryGet(typeSchema, out TypeSchema schema))
            return InvalidArg;
        *baseTypeID = SchemaRegistry.GetTypeId(schema.Type.BaseType);
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpGetMarshalAs")]
    public static uint SpGetMarshalAs(IntPtr typeSchema, uint* interopEquivalent)
    {
        if (interopEquivalent == null || !HandleTable.TryGet(typeSchema, out TypeSchema schema))
            return InvalidArg;
        *interopEquivalent = SchemaRegistry.GetMarshalAs(schema.Type);
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpQueryConstructorCount")]
    public static uint SpQueryConstructorCount(IntPtr typeSchema, uint* count)
    {
        if (count == null || !HandleTable.TryGet(typeSchema, out TypeSchema schema))
            return InvalidArg;
        *count = (uint)schema.Constructors.Length;
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpGetConstructorSchema")]
    public static uint SpGetConstructorSchema(IntPtr typeSchema, uint index, IntPtr* constructor, uint* id)
    {
        if (constructor == null || id == null || !HandleTable.TryGet(typeSchema, out TypeSchema schema))
            return InvalidArg;
        if (index >= schema.Constructors.Length)
            return InvalidArg;

        *constructor = HandleTable.Alloc(schema.Constructors[index]);
        *id = index;
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpQueryPropertyCount")]
    public static uint SpQueryPropertyCount(IntPtr typeSchema, uint* count)
    {
        if (count == null || !HandleTable.TryGet(typeSchema, out TypeSchema schema))
            return InvalidArg;
        *count = (uint)schema.Properties.Length;
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpGetPropertySchema")]
    public static uint SpGetPropertySchema(IntPtr typeSchema, uint index, IntPtr* property, uint* id)
    {
        if (property == null || id == null || !HandleTable.TryGet(typeSchema, out TypeSchema schema))
            return InvalidArg;
        if (index >= schema.Properties.Length)
            return InvalidArg;

        *property = HandleTable.Alloc(schema.Properties[index]);
        *id = index;
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpQueryMethodCount")]
    public static uint SpQueryMethodCount(IntPtr typeSchema, uint* count)
    {
        if (count == null || !HandleTable.TryGet(typeSchema, out TypeSchema schema))
            return InvalidArg;
        *count = (uint)schema.Methods.Length;
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpGetMethodSchema")]
    public static uint SpGetMethodSchema(IntPtr typeSchema, uint index, IntPtr* method, uint* id)
    {
        if (method == null || id == null || !HandleTable.TryGet(typeSchema, out TypeSchema schema))
            return InvalidArg;
        if (index >= schema.Methods.Length)
            return InvalidArg;

        *method = HandleTable.Alloc(schema.Methods[index]);
        *id = index;
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpQueryEventCount")]
    public static uint SpQueryEventCount(IntPtr typeSchema, uint* count)
    {
        if (count == null || !HandleTable.TryGet(typeSchema, out TypeSchema schema))
            return InvalidArg;
        *count = (uint)schema.Events.Length;
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpGetEventSchema")]
    public static uint SpGetEventSchema(IntPtr typeSchema, uint index, IntPtr* eventObj, uint* id)
    {
        if (eventObj == null || id == null || !HandleTable.TryGet(typeSchema, out TypeSchema schema))
            return InvalidArg;
        if (index >= schema.Events.Length)
            return InvalidArg;

        *eventObj = HandleTable.Alloc(schema.Events[index]);
        *id = index;
        return OK;
    }

    // ---- construction / invocation ---------------------------------------------------

    [UnmanagedCallersOnly(EntryPoint = "SpInvokeConstructor")]
    public static uint SpInvokeConstructor(IntPtr typeSchema, uint constructorID, UIXVariant* parameters, uint parameterCount, IntPtr* nativeObject)
    {
        if (nativeObject == null || !HandleTable.TryGet(typeSchema, out TypeSchema schema))
            return InvalidArg;
        if (constructorID >= schema.Constructors.Length)
            return InvalidArg;

        try
        {
            object instance = schema.Constructors[constructorID].Invoke(ToObjects(parameters, parameterCount));
            *nativeObject = HandleTable.Alloc(new SchemaObject(instance, schema));
            return OK;
        }
        catch (TargetInvocationException)
        {
            return Fail;
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "SpGetPropertyValue")]
    public static uint SpGetPropertyValue(IntPtr typeSchema, IntPtr nativeObject, uint propertyID, UIXVariant* propertyValue)
    {
        if (propertyValue == null || !HandleTable.TryGet(typeSchema, out TypeSchema schema))
            return InvalidArg;
        if (propertyID >= schema.Properties.Length)
            return InvalidArg;

        object instance = HandleTable.Get<SchemaObject>(nativeObject)?.Instance;
        try
        {
            *propertyValue = UIXVariant.FromObject(schema.Properties[propertyID].GetValue(instance));
            return OK;
        }
        catch (Exception e) when (e is TargetInvocationException or NotSupportedException)
        {
            return Fail;
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "SpSetPropertyValue")]
    public static uint SpSetPropertyValue(IntPtr typeSchema, IntPtr nativeObject, uint propertyID, UIXVariant* propertyValue)
    {
        if (propertyValue == null || !HandleTable.TryGet(typeSchema, out TypeSchema schema))
            return InvalidArg;
        if (propertyID >= schema.Properties.Length)
            return InvalidArg;

        object instance = HandleTable.Get<SchemaObject>(nativeObject)?.Instance;
        PropertyInfo property = schema.Properties[propertyID];
        try
        {
            property.SetValue(instance, Coerce(propertyValue->ToObject(), property.PropertyType));
            return OK;
        }
        catch (Exception e) when (e is TargetInvocationException or NotSupportedException or ArgumentException)
        {
            return Fail;
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "SpInvokeMethod")]
    public static uint SpInvokeMethod(IntPtr typeSchema, IntPtr nativeObject, uint methodID, UIXVariant* parameters, uint parameterCount, UIXVariant* returnValue)
    {
        if (returnValue == null || !HandleTable.TryGet(typeSchema, out TypeSchema schema))
            return InvalidArg;
        if (methodID >= schema.Methods.Length)
            return InvalidArg;

        object instance = HandleTable.Get<SchemaObject>(nativeObject)?.Instance;
        MethodInfo method = schema.Methods[methodID];
        try
        {
            object[] args = ToObjects(parameters, parameterCount);
            ParameterInfo[] expected = method.GetParameters();
            for (int i = 0; i < args.Length && i < expected.Length; i++)
                args[i] = Coerce(args[i], expected[i].ParameterType);

            object result = method.Invoke(instance, args);
            *returnValue = method.ReturnType == typeof(void) ? UIXVariant.Empty : UIXVariant.FromObject(result);
            return OK;
        }
        catch (Exception e) when (e is TargetInvocationException or NotSupportedException or ArgumentException)
        {
            return Fail;
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "SpInvokeToString")]
    public static uint SpInvokeToString(IntPtr typeSchema, IntPtr nativeObject, IntPtr* value)
    {
        if (value == null)
            return InvalidArg;

        object instance = HandleTable.Get<SchemaObject>(nativeObject)?.Instance;
        *value = (IntPtr)NativeString.AllocUni(instance?.ToString() ?? string.Empty);
        return OK;
    }

    // ---- constructor-schema-level ----------------------------------------------------

    [UnmanagedCallersOnly(EntryPoint = "SpQueryConstructorParameterCount")]
    public static uint SpQueryConstructorParameterCount(IntPtr constructorSchema, uint* count)
    {
        if (count == null || !HandleTable.TryGet(constructorSchema, out ConstructorInfo constructor))
            return InvalidArg;
        *count = (uint)constructor.GetParameters().Length;
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpGetConstructorParameterTypes")]
    public static uint SpGetConstructorParameterTypes(IntPtr constructorSchema, uint* ids, uint count)
    {
        if (ids == null || !HandleTable.TryGet(constructorSchema, out ConstructorInfo constructor))
            return InvalidArg;
        return WriteParameterTypes(constructor.GetParameters(), ids, count);
    }

    // ---- property-schema-level -------------------------------------------------------

    [UnmanagedCallersOnly(EntryPoint = "SpQueryPropertyName")]
    public static uint SpQueryPropertyName(IntPtr propertySchema, char** name)
    {
        if (name == null || !HandleTable.TryGet(propertySchema, out PropertyInfo property))
            return InvalidArg;
        *name = NativeString.InternUni(property.Name);
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpQueryPropertyType")]
    public static uint SpQueryPropertyType(IntPtr propertySchema, uint* type)
    {
        if (type == null || !HandleTable.TryGet(propertySchema, out PropertyInfo property))
            return InvalidArg;
        *type = SchemaRegistry.GetTypeId(property.PropertyType);
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpQueryPropertyCanRead")]
    public static uint SpQueryPropertyCanRead(IntPtr propertySchema, int* canRead)
    {
        if (canRead == null || !HandleTable.TryGet(propertySchema, out PropertyInfo property))
            return InvalidArg;
        *canRead = property.CanRead ? 1 : 0;
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpQueryPropertyCanWrite")]
    public static uint SpQueryPropertyCanWrite(IntPtr propertySchema, int* canWrite)
    {
        if (canWrite == null || !HandleTable.TryGet(propertySchema, out PropertyInfo property))
            return InvalidArg;
        *canWrite = property.CanWrite ? 1 : 0;
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpQueryPropertyIsStatic")]
    public static uint SpQueryPropertyIsStatic(IntPtr propertySchema, int* isStatic)
    {
        if (isStatic == null || !HandleTable.TryGet(propertySchema, out PropertyInfo property))
            return InvalidArg;
        *isStatic = (property.GetMethod ?? property.SetMethod)?.IsStatic == true ? 1 : 0;
        return OK;
    }

    // A property "notifies on change" if its declaring type raises a change event for it.
    // The CLR convention for exactly that is INotifyPropertyChanged, or a per-property
    // "<Name>Changed" event -- both checked here rather than assuming one or the other.
    [UnmanagedCallersOnly(EntryPoint = "SpQueryPropertyNotifiesOnChange")]
    [UnconditionalSuppressMessage("Trimming", "IL2075", Justification = "Declaring types originate from runtime-loaded assemblies (SpLoadDll); see SchemaRegistration.FromAssembly.")]
    public static uint SpQueryPropertyNotifiesOnChange(IntPtr propertySchema, int* notifiesOnChange)
    {
        if (notifiesOnChange == null || !HandleTable.TryGet(propertySchema, out PropertyInfo property))
            return InvalidArg;

        Type declaring = property.DeclaringType;
        bool notifies =
            typeof(System.ComponentModel.INotifyPropertyChanged).IsAssignableFrom(declaring) ||
            declaring?.GetEvent(property.Name + "Changed") != null;

        *notifiesOnChange = notifies ? 1 : 0;
        return OK;
    }

    // ---- method-schema-level ---------------------------------------------------------

    [UnmanagedCallersOnly(EntryPoint = "SpQueryMethodName")]
    public static uint SpQueryMethodName(IntPtr methodSchema, char** name)
    {
        if (name == null || !HandleTable.TryGet(methodSchema, out MethodInfo method))
            return InvalidArg;
        *name = NativeString.InternUni(method.Name);
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpQueryMethodReturnType")]
    public static uint SpQueryMethodReturnType(IntPtr methodSchema, uint* type)
    {
        if (type == null || !HandleTable.TryGet(methodSchema, out MethodInfo method))
            return InvalidArg;
        *type = SchemaRegistry.GetTypeId(method.ReturnType);
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpQueryMethodParameterCount")]
    public static uint SpQueryMethodParameterCount(IntPtr methodSchema, uint* count)
    {
        if (count == null || !HandleTable.TryGet(methodSchema, out MethodInfo method))
            return InvalidArg;
        *count = (uint)method.GetParameters().Length;
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpGetMethodParameterTypes")]
    public static uint SpGetMethodParameterTypes(IntPtr methodSchema, uint* ids, uint count)
    {
        if (ids == null || !HandleTable.TryGet(methodSchema, out MethodInfo method))
            return InvalidArg;
        return WriteParameterTypes(method.GetParameters(), ids, count);
    }

    [UnmanagedCallersOnly(EntryPoint = "SpQueryMethodIsStatic")]
    public static uint SpQueryMethodIsStatic(IntPtr methodSchema, int* isStatic)
    {
        if (isStatic == null || !HandleTable.TryGet(methodSchema, out MethodInfo method))
            return InvalidArg;
        *isStatic = method.IsStatic ? 1 : 0;
        return OK;
    }

    // ---- event-schema-level ----------------------------------------------------------

    [UnmanagedCallersOnly(EntryPoint = "SpQueryEventName")]
    public static uint SpQueryEventName(IntPtr eventSchema, char** name)
    {
        if (name == null || !HandleTable.TryGet(eventSchema, out EventInfo eventInfo))
            return InvalidArg;
        *name = NativeString.InternUni(eventInfo.Name);
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpQueryEventIsStatic")]
    public static uint SpQueryEventIsStatic(IntPtr eventSchema, int* isStatic)
    {
        if (isStatic == null || !HandleTable.TryGet(eventSchema, out EventInfo eventInfo))
            return InvalidArg;
        *isStatic = (eventInfo.AddMethod ?? eventInfo.RemoveMethod)?.IsStatic == true ? 1 : 0;
        return OK;
    }

    // ---- object lifetime / identity --------------------------------------------------

    [UnmanagedCallersOnly(EntryPoint = "SpAddRefExternalObject")]
    public static void SpAddRefExternalObject(IntPtr nativeObject)
    {
        if (HandleTable.TryGet(nativeObject, out SchemaObject obj))
            obj.RefCount++;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpReleaseExternalObject")]
    public static void SpReleaseExternalObject(IntPtr nativeObject)
    {
        if (!HandleTable.TryGet(nativeObject, out SchemaObject obj))
            return;

        if (--obj.RefCount <= 0)
            HandleTable.Free(nativeObject);
    }

    [UnmanagedCallersOnly(EntryPoint = "SpGetTypeID")]
    public static uint SpGetTypeID(IntPtr nativeObject, uint* typeID)
    {
        if (typeID == null || !HandleTable.TryGet(nativeObject, out SchemaObject obj))
            return InvalidArg;
        *typeID = SchemaRegistry.GetTypeId(obj.Schema.Type);
        return OK;
    }

    // "Query for the interface this type marshals as" -- the CLR analogue of QueryInterface
    // is a cast, so this succeeds when the object really does implement the requested
    // registered type and hands back the same object handle (identity is preserved by a
    // reference cast, exactly as QueryInterface on a C++ object would).
    [UnmanagedCallersOnly(EntryPoint = "SpQueryForMarshalAsInterface")]
    public static uint SpQueryForMarshalAsInterface(IntPtr nativeObject, uint interfaceID, IntPtr* interfaceImpl)
    {
        if (interfaceImpl == null || !HandleTable.TryGet(nativeObject, out SchemaObject obj))
            return InvalidArg;

        Type requested = SchemaRegistry.GetType(interfaceID);
        if (requested == null || !requested.IsInstanceOfType(obj.Instance))
        {
            *interfaceImpl = IntPtr.Zero;
            return Fail;
        }

        obj.RefCount++;
        *interfaceImpl = nativeObject;
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpSetStateCache")]
    public static void SpSetStateCache(IntPtr nativeObject, ulong state)
    {
        if (HandleTable.TryGet(nativeObject, out SchemaObject obj))
            obj.StateCache = state;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpGetStateCache")]
    public static void SpGetStateCache(IntPtr nativeObject, ulong* state)
    {
        if (state == null)
            return;
        *state = HandleTable.TryGet(nativeObject, out SchemaObject obj) ? obj.StateCache : 0UL;
    }

    // ---- enum-schema-level -----------------------------------------------------------

    [UnmanagedCallersOnly(EntryPoint = "SpQueryEnumName")]
    public static uint SpQueryEnumName(IntPtr nativeObject, char** name)
    {
        if (name == null || !HandleTable.TryGet(nativeObject, out EnumSchema schema))
            return InvalidArg;
        *name = NativeString.InternUni(schema.Name);
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpQueryEnumIsFlags")]
    public static uint SpQueryEnumIsFlags(IntPtr nativeObject, int* isFlags)
    {
        if (isFlags == null || !HandleTable.TryGet(nativeObject, out EnumSchema schema))
            return InvalidArg;
        *isFlags = schema.IsFlags ? 1 : 0;
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpQueryEnumValueCount")]
    public static uint SpQueryEnumValueCount(IntPtr nativeObject, uint* valueCount)
    {
        if (valueCount == null || !HandleTable.TryGet(nativeObject, out EnumSchema schema))
            return InvalidArg;
        *valueCount = (uint)schema.Names.Length;
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpGetEnumNameValue")]
    public static uint SpGetEnumNameValue(IntPtr nativeObject, uint index, char** name, int* value)
    {
        if (name == null || value == null || !HandleTable.TryGet(nativeObject, out EnumSchema schema))
            return InvalidArg;
        if (index >= schema.Names.Length)
            return InvalidArg;

        *name = NativeString.InternUni(schema.Names[index]);
        *value = schema.Values[index];
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpInvokeEnumToString")]
    public static uint SpInvokeEnumToString(IntPtr nativeObject, int value, IntPtr* result)
    {
        if (result == null || !HandleTable.TryGet(nativeObject, out EnumSchema schema))
            return InvalidArg;

        string text = Enum.ToObject(schema.Type, value).ToString();
        *result = (IntPtr)NativeString.AllocUni(text);
        return OK;
    }

    // ---- helpers ---------------------------------------------------------------------

    private static object[] ToObjects(UIXVariant* parameters, uint count)
    {
        if (parameters == null || count == 0)
            return Array.Empty<object>();

        var values = new object[count];
        for (uint i = 0; i < count; i++)
            values[i] = parameters[i].ToObject();
        return values;
    }

    // Reflection needs the exact declared type (an int variant assigned to a `long`
    // parameter would otherwise throw), and UIXVariant only carries a handful of
    // primitive shapes -- so widen/narrow to the target here rather than at every call site.
    private static object Coerce(object value, Type target)
    {
        if (value == null || target.IsInstanceOfType(value))
            return value;
        if (target.IsEnum)
            return Enum.ToObject(target, value);
        return Convert.ChangeType(value, target);
    }

    private static uint WriteParameterTypes(ParameterInfo[] parameters, uint* ids, uint count)
    {
        uint writable = Math.Min(count, (uint)parameters.Length);
        for (uint i = 0; i < writable; i++)
            ids[i] = SchemaRegistry.GetTypeId(parameters[i].ParameterType);
        return OK;
    }
}

#endif
