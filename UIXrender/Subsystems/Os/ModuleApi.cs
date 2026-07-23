using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Iris.Interop;
using Microsoft.Iris.Render.Engine;
using Microsoft.Iris.Render.Interop;
using Microsoft.Iris.Render.Subsystems.Schema;

namespace Microsoft.Iris.Render.Subsystems.Os;

// [UnmanagedCallersOnly] exports for the "resource / DLL loading" family in
// UIX/Microsoft/Iris/OS/NativeApi.cs -- how markup (.uix/.uib) pulls in an assembly and
// discovers the types it can bind to.
//
// Real, and deliberately managed: SpLoadDll loads a **.NET assembly** and
// SpCreateDllLoadResultFactory projects its exported types through the schema subsystem
// (Subsystems/Schema), which is exactly what the managed caller then walks via
// SpQueryTypeCount/SpGetTypeSchema/... That keeps the whole "markup references a class by
// name" pipeline working end-to-end on any platform, instead of depending on the Win32
// loader and the original's C++ type registry.
public static unsafe class ModuleApi
{
    private static uint OK => (uint)HRESULT.S_OK.hr;
    private static uint Fail => unchecked((uint)HRESULT.E_FAIL.hr);
    private static uint InvalidArg => unchecked((uint)HRESULT.E_INVALIDARG.hr);

    private sealed class LoadedModule(Assembly assembly, string location)
    {
        public Assembly Assembly { get; } = assembly;
        public string Location { get; } = location;
    }

    // Trim-unsafe by design: loading an assembly the host names at runtime is this
    // export's entire purpose. Suppressed with justification rather than left warning --
    // see SchemaRegistration.FromAssembly for how markup-visible assemblies must be
    // rooted by the host instead.
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "SpLoadDll exists precisely to load a host-chosen assembly at runtime; the host must root it.")]
    [UnmanagedCallersOnly(EntryPoint = "SpLoadDll")]
    public static uint SpLoadDll(char* uri, IntPtr* moduleHandle)
    {
        if (moduleHandle == null)
            return InvalidArg;

        *moduleHandle = IntPtr.Zero;
        string path = NativeString.UniToString(uri);
        if (string.IsNullOrEmpty(path))
            return InvalidArg;

        try
        {
            // Assembly.LoadFrom resolves alongside dependencies the same way the original
            // Win32 LoadLibrary did for its neighbours.
            Assembly assembly = Assembly.LoadFrom(Path.GetFullPath(path));
            *moduleHandle = HandleTable.Alloc(new LoadedModule(assembly, path));
            return OK;
        }
        catch (Exception e) when (e is IOException or BadImageFormatException)
        {
            return Fail;
        }
    }

    // The CLR has no assembly-unload for the default load context, so this releases our
    // own handle rather than pretending the code was evicted. Documented rather than
    // silently doing nothing: an unloadable AssemblyLoadContext would change observable
    // type identity, which markup depends on staying stable for the process lifetime.
    [UnmanagedCallersOnly(EntryPoint = "SpFreeDll")]
    public static void SpFreeDll(IntPtr moduleHandle) => HandleTable.Free(moduleHandle);

    [UnmanagedCallersOnly(EntryPoint = "SpCreateDllLoadResultFactory")]
    public static uint SpCreateDllLoadResultFactory(IntPtr moduleHandle, IntPtr* schemaFactory)
    {
        if (schemaFactory == null)
            return InvalidArg;

        *schemaFactory = IntPtr.Zero;
        if (!HandleTable.TryGet(moduleHandle, out LoadedModule module))
            return InvalidArg;

        try
        {
            *schemaFactory = HandleTable.Alloc(SchemaRegistration.FromAssembly(module.Assembly));
            return OK;
        }
        catch (ReflectionTypeLoadException)
        {
            return Fail;
        }
    }

    // The "qualifier" selects a sub-view of a module's schema (markup can reference one
    // module under several qualified names). Nothing in this reimplementation partitions
    // a module's exported types, so the load result is the whole registration -- a real
    // result, and the same object the factory already produced.
    [UnmanagedCallersOnly(EntryPoint = "SpCreateDllLoadResult")]
    public static uint SpCreateDllLoadResult(IntPtr schemaFactory, char* qualifier, IntPtr* loadResult)
    {
        if (loadResult == null)
            return InvalidArg;

        *loadResult = IntPtr.Zero;
        if (!HandleTable.TryGet(schemaFactory, out SchemaRegistration registration))
            return InvalidArg;

        *loadResult = HandleTable.Alloc(registration);
        return OK;
    }

    // Advisory notification that a module's schema is going away; there are no cached
    // per-module schema views to invalidate in this implementation.
    [UnmanagedCallersOnly(EntryPoint = "SpSendDllSchemaUnloadNotification")]
    public static void SpSendDllSchemaUnloadNotification(IntPtr moduleHandle) { }

    // Real, cross-platform: reads an embedded managed resource out of the loaded
    // assembly (the .NET equivalent of a Win32 RT_RCDATA resource) into an unmanaged
    // buffer the caller reads directly. `moduleBaseName` selects the assembly by simple
    // name among those already loaded, matching the original's "base name" lookup.
    [UnmanagedCallersOnly(EntryPoint = "SpLoadBinaryResource")]
    public static int SpLoadBinaryResource(char* moduleBaseName, char* resourceName, int allowLoadAsCode, IntPtr* pBits, uint* size)
    {
        if (pBits == null || size == null)
            return 0;

        *pBits = IntPtr.Zero;
        *size = 0;

        Assembly assembly = FindAssembly(NativeString.UniToString(moduleBaseName));
        string name = NativeString.UniToString(resourceName);
        if (assembly == null || string.IsNullOrEmpty(name))
            return 0;

        using Stream stream = assembly.GetManifestResourceStream(name);
        if (stream == null)
            return 0;

        var bytes = new byte[stream.Length];
        int read = stream.ReadAtLeast(bytes, bytes.Length, throwOnEndOfStream: false);

        IntPtr buffer = Marshal.AllocHGlobal(read);
        Marshal.Copy(bytes, 0, buffer, read);
        *pBits = buffer;
        *size = (uint)read;
        return 1;
    }

    // Real, backend-agnostic: reads the embedded font resource and registers its bytes
    // with the text engine's FontStore (which the CPU StbTrueType backend measures and
    // rasterizes with), keyed by the resource's base name. No OS font registration
    // (AddFontMemResourceEx / fontconfig) is involved -- the font lives entirely in the
    // process for this reimplementation's own text rendering. See logs/UIXrender/Rendering.md.
    [UnmanagedCallersOnly(EntryPoint = "SpLoadFontResource")]
    public static int SpLoadFontResource(char* moduleBaseName, char* resourceName)
    {
        Assembly assembly = FindAssembly(NativeString.UniToString(moduleBaseName));
        string name = NativeString.UniToString(resourceName);
        if (assembly == null || string.IsNullOrEmpty(name))
            return 0;

        using Stream stream = assembly.GetManifestResourceStream(name);
        if (stream == null)
            return 0;

        var bytes = new byte[stream.Length];
        stream.ReadExactly(bytes);

        string family = Path.GetFileNameWithoutExtension(name);
        return Subsystems.Text.FontStore.Register(family, bytes) ? 1 : 0;
    }

    private static Assembly FindAssembly(string baseName)
    {
        if (string.IsNullOrEmpty(baseName))
            return null;

        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (string.Equals(assembly.GetName().Name, baseName, StringComparison.OrdinalIgnoreCase))
                return assembly;
        }
        return null;
    }
}
