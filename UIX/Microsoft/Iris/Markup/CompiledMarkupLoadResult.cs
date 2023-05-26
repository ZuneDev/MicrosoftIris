// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.CompiledMarkupLoadResult
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Session;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Iris.Markup
{
    [Serializable]
    internal class CompiledMarkupLoadResult : MarkupLoadResult
    {
        private Resource _resource;
        private CompiledMarkupLoader _loader;
        private IntPtr _addressOfLineNumberData;

        public CompiledMarkupLoadResult(Resource resource, string uri)
          : base(uri)
        {
            _resource = resource;
            if (uri != resource.Uri)
                _uriUnderlying = resource.Uri;
            _loader = CompiledMarkupLoader.Load(this, resource);
        }

        protected CompiledMarkupLoadResult(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override bool IsSource => false;

        public override MarkupConstantsTable ConstantsTable => _binaryDataTable.ConstantsTable;

        public override MarkupImportTables ImportTables => _binaryDataTable.ImportTables;

        public void SetAddressOfLineNumberData(IntPtr address) => _addressOfLineNumberData = address;

        public override MarkupLineNumberTable LineNumberTable
        {
            get
            {
                if (_lineNumberTable == null)
                    _lineNumberTable = CompiledMarkupLoader.DecodeLineNumberTable(_addressOfLineNumberData);
                return _lineNumberTable;
            }
        }

        public override void Load(LoadPass currentPass)
        {
            if (_loader == null)
                return;
            ErrorManager.EnterContext(ErrorContextUri);
            _loader.Depersist(currentPass);
            ErrorManager.ExitContext();
            if (currentPass != LoadPass.Done)
                return;
            LoadComplete();
        }

        private void LoadComplete()
        {
            if (_resource != null)
            {
                _resource.Free();
                _resource = null;
            }
            _loader = null;
            if (Status != LoadResultStatus.Loading)
                return;
            SetStatus(LoadResultStatus.Success);
        }
    }
}
