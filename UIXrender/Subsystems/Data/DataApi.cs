using System;
using System.Runtime.InteropServices;
using Microsoft.Iris.Interop;
using Microsoft.Iris.Render.Engine;
using Microsoft.Iris.Render.Interop;

namespace Microsoft.Iris.Render.Subsystems.Data;

// [UnmanagedCallersOnly] exports for the SpData* family in UIX/Microsoft/Iris/OS/NativeApi.cs.
public static unsafe class DataApi
{
    private static uint OK => (uint)HRESULT.S_OK.hr;
    private static uint InvalidArg => unchecked((uint)HRESULT.E_INVALIDARG.hr);

    // Mirrors NativeApi.NativeDataMappingEntry's marshaled layout: four LPWStr pointers
    // interleaved with two 64-bit type handles, then a UIXVariant. Declared here rather
    // than in Interop/ because nothing outside this subsystem reads it.
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeDataMappingEntry
    {
        public IntPtr Source;
        public IntPtr Target;
        public IntPtr PropertyName;
        public ulong PropertyTypeHandle;
        public IntPtr PropertyTypeName;
        public ulong UnderlyingCollectionTypeHandle;
        public IntPtr UnderlyingCollectionTypeName;
        public UIXVariant DefaultValue;
    }

    // ---- provider / factory ----------------------------------------------------------

