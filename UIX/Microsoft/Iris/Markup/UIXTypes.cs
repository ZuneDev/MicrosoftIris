// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIXTypes
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup
{
    public static class UIXTypes
    {
        public static TypeSchema MapIDToType(short ID) => ID != -1 ? UIXLoadResultExports.ExportTable[ID] : null;

        public static TypeSchema[] MapIDsToTypes(short[] IDs)
        {
            TypeSchema[] typeSchemaArray;
            if (IDs == null)
            {
                typeSchemaArray = TypeSchema.EmptyList;
            }
            else
            {
                typeSchemaArray = new TypeSchema[IDs.Length];
                for (int index = 0; index < IDs.Length; ++index)
                    typeSchemaArray[index] = UIXLoadResultExports.ExportTable[IDs[index]];
            }
            return typeSchemaArray;
        }

        public static void RegisterTypeForID(short ID, TypeSchema type) => UIXLoadResultExports.ExportTable[ID] = type;
    }
}
