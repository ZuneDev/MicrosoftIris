// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Drawing.ResourceImageItem
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Render;
using Microsoft.Iris.Render.Extensions;

namespace Microsoft.Iris.Drawing
{
    internal class ResourceImageItem : ImageCacheItem
    {
        private string _source;
        private Resource _resource;
        private bool _acquireCalled;
        private ResourceAcquisitionCompleteHandler _resourceAcquisitionHandler;

        internal ResourceImageItem(
          IRenderSession renderSession,
          string source,
          Size maxSize,
          bool flippable,
          bool antialiasEdges)
          : this(renderSession, ResourceManager.Instance.GetResource(source), maxSize, source, flippable, antialiasEdges)
        {

        }

        internal ResourceImageItem(IRenderSession renderSession, Resource resource, Size maxSize, string identifier, bool flippable, bool antialiasEdges)
          : base(renderSession, identifier, maxSize, flippable, antialiasEdges)
        {
            _source = identifier;
            _resource = resource;
        }

        protected override void OnDispose()
        {
            if (_resource != null)
            {
                FreeResource();
                _resource = null;
            }
            base.OnDispose();
        }

        internal Resource Resource
        {
            get
            {
                EnsureResource();
                return _resource;
            }
        }

        internal ImageStatus Status
        {
            get
            {
                switch (_resource.Status)
                {
                    case ResourceStatus.NeedsAcquire:
                        return ImageStatus.PendingLoad;
                    case ResourceStatus.Acquiring:
                        return ImageStatus.Loading;
                    case ResourceStatus.Available:
                        return ImageStatus.Complete;
                    case ResourceStatus.Error:
                        return ImageStatus.Error;
                    default:
                        return ImageStatus.Error;
                }
            }
        }

        protected override bool EnsureBuffer()
        {
            if (EnsureResource())
            {
                if (IsResourceAvailable())
                    OnResourceLoadComplete(_resource);
                else if (!_acquireCalled)
                {
                    _resourceAcquisitionHandler = new ResourceAcquisitionCompleteHandler(OnResourceLoadComplete);
                    _resource.Acquire(_resourceAcquisitionHandler);
                    _acquireCalled = true;
                }
            }
            return false;
        }

        protected override void OnImageLoadComplete()
        {
            ImageStatus status = Status;
            if (LoadCompleteHandler == null)
                return;
            LoadCompleteHandler(this, status);
        }

        private bool IsResourceAvailable() => _resource.Status == ResourceStatus.Available;

        public bool EnsureResource()
        {
            if (_resource == null)
                _resource = ResourceManager.Instance.GetResource(_source);
            return _resource != null;
        }

        private void OnResourceLoadComplete(Resource resource)
        {
            if (resource != _resource)
                return;
            if (!IsSuccessfulResourceLoad(_resource) || Application.IsShuttingDown)
            {
                OnImageLoadComplete();
                FreeResource();
            }
            else
            {
                SetBuffer(_resource.Buffer, _resource.Length);
                if (_resource.Length <= 0U)
                    return;
                if (!ProcessBuffer())
                    _resource.Status = ResourceStatus.Error;
                OnImageLoadComplete();
            }
        }

        private static bool IsSuccessfulResourceLoad(Resource resource) => resource.Status != ResourceStatus.Error;

        private void FreeResource()
        {
            if (_acquireCalled)
            {
                _resource.Free(_resourceAcquisitionHandler);
                _resourceAcquisitionHandler = null;
            }
            _acquireCalled = false;
        }

        public override void StartLoad() => EnsureBuffer();

        public override string ToString() => _source;

        public override void ReleaseImage()
        {
            LoadCompleteHandler = null;
            base.ReleaseImage();
        }

        public override void RemoveData()
        {
            FreeResource();
            base.RemoveData();
        }

        public event ContentLoadCompleteHandler LoadCompleteHandler;
    }
}
