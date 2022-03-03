// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.CompilationMessage
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup
{
    internal static class CompilationMessage
    {
        public const string CanOnlyCompileMarkupResources = "'{0}' is not markup, it cannot be compiled";
        public const string RemoteResourceTargetsUnsupported = "'{0}' cannot be compiled since it is not local (it is remote and requires download)";
        public const string UnableToOpenFileForWrite = "'{0}' cannot be opened for writing";
        public const string ImportDependencyFailure = "Import of '{0}' failed";
        public const string ImportMemberFailure = "Import of {0} named '{1}' from '{2}' failed";
        public const string ImportVirtualMethodFailure = "Import of virtual method with index {0} from '{1}' failed";
        public const string UnableToOpenOutputFile = "Unable to open output file '{0}'.  Error code {1}";
        public const string UnableToSaveToOutputFile = "An error occurred while saving data to output file '{0}'.  Error code {1}";
        public const string MultipleImportNames = "Multiple names '{0}' and '{1}' used to refer to the same entity.  The first name will be used for all references to this item within compiled UIB files";
        public const string InvalidInputFileName = "'{0}' is not a valid filename";
    }
}
