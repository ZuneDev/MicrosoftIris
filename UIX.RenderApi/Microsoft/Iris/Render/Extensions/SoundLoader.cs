// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Extensions.SoundLoader
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Internal;
using Microsoft.Iris.Render.Protocol;
using System;

namespace Microsoft.Iris.Render.Extensions
{
    public static class SoundLoader
    {
        public static void FromResource(
          string moduleName,
          string resourceId,
          out ExtensionsApi.HSpSound soundDataHandle,
          out ExtensionsApi.SoundInformation soundDataInfo)
        {
            Win32Api.HINSTANCE hInstance = ModuleManager.Instance.LoadModule(moduleName);
            Debug2.Validate(hInstance != Win32Api.HINSTANCE.NULL, typeof(ArgumentException), nameof(moduleName));
            IntPtr resourceData;
            int resourceSize;
            ModuleManager.Instance.LoadResource(hInstance, resourceId, out resourceData, out resourceSize);
            FromMemory(resourceData, resourceSize, out soundDataHandle, out soundDataInfo);
        }

        public static void FromMemory(
          IntPtr rawSoundData,
          int rawSoundDataSize,
          out ExtensionsApi.HSpSound soundDataHandle,
          out ExtensionsApi.SoundInformation soundDataInfo)
        {
            Debug2.Validate(rawSoundData != IntPtr.Zero, typeof(ArgumentNullException), nameof(rawSoundData));
            Debug2.Validate(rawSoundDataSize > 0, typeof(ArgumentException), "Invalid sound data size");
            ExtensionsApi.SoundOptions options = ExtensionsApi.SoundOptions.Decode;
            ExtensionsApi.SoundInformation info = new ExtensionsApi.SoundInformation();
            ExtensionsApi.HSpSound hSound;
            EngineApi.IFC(ExtensionsApi.SpSoundLoadBuffer(rawSoundData, rawSoundDataSize, options, out hSound, out info));
            soundDataHandle = hSound;
            soundDataInfo = info;
        }

        public static void DisposeData(
          ExtensionsApi.HSpSound soundDataHandle,
          ExtensionsApi.SoundInformation soundDataInfo)
        {
            EngineApi.IFC(ExtensionsApi.SpSoundDispose(soundDataHandle, soundDataInfo));
        }
    }
}
