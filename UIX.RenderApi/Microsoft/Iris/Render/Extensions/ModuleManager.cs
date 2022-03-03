// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Extensions.ModuleManager
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Internal;
using System;

namespace Microsoft.Iris.Render.Extensions
{
    internal class ModuleManager
    {
        private static ModuleManager s_singleton;
        private Map<string, Win32Api.HINSTANCE> m_dictModules;

        ~ModuleManager()
        {
            if (this.m_dictModules == null)
                return;
            Map<string, Win32Api.HINSTANCE>.ValueCollection.Enumerator enumerator = this.m_dictModules.Values.GetEnumerator();
            while (enumerator.MoveNext())
                Win32Api.FreeLibrary(enumerator.Current);
            this.m_dictModules = null;
        }

        public static ModuleManager Instance
        {
            get
            {
                if (s_singleton == null)
                    s_singleton = new ModuleManager();
                return s_singleton;
            }
        }

        public Win32Api.HINSTANCE LoadModule(string stModuleName)
        {
            if (this.m_dictModules == null)
                this.m_dictModules = new Map<string, Win32Api.HINSTANCE>();
            Win32Api.HINSTANCE hinstance;
            if (this.m_dictModules.ContainsKey(stModuleName))
            {
                hinstance = this.m_dictModules[stModuleName];
            }
            else
            {
                hinstance = Win32Api.LoadLibraryEx(stModuleName, Win32Api.HANDLE.NULL, 2U);
                Debug2.Validate(hinstance != Win32Api.HINSTANCE.NULL, typeof(ArgumentException), "Failed to load module {0}", stModuleName);
                this.m_dictModules[stModuleName] = hinstance;
            }
            return hinstance;
        }

        public void LoadResource(
          Win32Api.HINSTANCE hInstance,
          string resourceId,
          out IntPtr resourceData,
          out int resourceSize)
        {
            IntPtr resource = Win32Api.FindResource(hInstance.h, resourceId, new IntPtr(10));
            Debug2.Validate(resource != IntPtr.Zero, typeof(ArgumentException), string.Format("Unable to find resource {0} in the module", resourceId));
            IntPtr i = Win32Api.LoadResource(hInstance.h, resource);
            Debug2.Validate(i != IntPtr.Zero, typeof(ArgumentException), string.Format("Unable to load resource {0} from the module", resourceId));
            IntPtr num1 = Win32Api.LockResource(i);
            Debug2.Validate(num1 != IntPtr.Zero, typeof(InvalidOperationException), "Failed to aquire pointer to resource data");
            int num2 = Win32Api.SizeofResource(hInstance.h, resource);
            resourceData = num1;
            resourceSize = num2;
        }
    }
}
