// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.ParseResult
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup.Validation;

namespace Microsoft.Iris.Markup
{
    internal class ParseResult
    {
        public string Root;
        public string Version;
        public ValidateNamespace XmlnsList;
        public bool HasErrors;
        public Vector<ValidateClass> ClassList = new Vector<ValidateClass>(4);
        public Vector<ValidateAlias> AliasList = s_EmptyAliasList;
        public Vector<ValidateDataMapping> DataMappingList = s_EmptyDataMappingList;
        public static Vector<ValidateAlias> s_EmptyAliasList = new Vector<ValidateAlias>(0);
        public static Vector<ValidateDataMapping> s_EmptyDataMappingList = new Vector<ValidateDataMapping>(0);
    }
}
