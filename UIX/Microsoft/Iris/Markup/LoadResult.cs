// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.LoadResult
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Session;
using System;

namespace Microsoft.Iris.Markup
{
    public abstract class LoadResult : SharedDisposableObject
    {
        private string _uri;
        protected string _uriUnderlying;
        private uint _islandReferences;
        private string _compilerReferenceName;
        public static LoadResult[] EmptyList = new LoadResult[0];

        public LoadResult(string uri) => _uri = uri;

        protected override void OnDispose() => base.OnDispose();

        public string Uri => _uri;

        public string UnderlyingUri => _uriUnderlying;

        public string ErrorContextUri => _uriUnderlying == null ? _uri : _uriUnderlying;

        public uint IslandReferences => _islandReferences;

        public void AddReference(uint islandId)
        {
            if (((int)_islandReferences & (int)islandId) != 0)
                return;
            if (_islandReferences == 0U)
                RegisterUsage(this);
            _islandReferences |= islandId;
            foreach (LoadResult dependency in Dependencies)
            {
                if (dependency != this)
                    dependency.AddReference(islandId);
            }
        }

        public void RemoveReferenceDeep(uint islandId)
        {
            if ((_islandReferences & islandId) <= 0U)
                return;
            _islandReferences &= ~islandId;
            foreach (LoadResult dependency in Dependencies)
            {
                if (dependency != this)
                    dependency.RemoveReferenceDeep(islandId);
            }
            if (_islandReferences != 0U)
                return;
            UnregisterUsage(this);
            foreach (LoadResult dependency in Dependencies)
            {
                if (dependency != this)
                    dependency.UnregisterUsage(this);
            }
        }

        public void RemoveAllReferences() => RemoveReferenceDeep(IslandReferences);

        protected void RegisterDependenciesUsage()
        {
            foreach (LoadResult dependency in Dependencies)
            {
                if (dependency != this)
                {
                    dependency.RegisterUsage(this);
                    dependency.AddReference(_islandReferences);
                }
            }
        }

        public void RegisterProxyUsage() => RegisterUsage(this);

        public void UnregisterProxyUsage() => UnregisterUsage(this);

        public virtual void Load(LoadPass pass)
        {
        }

        public abstract TypeSchema FindType(string name);

        public abstract LoadResultStatus Status { get; }

        public virtual LoadResult[] Dependencies => EmptyList;

        public virtual TypeSchema[] ExportTable => TypeSchema.EmptyList;

        public virtual bool Cachable => true;

        public void SetCompilerReferenceName(string name)
        {
            if (_compilerReferenceName == null)
            {
                _compilerReferenceName = name;
            }
            else
            {
                if (name.Equals(_compilerReferenceName, StringComparison.OrdinalIgnoreCase))
                    return;
                ErrorManager.ReportWarning("Multiple names '{0}' and '{1}' used to refer to the same entity.  The first name will be used for all references to this item within compiled UIB files", _compilerReferenceName, name);
            }
        }

        public virtual string GetCompilerReferenceName() => _compilerReferenceName;

        public override string ToString() => _uri;
    }
}
