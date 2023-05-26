// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.SourceMarkupLoadResult
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Markup.Validation;
using Microsoft.Iris.Session;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Iris.Markup
{
    [Serializable]
    internal class SourceMarkupLoadResult : MarkupLoadResult
    {
        private MarkupConstantsTable _constantsTable;
        private MarkupImportTables _importTables;
        private Resource _resource;
        private SourceMarkupLoader _loader;
        private bool _doneWithLoader;

        public SourceMarkupLoadResult(Resource resource, string uri)
          : base(uri)
        {
            _resource = resource;
            if (uri != resource.Uri)
                _uriUnderlying = resource.Uri;
            _loader = SourceMarkupLoader.Load(this, resource);
        }

        public SourceMarkupLoadResult(string uri)
          : base(uri)
        {
        }

        protected SourceMarkupLoadResult(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _constantsTable = info.GetValue<MarkupConstantsTable>(nameof(ConstantsTable));
            _importTables = info.GetValue<MarkupImportTables>(nameof(ImportTables));
        }

        public override bool IsSource => true;

        internal SourceMarkupLoader Loader => _loader;

        public override MarkupConstantsTable ConstantsTable => _constantsTable;

        public override MarkupImportTables ImportTables => _importTables;

        public override void Load(LoadPass currentPass)
        {
            if (_doneWithLoader)
                return;
            ErrorManager.EnterContext(ErrorContextUri);
            _loader.Validate(currentPass);
            ErrorManager.ExitContext();
            if (currentPass != LoadPass.Done)
                return;
            if (!MarkupSystem.TrackAdditionalMetadata)
                _loader = null;
            _doneWithLoader = true;
        }

        public void ValidateType(MarkupTypeSchema typeSchema, LoadPass currentPass)
        {
            ValidateClass loadData = (ValidateClass)typeSchema.LoadData;
            if (loadData == null)
                return;
            ErrorManager.EnterContext(ErrorContextUri);
            loadData.Validate(currentPass);
            ErrorManager.ExitContext();
        }

        public void ValidationComplete()
        {
            if (_resource != null)
            {
                _resource.Free();
                _resource = null;
            }
            if (Status == LoadResultStatus.Loading)
                SetStatus(LoadResultStatus.Success);
            foreach (MarkupTypeSchema markupTypeSchema in ExportTable)
                markupTypeSchema.Seal();
        }

        public void SetConstantsTable(MarkupConstantsTable constantsTable) => _constantsTable = constantsTable;

        public void SetImportTables(MarkupImportTables importTables) => _importTables = importTables;

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(_resource), _resource);
        }
    }
}
