// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.ClassTypeSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup.UIX;
using Microsoft.Iris.UI;
using System;

namespace Microsoft.Iris.Markup
{
    internal class ClassTypeSchema : MarkupTypeSchema
    {
        private bool _isShared;
        private Class _sharedInstance;

        public ClassTypeSchema(MarkupLoadResult owner, string name)
          : base(owner, name)
        {
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            if (_sharedInstance == null)
                return;
            _sharedInstance.Dispose(this);
            _sharedInstance = null;
        }

        public override MarkupType MarkupType => MarkupType.Class;

        protected override TypeSchema DefaultBase => ObjectSchema.Type;

        public override Type RuntimeType => typeof(Class);

        public override object ConstructDefault() => _isShared ? SharedInstance : ConstructNewInstance();

        public override void InitializeInstance(ref object instance) => InitializeInstance((IMarkupTypeBase)instance);

        public override bool HasInitializer => !_isShared;

        public override bool Disposable => !_isShared && base.Disposable;

        public bool IsShared => _isShared;

        public void MarkShareable() => _isShared = true;

        public Class SharedInstance
        {
            get
            {
                if (!_isShared)
                    return null;
                if (_sharedInstance == null)
                {
                    _sharedInstance = ConstructNewInstance();
                    _sharedInstance.DeclareOwner(this);
                    InitializeInstance(_sharedInstance);
                }
                return _sharedInstance;
            }
        }

        protected virtual Class ConstructNewInstance() => new Class(this);
    }
}
