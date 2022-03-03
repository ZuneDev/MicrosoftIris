// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.MarkupDataTypeBaseObject
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.UI;
using System;

namespace Microsoft.Iris.Markup
{
    internal abstract class MarkupDataTypeBaseObject :
      Class,
      AssemblyObjectProxyHelper.IAssemblyProxyObject
    {
        protected MarkupDataTypeBaseObject(MarkupTypeSchema type)
          : base(type)
        {
        }

        public override object ReadSymbol(SymbolReference symbolRef)
        {
            object obj;
            return symbolRef.Origin == SymbolOrigin.Properties && ExternalObjectGetProperty(symbolRef.Symbol, out obj) ? obj : base.ReadSymbol(symbolRef);
        }

        public override void WriteSymbol(SymbolReference symbolRef, object value)
        {
            if (symbolRef.Origin == SymbolOrigin.Properties && ExternalObjectSetProperty(symbolRef.Symbol, value))
                return;
            base.WriteSymbol(symbolRef, value);
        }

        public override object GetProperty(string name)
        {
            object obj;
            return ExternalObjectGetProperty(name, out obj) ? obj : base.GetProperty(name);
        }

        public override void SetProperty(string name, object value)
        {
            if (ExternalObjectSetProperty(name, value))
                return;
            base.SetProperty(name, value);
        }

        public void FireNotificationThreadSafe(string property)
        {
            property = NotifyService.CanonicalizeString(property);
            _notifier.FireThreadSafe(property);
        }

        protected virtual bool ExternalObjectGetProperty(string propertyName, out object value)
        {
            value = null;
            return false;
        }

        protected virtual bool ExternalObjectSetProperty(string propertyName, object value) => false;

        protected abstract IDataProviderBaseObject ExternalAssemblyObject { get; }

        object AssemblyObjectProxyHelper.IAssemblyProxyObject.AssemblyObject => ExternalAssemblyObject;

        public abstract IntPtr ExternalNativeObject { get; }
    }
}