    [UnmanagedCallersOnly(EntryPoint = "SpDataProviderConstructQuery")]
    public static uint SpDataProviderConstructQuery(IntPtr nativeFactory, char* providerName, ulong queryTypeHandle, ulong resultTypeHandle, ulong queryHandle, IntPtr* query)
    {
        if (query == null)
            return InvalidArg;

        var constructed = new DataQuery(NativeString.UniToString(providerName), queryTypeHandle, resultTypeHandle, queryHandle);
        *query = HandleTable.Alloc(constructed);
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpDataProviderReportDataMapping")]
    public static uint SpDataProviderReportDataMapping(IntPtr nativeCallback, char* providerName, ulong typeHandle, uint entryCount, NativeDataMappingEntry* entries)
    {
        if (entries == null && entryCount > 0)
            return InvalidArg;

        // The callback pointer identifies the factory collecting these mappings; a caller
        // that hasn't registered one yet still gets a well-defined success, matching the
        // original's fire-and-forget "report" shape.
        DataProviderFactory factory = HandleTable.Get<DataProviderFactory>(nativeCallback);
        if (factory == null)
            return OK;

        string provider = NativeString.UniToString(providerName);
        for (uint i = 0; i < entryCount; i++)
        {
            NativeDataMappingEntry entry = entries[i];
            factory.ReportMapping(new DataMapping
            {
                ProviderName = provider,
                Source = Marshal.PtrToStringUni(entry.Source),
                Target = Marshal.PtrToStringUni(entry.Target),
                PropertyName = Marshal.PtrToStringUni(entry.PropertyName),
                PropertyTypeHandle = entry.PropertyTypeHandle,
                PropertyTypeName = Marshal.PtrToStringUni(entry.PropertyTypeName),
                UnderlyingCollectionTypeHandle = entry.UnderlyingCollectionTypeHandle,
                UnderlyingCollectionTypeName = Marshal.PtrToStringUni(entry.UnderlyingCollectionTypeName),
                DefaultValue = entry.DefaultValue,
            });
        }

        return OK;
    }

    // ---- query lifecycle -------------------------------------------------------------

    [UnmanagedCallersOnly(EntryPoint = "SpDataQueryNotifyInitialized")]
    public static uint SpDataQueryNotifyInitialized(IntPtr nativeQuery)
    {
        if (!HandleTable.TryGet(nativeQuery, out DataQuery query))
            return InvalidArg;
        query.NotifyInitialized();
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpDataQueryRefresh")]
    public static uint SpDataQueryRefresh(IntPtr nativeQuery)
    {
        if (!HandleTable.TryGet(nativeQuery, out DataQuery query))
            return InvalidArg;
        query.Refresh();
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpDataQueryGetEnabledProperty")]
    public static uint SpDataQueryGetEnabledProperty(IntPtr nativeQuery, int* propertyValue)
    {
        if (propertyValue == null || !HandleTable.TryGet(nativeQuery, out DataQuery query))
            return InvalidArg;
        *propertyValue = query.Enabled ? 1 : 0;
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpDataQuerySetEnabledProperty")]
    public static uint SpDataQuerySetEnabledProperty(IntPtr nativeQuery, int propertyValue)
    {
        if (!HandleTable.TryGet(nativeQuery, out DataQuery query))
            return InvalidArg;
        query.Enabled = propertyValue != 0;
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpDataQueryGetStatusProperty")]
    public static uint SpDataQueryGetStatusProperty(IntPtr nativeQuery, DataProviderQueryStatus* propertyValue)
    {
        if (propertyValue == null || !HandleTable.TryGet(nativeQuery, out DataQuery query))
            return InvalidArg;
        *propertyValue = query.Status;
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpDataQuerySetStatusProperty")]
    public static uint SpDataQuerySetStatusProperty(IntPtr nativeQuery, DataProviderQueryStatus propertyValue)
    {
        if (!HandleTable.TryGet(nativeQuery, out DataQuery query))
            return InvalidArg;
        query.Status = propertyValue;
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpDataQueryGetResultProperty")]
    public static uint SpDataQueryGetResultProperty(IntPtr nativeQuery, UIXVariant* propertyValue)
    {
        if (propertyValue == null || !HandleTable.TryGet(nativeQuery, out DataQuery query))
            return InvalidArg;
        *propertyValue = query.Result;
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpDataQuerySetResultProperty")]
    public static uint SpDataQuerySetResultProperty(IntPtr nativeQuery, UIXVariant* propertyValue)
    {
        if (propertyValue == null || !HandleTable.TryGet(nativeQuery, out DataQuery query))
            return InvalidArg;
        query.Result = *propertyValue;
        return OK;
    }

    // ---- base object -----------------------------------------------------------------

    [UnmanagedCallersOnly(EntryPoint = "SpDataBaseObjectGetTypeHandle")]
    public static void SpDataBaseObjectGetTypeHandle(IntPtr nativeQuery, ulong* typeHandle)
    {
        if (typeHandle == null)
            return;
        *typeHandle = HandleTable.TryGet(nativeQuery, out DataBaseObject obj) ? obj.TypeHandle : 0UL;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpDataBaseObjectGetProperty")]
    public static uint SpDataBaseObjectGetProperty(IntPtr nativeQuery, char* propertyName, UIXVariant* propertyValue)
    {
        if (propertyValue == null || !HandleTable.TryGet(nativeQuery, out DataBaseObject obj))
            return InvalidArg;

        *propertyValue = obj.TryGetProperty(NativeString.UniToString(propertyName), out UIXVariant value) ? value : UIXVariant.Empty;
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpDataBaseObjectSetProperty")]
    public static uint SpDataBaseObjectSetProperty(IntPtr nativeQuery, char* propertyName, UIXVariant* propertyValue)
    {
        if (propertyValue == null || !HandleTable.TryGet(nativeQuery, out DataBaseObject obj))
            return InvalidArg;

        obj.SetProperty(NativeString.UniToString(propertyName), *propertyValue);
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpDataBaseObjectSetInternalHandle")]
    public static uint SpDataBaseObjectSetInternalHandle(IntPtr nativeQuery, ulong frameworkQuery)
    {
        if (!HandleTable.TryGet(nativeQuery, out DataBaseObject obj))
            return InvalidArg;
        obj.InternalHandle = frameworkQuery;
        return OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpDataBaseObjectGetInternalHandle")]
    public static uint SpDataBaseObjectGetInternalHandle(IntPtr nativeQuery, ulong* frameworkQuery)
    {
        if (frameworkQuery == null || !HandleTable.TryGet(nativeQuery, out DataBaseObject obj))
            return InvalidArg;
        *frameworkQuery = obj.InternalHandle;
        return OK;
    }
}
