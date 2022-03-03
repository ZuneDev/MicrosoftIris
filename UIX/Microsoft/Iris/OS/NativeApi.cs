// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.OS.NativeApi
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.CodeModel.Cpp;
using Microsoft.Iris.Drawing;
using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI;
using Microsoft.Iris.RenderAPI.Drawing;
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Microsoft.Iris.OS
{
    [SuppressUnmanagedCodeSecurity]
    internal static unsafe class NativeApi
    {
        private const string s_stRenderDll = "UIXRender.dll";
        private const string s_stSupportDll = "UIXsup.dll";
        public const int DOWNLOAD_ERROR_GENERALFAILURE = -1;
        public const int DOWNLOAD_ERROR_NONE = 0;
        internal const uint WM_IME_STARTCOMPOSITION = 269;
        internal const uint WM_IME_ENDCOMPOSITION = 270;
        internal const uint FM_DEFERRED_IME_DISASSOCIATE_CONTEXT = 1032;
        public const int HTTPDOWNLOAD_ERROR_INVALIDURI = 1;
        public const int HTTPDOWNLOAD_ERROR_HOSTCONNECTIONFAILED = 2;

        public static bool SUCCEEDED(uint hr) => (int)hr >= 0;

        public static bool FAILED(uint hr) => (int)hr < 0;

        [DllImport("UIXRender.dll")]
        public static extern void SpInitializeTracing();

        [DllImport("UIXRender.dll")]
        public static extern void SpUninitializeTracing();

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern void SpUpdateTraceSettings(
          string debugTraceFile,
          string writeLinePrefix,
          bool sendOutputToDebugger,
          bool showCategories,
          bool timedWriteLines);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern void SpLogTrace(string categoryName, string message, int indentLevel);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SpDownloadGetBuffer(IntPtr handle);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpDownloadClose(IntPtr handle);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpFileDownload(
          string path,
          NativeApi.DownloadCompleteHandler handler,
          IntPtr context,
          out IntPtr handle);

        public static IntPtr DownloadGetBuffer(IntPtr handle)
        {
            IntPtr buffer = SpDownloadGetBuffer(handle);
            return !(buffer == IntPtr.Zero) ? buffer : throw new OutOfMemoryException();
        }

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern bool SpLoadBinaryResource(
          string moduleBaseName,
          string resourceName,
          bool allowLoadAsCode,
          out IntPtr pBits,
          out uint size);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern bool SpLoadFontResource(string moduleBaseName, string resourceName);

        [DllImport("UIXRender.dll")]
        private static extern IntPtr SpMemAlloc(uint cb, bool zeroMemory);

        [DllImport("UIXRender.dll")]
        private static extern void SpMemFree(IntPtr pv);

        public static IntPtr MemAlloc(uint cb, bool zeroMemory)
        {
            IntPtr num = SpMemAlloc(cb, zeroMemory);
            return !(num == IntPtr.Zero) ? num : throw new OutOfMemoryException();
        }

        public static void MemFree(IntPtr pv) => SpMemFree(pv);

        [DllImport("UIXRender.dll")]
        public static extern void SpFreeDib(IntPtr hdib);

        [DllImport("UIXRender.dll")]
        public static extern int SpGetDpi();

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpLoadDll(string uri, out IntPtr moduleHandle);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern void SpFreeDll(IntPtr moduleHandle);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpCreateDllLoadResultFactory(
          IntPtr moduleHandle,
          out IntPtr schemaFactory);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpCreateDllLoadResult(
          IntPtr schemaFactory,
          string qualifier,
          out IntPtr loadResult);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern void SpSendDllSchemaUnloadNotification(IntPtr moduleHandle);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpSetSchemaID(IntPtr schema, ushort id);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpQueryTypeCount(IntPtr schema, out uint typeCount);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpQueryEnumCount(IntPtr schema, out uint enumCount);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpGetTypeSchema(
          IntPtr schema,
          uint index,
          out IntPtr type,
          out uint ID);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpGetEnumSchema(
          IntPtr schema,
          uint index,
          out IntPtr enumType,
          out uint ID);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpQueryTypeName(IntPtr typeSchema, out char* name);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpIsRuntimeImmutable(IntPtr typeSchema, out bool isRuntimeImmutable);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpQueryBaseType(IntPtr typeSchema, out uint baseTypeID);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpGetMarshalAs(IntPtr typeSchema, out uint interopEquivalent);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpQueryConstructorCount(IntPtr typeSchema, out uint count);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpGetConstructorSchema(
          IntPtr typeSchema,
          uint index,
          out IntPtr constructor,
          out uint ID);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpQueryPropertyCount(IntPtr typeSchema, out uint count);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpGetPropertySchema(
          IntPtr typeSchema,
          uint index,
          out IntPtr property,
          out uint ID);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpQueryMethodCount(IntPtr typeSchema, out uint count);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpGetMethodSchema(
          IntPtr typeSchema,
          uint index,
          out IntPtr method,
          out uint ID);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpQueryEventCount(IntPtr typeSchema, out uint count);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpGetEventSchema(
          IntPtr typeSchema,
          uint index,
          out IntPtr eventObj,
          out uint ID);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern unsafe uint SpInvokeConstructor(
          IntPtr typeSchema,
          uint constructorID,
          UIXVariant* parameters,
          uint parameterCount,
          out IntPtr nativeObject);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpGetPropertyValue(
          IntPtr typeSchema,
          IntPtr nativeObject,
          uint propertyID,
          out UIXVariant propertyValue);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern unsafe uint SpSetPropertyValue(
          IntPtr typeSchema,
          IntPtr nativeObject,
          uint propertyID,
          UIXVariant* propertyValue);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern unsafe uint SpInvokeMethod(
          IntPtr typeSchema,
          IntPtr nativeObject,
          uint methodID,
          UIXVariant* parameters,
          uint parameterCount,
          out UIXVariant returnValue);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpInvokeToString(
          IntPtr typeSchema,
          IntPtr nativeObject,
          out IntPtr value);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpQueryConstructorParameterCount(
          IntPtr constructorSchema,
          out uint count);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpGetConstructorParameterTypes(
          IntPtr constructorSchema,
          uint[] IDs,
          uint count);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpQueryPropertyName(IntPtr propertySchema, out char* name);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpQueryPropertyType(IntPtr propertySchema, out uint type);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpQueryPropertyCanRead(IntPtr propertySchema, out bool canRead);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpQueryPropertyCanWrite(IntPtr propertySchema, out bool canWrite);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpQueryPropertyIsStatic(IntPtr propertySchema, out bool isStatic);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpQueryPropertyNotifiesOnChange(
          IntPtr propertySchema,
          out bool notifiesOnChange);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpQueryMethodName(IntPtr methodSchema, out char* name);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpQueryMethodReturnType(IntPtr methodSchema, out uint type);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpQueryMethodParameterCount(IntPtr methodSchema, out uint count);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpGetMethodParameterTypes(
          IntPtr methodSchema,
          uint[] IDs,
          uint count);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpQueryMethodIsStatic(IntPtr methodSchema, out bool isStatic);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpQueryEventName(IntPtr eventSchema, out char* name);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpQueryEventIsStatic(IntPtr eventSchema, out bool isStatic);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern void SpAddRefExternalObject(IntPtr nativeObject);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern void SpReleaseExternalObject(IntPtr nativeObject);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpGetTypeID(IntPtr nativeObject, out uint typeID);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpQueryForMarshalAsInterface(
          IntPtr nativeObject,
          uint interfaceID,
          out IntPtr interfaceImpl);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern void SpSetStateCache(IntPtr nativeObject, ulong state);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern void SpGetStateCache(IntPtr nativeObject, out ulong state);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpUIXListRegisterCallbacks(
          IntPtr nativeList,
          IUIXListCallbacks listener);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpUIXListUnregisterCallbacks(
          IntPtr nativeList,
          IUIXListCallbacks listener);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern unsafe uint SpUIXListAdd(
          IntPtr nativeList,
          UIXVariant* item,
          out int count);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpUIXListClear(IntPtr nativeList);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern unsafe uint SpUIXListIndexOf(
          IntPtr nativeList,
          UIXVariant* item,
          out int index);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern unsafe uint SpUIXListInsert(
          IntPtr nativeList,
          int index,
          UIXVariant* item);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern unsafe uint SpUIXListRemove(IntPtr nativeList, UIXVariant* item);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpUIXListRemoveAt(IntPtr nativeList, int index);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpUIXListGetItem(IntPtr nativeList, int index, out UIXVariant item);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern unsafe uint SpUIXListSetItem(
          IntPtr nativeList,
          int index,
          UIXVariant* item);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpUIXListGetCount(IntPtr nativeList, out int count);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpUIXListMove(IntPtr nativeList, int oldIndex, int newIndex);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpUIXListIsItemAvailable(
          IntPtr nativeList,
          int index,
          out bool isAvailable);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpUIXListFetchSlowData(IntPtr nativeList, int index);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpUIXListWantSlowDataRequests(
          IntPtr nativeList,
          out bool wantSlowDataRequests);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpUIXListNotifyVisualsCreated(IntPtr nativeList, int index);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpUIXListNotifyVisualsReleased(IntPtr nativeList, int index);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpQueryEnumName(IntPtr nativeObject, out char* name);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpQueryEnumIsFlags(IntPtr nativeObject, out bool isFlags);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpQueryEnumValueCount(IntPtr nativeObject, out uint valueCount);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpGetEnumNameValue(
          IntPtr nativeObject,
          uint index,
          out char* name,
          out int value);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpInvokeEnumToString(
          IntPtr nativeObject,
          int value,
          out IntPtr result);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern void SpDataBaseObjectGetTypeHandle(
          IntPtr nativeQuery,
          out ulong typeHandle);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpDataBaseObjectGetProperty(
          IntPtr nativeQuery,
          string propertyName,
          out UIXVariant propertyValue);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern unsafe uint SpDataBaseObjectSetProperty(
          IntPtr nativeQuery,
          string propertyName,
          UIXVariant* propertyValue);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpDataBaseObjectSetInternalHandle(
          IntPtr nativeQuery,
          ulong frameworkQuery);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpDataBaseObjectGetInternalHandle(
          IntPtr nativeQuery,
          out ulong frameworkQuery);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpDataProviderConstructQuery(
          IntPtr nativeFactory,
          string providerName,
          ulong queryTypeHandle,
          ulong resultTypeHandle,
          ulong queryHandle,
          out IntPtr query);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpDataQueryNotifyInitialized(IntPtr nativeQuery);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpDataQueryRefresh(IntPtr nativeQuery);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpDataQueryGetEnabledProperty(
          IntPtr nativeQuery,
          out bool propertyValue);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpDataQuerySetEnabledProperty(IntPtr nativeQuery, bool propertyValue);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpDataQueryGetStatusProperty(
          IntPtr nativeQuery,
          out DataProviderQueryStatus propertyValue);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpDataQuerySetStatusProperty(
          IntPtr nativeQuery,
          DataProviderQueryStatus propertyValue);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpDataQueryGetResultProperty(
          IntPtr nativeQuery,
          out UIXVariant propertyValue);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern unsafe uint SpDataQuerySetResultProperty(
          IntPtr nativeQuery,
          UIXVariant* propertyValue);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpDataProviderReportDataMapping(
          IntPtr nativeCallback,
          string providerName,
          ulong typeHandle,
          uint entryCount,
          [MarshalAs(UnmanagedType.LPArray)] NativeApi.NativeDataMappingEntry[] entries);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpRegisterNativeServicesCallbacks([MarshalAs(UnmanagedType.Interface)] IRawUIXServices rawServices);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern void SpUnregisterNativeServicesCallbacks();

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern void SpCallDeferredInvokeProc(IntPtr pfnCallback, IntPtr pvCallbackData);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern void SpGetStringHandle(IntPtr nativeString, out ulong handle);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern void SpConvertStringToManaged(IntPtr nativeString, out ulong handle);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern bool SpCreateNativeString(
          ulong handle,
          int length,
          out IntPtr nativeString);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern unsafe void SpCopyString(string source, char* destintation, uint length);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern void SpGetImageHandle(IntPtr nativeImage, out ulong handle);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern HRESULT SpConvertImageToManaged(
          IntPtr nativeImage,
          out ulong handle);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern HRESULT SpCreateNativeImage(
          ulong handle,
          string source,
          out IntPtr nativeImage);

        [DllImport("UIXRender.dll")]
        public static extern HRESULT SpCreateNotifyWindow(
          out IntPtr handle,
          NativeApi.NotifyWindowCallback callback);

        [DllImport("UIXRender.dll")]
        public static extern void SpDestroyNotifyWindow();

        [DllImport("UIXRender.dll")]
        internal static extern HRESULT SpRichTextBuildObject(
          bool fRichTextMode,
          Size sizeMaximumSurface,
          [MarshalAs(UnmanagedType.Interface)] IRichTextCallbacks pCallbacks,
          out Win32Api.HANDLE hRto);

        [DllImport("UIXRender.dll")]
        internal static extern void SpRichTextDestroyObject(Win32Api.HANDLE hRto);

        [DllImport("UIXRender.dll")]
        internal static extern void SpRichTextDestroyGlyphRunInfo(IntPtr hGlyphRunInfo);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        internal static extern HRESULT SpRichTextForwardKeyCharacter(
          Win32Api.HANDLE hRto,
          uint message,
          int character,
          int scanCode,
          int repeatCount,
          uint modifierState,
          ushort flags,
          out bool handled);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        internal static extern HRESULT SpRichTextForwardKeyState(
          Win32Api.HANDLE hRto,
          uint message,
          int virtualKey,
          int scanCode,
          int repeatCount,
          uint modifierState,
          ushort flags,
          out bool handled);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        internal static extern HRESULT SpRichTextForwardMouseInput(
          Win32Api.HANDLE hRto,
          uint message,
          uint modifierState,
          int mouseButton,
          int x,
          int y,
          int mouseWheelDelta,
          out bool handled);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        internal static extern HRESULT SpRichTextForwardImeMessage(
          Win32Api.HANDLE hRto,
          uint message,
          UIntPtr wParam,
          UIntPtr lParam);

        [DllImport("UIXRender.dll")]
        internal static extern HRESULT SpRichTextGetNaturalBounds(
          Win32Api.HANDLE hRto,
          out int cWidth,
          out int cHeight);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        internal static extern HRESULT SpRichTextGetSimpleContent(
          Win32Api.HANDLE hRto,
          StringBuilder textBuffer,
          int cchBuffer);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        internal static extern HRESULT SpRichTextGetSimpleContentLength(
          Win32Api.HANDLE hRto,
          out int textLength);

        [DllImport("UIXRender.dll")]
        internal static extern HRESULT SpRichTextMeasure(
          Win32Api.HANDLE hRto,
          ref TextMeasureParams.MarshalledData measureParams,
          NativeApi.ReportRunCallback rrcb,
          IntPtr pvData);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        internal static extern HRESULT SpRichTextRasterize(
          IntPtr hGlyphRunInfo,
          int fOutlineMode,
          Color clrText,
          int fShadowMode,
          out IntPtr phTextBitmap,
          out IntPtr ppvBits,
          out Size psizeBitmap);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        internal static extern HRESULT SpRichTextSetFocus(
          Win32Api.HANDLE hRto,
          bool gainingFocus);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        internal static extern HRESULT SpRichTextSetContent(
          Win32Api.HANDLE hRto,
          string pszContent);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        internal static extern HRESULT SpRichTextSetWordWrap(
          Win32Api.HANDLE hRto,
          bool fWordWrap);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        internal static extern HRESULT SpRichTextSetMaximumLength(
          Win32Api.HANDLE hRto,
          int maximumLength);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        internal static extern HRESULT SpRichTextSetDetectUrls(
          Win32Api.HANDLE hRto,
          bool detectUrls);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        internal static extern HRESULT SpRichTextSetOversampleMode(
          Win32Api.HANDLE hRto,
          bool fOversample);

        [DllImport("UIXRender.dll")]
        internal static extern HRESULT SpRichTextCut(Win32Api.HANDLE hRto);

        [DllImport("UIXRender.dll")]
        internal static extern HRESULT SpRichTextCopy(Win32Api.HANDLE hRto);

        [DllImport("UIXRender.dll")]
        internal static extern HRESULT SpRichTextPaste(Win32Api.HANDLE hRto);

        [DllImport("UIXRender.dll")]
        internal static extern HRESULT SpRichTextDelete(Win32Api.HANDLE hRto);

        [DllImport("UIXRender.dll")]
        internal static extern HRESULT SpRichTextCanUndo(Win32Api.HANDLE hRto, out bool canUndo);

        [DllImport("UIXRender.dll")]
        internal static extern HRESULT SpRichTextUndo(Win32Api.HANDLE hRto);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        internal static extern HRESULT SpRichTextSetReadOnly(Win32Api.HANDLE hRto, bool readOnly);

        [DllImport("UIXRender.dll")]
        internal static extern HRESULT SpRichTextSetScale(Win32Api.HANDLE hRto, float flScale);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        internal static extern HRESULT SpRichTextSetSelectionRange(
          Win32Api.HANDLE hRto,
          int selectionStart,
          int selectionEnd);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        internal static extern HRESULT SpRichTextOnTimerTick(Win32Api.HANDLE hRto, uint timerId);

        [DllImport("UIXRender.dll")]
        internal static extern bool SpSimpleTextIsAvailable();

        [DllImport("UIXRender.dll")]
        internal static extern HRESULT SpSimpleTextBuildObject(
          Size sizeMaximumSurface,
          out Win32Api.HANDLE hSto);

        [DllImport("UIXRender.dll")]
        internal static extern void SpSimpleTextDestroyObject(Win32Api.HANDLE hSto);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        internal static extern unsafe HRESULT SpSimpleTextMeasure(
          Win32Api.HANDLE hSto,
          string pszRef,
          short wAlignment,
          TextStyle.MarshalledData* textStyle,
          Size sizeConstraint,
          out IntPtr hGlyphRunInfo,
          NativeApi.RasterizeRunPacket* pRun);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        internal static extern unsafe HRESULT SpSimpleTextMeasurePossible(
          Win32Api.HANDLE hSto,
          string pszRef,
          TextStyle.MarshalledData* textStyle,
          out bool fPossible);

        [DllImport("UIXRender.dll")]
        internal static extern HRESULT SpRichTextScroll(
          Win32Api.HANDLE hRto,
          int whichBar,
          int scrollType);

        [DllImport("UIXRender.dll")]
        internal static extern HRESULT SpRichTextScrollToPosition(
          Win32Api.HANDLE hRto,
          int whichBar,
          int whereTo);

        [DllImport("UIXRender.dll")]
        internal static extern HRESULT SpRichTextSetScrollbars(
          Win32Api.HANDLE hRto,
          bool allowVertical,
          bool allowHorizontal);

        [DllImport("UIXRender.dll")]
        internal static extern HRESULT SpRegisterImeCallbacks(
          [MarshalAs(UnmanagedType.Interface)] IImeCallbacks pImeCallbacks,
          out uint dwToken);

        [DllImport("UIXRender.dll")]
        internal static extern HRESULT SpUnregisterImeCallbacks(uint dwToken);

        [DllImport("UIXRender.dll")]
        internal static extern HRESULT SpPostDeferredImeMessage(
          uint message,
          UIntPtr wParam,
          UIntPtr lParam);

        [DllImport("UIXRender.dll")]
        public static extern int SpExtractDroppedFileNames(
          IntPtr punk,
          NativeApi.ExtractDroppedFileNamesCallback callback);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpXmlLiteCreateXmlReader(
          IntPtr buffer,
          int length,
          bool isFragment,
          out IntPtr xmlReader);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern void SpXmlLiteDeleteXmlReader(IntPtr xmlReader);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpXmlLiteRead(IntPtr xmlReader, out NativeXmlNodeType nodeType);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpXmlLiteMoveToFirstAttribute(IntPtr xmlReader);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpXmlLiteMoveToNextAttribute(IntPtr xmlReader);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern bool SpXmlLiteIsEmptyElement(IntPtr xmlReader);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpXmlLiteGetQualifiedName(
          IntPtr xmlReader,
          out IntPtr name,
          out uint length);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpXmlLiteGetLocalName(
          IntPtr xmlReader,
          out IntPtr name,
          out uint length);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpXmlLiteGetPrefix(
          IntPtr xmlReader,
          out IntPtr prefix,
          out uint length);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpXmlLiteGetValue(
          IntPtr xmlReader,
          out IntPtr value,
          out uint length);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpXmlLiteGetLinePosition(IntPtr xmlReader, out uint linePosition);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpXmlLiteGetLineNumber(IntPtr xmlReader, out uint lineNumber);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern HRESULT SpRegNotifyChangeKey(
          IntPtr hkey,
          string wszPath,
          NativeApi.RegChangeCallback callback,
          out IntPtr handle);

        [DllImport("UIXRender.dll")]
        public static extern HRESULT SpRegRevokeNotifyChangeKey(IntPtr handle);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern void SpHttpStartup();

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern uint SpHttpDownload(
          string url,
          NativeApi.DownloadCompleteHandler handler,
          IntPtr context,
          out IntPtr handle);

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern void SpHttpShutdown();

        [DllImport("UIXRender.dll")]
        public static extern void SpHttpFlushProxyCache();

        [DllImport("UIXRender.dll", CharSet = CharSet.Unicode)]
        public static extern void SpGetMouseCursorInfo(out int height, out int hotY);

        public static string PtrToStringUni(IntPtr psz, int length) => length == 0 ? "" : Marshal.PtrToStringUni(psz, length);

        public delegate void DownloadCompleteHandler(
          IntPtr handle,
          int error,
          uint length,
          IntPtr context);

        [ComVisible(false)]
        public struct NativeDataMappingEntry
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string Source;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string Target;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string PropertyName;
            public ulong PropertyTypeHandle;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string PropertyTypeName;
            public ulong UnderlyingCollectionTypeHandle;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string UnderlyingCollectionTypeName;
            public UIXVariant DefaultValue;
        }

        [ComVisible(false)]
        public struct IMAGE_DECODE_PARAMS
        {
            public const uint UIX_IMAGE_OPT_FLIPPABLE = 1;
            public const uint UIX_IMAGE_OPT_ANTIALIAS_EDGES = 2;
            public int maximumWidth;
            public int maximumHeight;
            public uint options;

            public bool Flippable => ((int)options & 1) == 1;

            public bool AntialiasEdges => ((int)options & 2) == 2;
        }

        [ComVisible(false)]
        public struct UIX_RAW_IMAGE_INFO
        {
            public int width;
            public int height;
            public int stride;
            public RawImageFormat format;
            public IntPtr bits;
        }

        public enum NotificationType
        {
            GetObject,
        }

        public delegate IntPtr NotifyWindowCallback(
          NativeApi.NotificationType notification,
          int param1,
          int param2);

        [ComVisible(false)]
        public struct RasterizeRunPacket
        {
            public Rectangle rcLayoutBounds;
            public RectangleF rcfRenderBounds;
            public int naturalX;
            public int naturalY;
            public int rasterizeX;
            public int rasterizeY;
            public byte AAConfig;
            public Color clrText;
            public Color clrBackground;
            public int fontFaceUniqueId;
            public Win32Api.LOGFONTW_STRUCT lf;
            public Size sizeRasterizeRun;
            public Size sizeNatural;
            public int ascenderInset;
            public int baselineInset;
            public int nLineNumber;
            public int dwEffects;
            public NativeApi.UnderlineStyle usUnderlineStyle;
            public Rectangle rcUnderlineBounds;
        }

        public enum UnderlineStyle
        {
            None,
            Solid,
            Thick,
            Dotted,
            Dash,
            DashDot,
            DashDotDot,
        }

        internal unsafe delegate HRESULT ReportRunCallback(
          IntPtr hGlyphRunInfo,
          NativeApi.RasterizeRunPacket* pRun,
          IntPtr lpString,
          uint nChars,
          IntPtr pvData);

        internal enum ScrollNotification
        {
            LineUp,
            LineDown,
            PageUp,
            PageDown,
        }

        public delegate void ExtractDroppedFileNamesCallback(
          uint totalFileCount,
          uint currentIndex,
          [MarshalAs(UnmanagedType.LPWStr)] string filename);

        public delegate void RegChangeCallback();
    }
}
