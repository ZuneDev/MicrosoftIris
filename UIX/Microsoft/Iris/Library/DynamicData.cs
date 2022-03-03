// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Library.DynamicData
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris.Library
{
    internal struct DynamicData
    {
        private SmartMap _dataMap;

        public void Create() => _dataMap = new SmartMap();

        public object GetData(DataCookie cookie) => _dataMap[GetKey(cookie)];

        public void SetData(DataCookie cookie, object value) => _dataMap[GetKey(cookie)] = value;

        public Delegate GetEventHandler(EventCookie cookie) => _dataMap[GetKey(cookie)] as Delegate;

        public bool AddEventHandler(EventCookie cookie, Delegate handlerToAdd)
        {
            uint key = GetKey(cookie);
            Delegate data = _dataMap[key] as Delegate;
            _dataMap[key] = Delegate.Combine(data, handlerToAdd);
            return (object)data == null;
        }

        public bool RemoveEventHandler(EventCookie cookie, Delegate handlerToRemove)
        {
            uint key = GetKey(cookie);
            Delegate @delegate = Delegate.Remove(_dataMap[key] as Delegate, handlerToRemove);
            _dataMap[key] = @delegate;
            return (object)@delegate == null;
        }

        public void RemoveEventHandlers(EventCookie cookie) => _dataMap[GetKey(cookie)] = null;

        private static uint GetKey(DataCookie cookie) => DataCookie.ToUInt32(cookie);

        private static uint GetKey(EventCookie cookie) => EventCookie.ToUInt32(cookie);
    }
}
