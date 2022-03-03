// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.OS.FileResources
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;

namespace Microsoft.Iris.OS
{
    internal class FileResources : IResourceProvider
    {
        private static FileResources s_instance = new FileResources();

        public static FileResources Instance => s_instance;

        public Resource GetResource(string hierarchicalPart, string uri, bool forceSynchronous) => new FileResource(uri, hierarchicalPart, forceSynchronous);
    }
}
