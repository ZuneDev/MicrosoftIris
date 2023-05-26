// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.UI.RootLoadResult
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Iris.UI
{
    [Serializable]
    internal class RootLoadResult : MarkupLoadResult
    {
        public UIClassTypeSchema RootType;

        public RootLoadResult(string name)
          : base(name)
          => RootType = new UIClassTypeSchema(this, name);

        protected RootLoadResult(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            RootType = info.GetValue<UIClassTypeSchema>(nameof(RootType));
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            RootType.Dispose(this);
            RootType = null;
        }

        public override TypeSchema FindType(string name) => (TypeSchema)null;

        public override LoadResultStatus Status => LoadResultStatus.Success;

        public override bool IsSource => true;

        public override MarkupConstantsTable ConstantsTable => (MarkupConstantsTable)null;

        public override MarkupImportTables ImportTables => (MarkupImportTables)null;

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(RootType), RootType);
        }
    }
}
