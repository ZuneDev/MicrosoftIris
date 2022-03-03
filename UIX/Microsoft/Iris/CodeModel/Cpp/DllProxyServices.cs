// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.CodeModel.Cpp.DllProxyServices
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Markup;
using Microsoft.Iris.OS;
using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI;
using Microsoft.Iris.Session;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.CodeModel.Cpp
{
    internal class DllProxyServices : IRawUIXServices
    {
        private static StringProxyHandleTable s_stringTable;

        public static void Startup()
        {
            DllProxyObject.CreateHandleTable();
            s_stringTable = new StringProxyHandleTable();
            int num = (int)NativeApi.SpRegisterNativeServicesCallbacks(new DllProxyServices());
        }

        public static void Shutdown()
        {
            DllProxyObject.ReleaseOutstandingProxies();
            NativeMarkupDataType.ReleaseOutstandingProxies();
            NativeApi.SpUnregisterNativeServicesCallbacks();
            s_stringTable = null;
        }

        HRESULT IRawUIXServices.NotifyChangeForObject(
          IntPtr nativeObject,
          uint changedID)
        {
            return DllProxyObject.OnChangeNotification(nativeObject, changedID);
        }

        private static void Crash(string message) => throw new Exception(message);

        void IRawUIXServices.Crash(string message) => Crash(message);

        public static string GetString(IntPtr nativeStringObject)
        {
            string str = null;
            if (nativeStringObject != IntPtr.Zero)
            {
                ulong handle;
                NativeApi.SpGetStringHandle(nativeStringObject, out handle);
                if (handle == 0UL)
                    NativeApi.SpConvertStringToManaged(nativeStringObject, out handle);
                s_stringTable.LookupByHandle(handle, out str);
                NativeApi.SpReleaseExternalObject(nativeStringObject);
            }
            return str;
        }

        public static void CreateNativeString(string value, out IntPtr nativeObject)
        {
            nativeObject = IntPtr.Zero;
            if (value == null || NativeApi.SpCreateNativeString(AllocateStringHandle(value), value.Length, out nativeObject))
                return;
            Crash("Unable to allocate string");
        }

        unsafe ulong IRawUIXServices.AllocateString(char* value, out int length)
        {
            string str = new string(value);
            length = str.Length;
            return AllocateStringHandle(str);
        }

        private static ulong AllocateStringHandle(string value)
        {
            ulong handle;
            s_stringTable.GetStringHandle(value, out handle);
            return handle;
        }

        unsafe void IRawUIXServices.CopyString(
          ulong handle,
          char* target,
          uint targetSize)
        {
            string source;
            s_stringTable.LookupByHandle(handle, out source);
            NativeApi.SpCopyString(source, target, targetSize);
        }

        unsafe char* IRawUIXServices.PinString(ulong handle)
        {
            char* chPtr;
            s_stringTable.PinString(handle, out chPtr);
            return chPtr;
        }

        void IRawUIXServices.UnpinString(ulong handle) => s_stringTable.UnpinString(handle);

        void IRawUIXServices.ReleaseString(ulong handle) => s_stringTable.ReleaseStringHandle(handle, out string _);

        public static UIImage GetImage(IntPtr nativeImageObject)
        {
            UIImage uiImage = null;
            if (nativeImageObject != IntPtr.Zero)
            {
                ulong handle;
                NativeApi.SpGetImageHandle(nativeImageObject, out handle);
                if (handle == 0UL)
                    NativeApi.SpConvertImageToManaged(nativeImageObject, out handle);
                uiImage = (UIImage)GCHandle.FromIntPtr(new IntPtr((long)handle)).Target;
                NativeApi.SpReleaseExternalObject(nativeImageObject);
            }
            return uiImage;
        }

        public static MarkupDataQuery GetDataQuery(IntPtr nativeQuery)
        {
            MarkupDataQuery markupDataQuery = null;
            if (nativeQuery != IntPtr.Zero)
            {
                ulong frameworkQuery;
                int internalHandle = (int)NativeApi.SpDataBaseObjectGetInternalHandle(nativeQuery, out frameworkQuery);
                markupDataQuery = NativeMarkupDataQuery.LookupByHandle(frameworkQuery);
                NativeApi.SpReleaseExternalObject(nativeQuery);
            }
            return markupDataQuery;
        }

        public static MarkupDataType GetDataType(IntPtr nativeObject)
        {
            NativeMarkupDataType nativeMarkupDataType = null;
            if (nativeObject != IntPtr.Zero)
            {
                ulong frameworkQuery;
                int internalHandle = (int)NativeApi.SpDataBaseObjectGetInternalHandle(nativeObject, out frameworkQuery);
                if (frameworkQuery != 0UL)
                    nativeMarkupDataType = NativeMarkupDataType.LookupByHandle(frameworkQuery);
                if (nativeMarkupDataType == null)
                {
                    ulong typeHandle;
                    NativeApi.SpDataBaseObjectGetTypeHandle(nativeObject, out typeHandle);
                    nativeMarkupDataType = NativeMarkupDataType.Create(typeHandle, nativeObject);
                }
                NativeApi.SpReleaseExternalObject(nativeObject);
            }
            return nativeMarkupDataType;
        }

        public static void CreateNativeImage(UIImage image, out IntPtr nativeObject)
        {
            nativeObject = IntPtr.Zero;
            if (image == null || !NativeApi.SpCreateNativeImage(GetImageHandle(image, image.Source), image.Source, out nativeObject).IsError())
                return;
            Crash("Unable to allocate native image object");
        }

        private static ulong GetImageHandle(UIImage image, string source) => (ulong)GCHandle.ToIntPtr(GCHandle.Alloc(image)).ToInt64();

        unsafe ulong IRawUIXServices.AllocateImageFromUri(
          string uri,
          NativeApi.IMAGE_DECODE_PARAMS* decodeParams)
        {
            Size maximumSize;
            bool flippable;
            bool antialiasEdges;
            CrackDecodeParams(decodeParams, out maximumSize, out flippable, out antialiasEdges);
            return GetImageHandle(new UriImage(uri, Inset.Zero, maximumSize, flippable, antialiasEdges), uri);
        }

        unsafe ulong IRawUIXServices.AllocateImageFromBits(
          string ID,
          NativeApi.UIX_RAW_IMAGE_INFO* imageInfo,
          NativeApi.IMAGE_DECODE_PARAMS* decodeParams)
        {
            SurfaceFormat surfaceFormat;
            ImageFormatUtils.RawImageFormatToSurfaceFormat(imageInfo->format, out surfaceFormat);
            Size maximumSize;
            bool flippable;
            bool antialiasEdges;
            CrackDecodeParams(decodeParams, out maximumSize, out flippable, out antialiasEdges);
            Size imageSize = new Size(imageInfo->width, imageInfo->height);
            return GetImageHandle(new RawImage(ID, imageSize, imageInfo->stride, surfaceFormat, imageInfo->bits, true, Inset.Zero, maximumSize, flippable, antialiasEdges), ID);
        }

        unsafe void IRawUIXServices.RemoveCachedImage(
          string ID,
          NativeApi.IMAGE_DECODE_PARAMS* decodeParams)
        {
            Size maximumSize;
            bool flippable;
            bool antialiasEdges;
            CrackDecodeParams(decodeParams, out maximumSize, out flippable, out antialiasEdges);
            UriImage.RemoveCache(ID, maximumSize, flippable, antialiasEdges);
        }

        private static unsafe void CrackDecodeParams(
          NativeApi.IMAGE_DECODE_PARAMS* decodeParams,
          out Size maximumSize,
          out bool flippable,
          out bool antialiasEdges)
        {
            if ((IntPtr)decodeParams != IntPtr.Zero)
            {
                maximumSize = new Size(decodeParams->maximumWidth, decodeParams->maximumHeight);
                flippable = decodeParams->Flippable;
                antialiasEdges = decodeParams->AntialiasEdges;
            }
            else
            {
                maximumSize = Size.Zero;
                flippable = false;
                antialiasEdges = false;
            }
        }

        void IRawUIXServices.ReleaseImage(ulong handle) => GCHandle.FromIntPtr(new IntPtr((long)handle)).Free();

        void IRawUIXServices.RegisterDataProvider(
          string providerName,
          IntPtr nativeFactoryCallback)
        {
            MarkupDataProvider.RegisterDataProvider(new NativeDataProviderWrapper(providerName, nativeFactoryCallback));
        }

        unsafe void IRawUIXServices.GetDataMapping(
          string providerName,
          ulong typeHandle,
          IntPtr nativeCallback)
        {
            if (!(TypeSchema.LookupById(typeHandle) is MarkupDataTypeSchema typeSchema))
                return;
            MarkupDataMapping dataMapping = MarkupDataProvider.FindDataMapping(providerName, typeSchema);
            NativeApi.NativeDataMappingEntry[] entries = new NativeApi.NativeDataMappingEntry[dataMapping.Mappings.Length];
            for (int index = 0; index < entries.Length; ++index)
            {
                MarkupDataMappingEntry mapping = dataMapping.Mappings[index];
                entries[index].Source = mapping.Source;
                entries[index].Target = mapping.Target;
                entries[index].PropertyName = mapping.Property.Name;
                entries[index].PropertyTypeName = mapping.Property.PropertyType.Name;
                entries[index].PropertyTypeHandle = mapping.Property.PropertyType.UniqueId;
                if (mapping.Property.AlternateType != null)
                {
                    entries[index].UnderlyingCollectionTypeName = mapping.Property.AlternateType.Name;
                    entries[index].UnderlyingCollectionTypeHandle = mapping.Property.AlternateType.UniqueId;
                }
                // ISSUE: untyped stack allocation
                UIXVariant* destination = stackalloc UIXVariant[sizeof(UIXVariant)];
                UIXVariant.MarshalObject(mapping.DefaultValue, destination);
                entries[index].DefaultValue = *destination;
            }
            int num = (int)NativeApi.SpDataProviderReportDataMapping(nativeCallback, providerName, typeHandle, (uint)entries.Length, entries);
        }

        void IRawUIXServices.NotifyChangeForDataObject(
          ulong frameworkObjectHandle,
          string propertyName,
          UIXVariant.VariantType variantType)
        {
            MarkupDataTypeBaseObject dataTypeBaseObject = null;
            switch (variantType)
            {
                case UIXVariant.VariantType.UIXDataQuery:
                    dataTypeBaseObject = NativeMarkupDataQuery.LookupByHandle(frameworkObjectHandle);
                    break;
                case UIXVariant.VariantType.UIXDataType:
                    dataTypeBaseObject = NativeMarkupDataType.LookupByHandle(frameworkObjectHandle);
                    break;
            }
            if (dataTypeBaseObject == null)
                return;
            propertyName = NotifyService.CanonicalizeString(propertyName);
            dataTypeBaseObject.FireNotificationThreadSafe(propertyName);
        }

        IntPtr IRawUIXServices.GetAppWindowHandle()
        {
            IntPtr num = IntPtr.Zero;
            if (UISession.Default != null && UISession.Default.Form != null)
                num = UISession.Default.Form.__WindowHandle;
            return num;
        }

        void IRawUIXServices.ReportError(bool isWarning, string message)
        {
            if (isWarning)
                ErrorManager.ReportWarning(message);
            else
                ErrorManager.ReportError(message);
        }

        void IRawUIXServices.LowPriorityDeferredInvoke(IntPtr callback, IntPtr data) => DeferredCall.Post(DispatchPriority.Idle, new SimpleCallback(new DllProxyServices.DeferredInvokeState()
        {
            _callback = callback,
            _data = data
        }.Thunk));

        internal class DeferredInvokeState
        {
            public IntPtr _callback;
            public IntPtr _data;

            public void Thunk() => NativeApi.SpCallDeferredInvokeProc(_callback, _data);
        }
    }
}
