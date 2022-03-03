// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIXLoadResult
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup.UIX;

namespace Microsoft.Iris.Markup
{
    internal class UIXLoadResult : LoadResult
    {
        public UIXLoadResult(string uri)
          : base(uri)
        {
        }

        public static void InitializeStatics() => UIXLoadResultExports.InitializeStatics();

        public override TypeSchema[] ExportTable => UIXLoadResultExports.ExportTable;

        private void SetExportTable(TypeSchema[] value) => UIXLoadResultExports.ExportTable = value;

        protected override void OnDispose()
        {
            base.OnDispose();
            foreach (DisposableObject disposableObject in ExportTable)
                disposableObject.Dispose(this);
            SetExportTable(null);
        }

        public override TypeSchema FindType(string name)
        {
            for (int index = 0; index < ExportTable.Length; ++index)
            {
                TypeSchema typeSchema = ExportTable[index];
                if (typeSchema.Name == name || typeSchema.AlternateName == name)
                    return typeSchema;
            }
            return null;
        }

        public override LoadResultStatus Status => LoadResultStatus.Success;

        public static Result ValidateStringAsValue(
          string inline_,
          TypeSchema propertyType,
          RangeValidator validator,
          out object value)
        {
            Result result = propertyType.TypeConverter(inline_, StringSchema.Type, out value);
            if (result.Failed || validator == null)
                return result;
            result = validator(value);
            return result;
        }
    }
}
