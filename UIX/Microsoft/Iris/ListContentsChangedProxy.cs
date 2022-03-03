// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ListContentsChangedProxy
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using System;
using System.Collections;

namespace Microsoft.Iris
{
    internal class ListContentsChangedProxy
    {
        private ListContentsChangedHandler _method;

        public static UIListContentsChangedHandler Thunk(
          ListContentsChangedHandler method)
        {
            return new UIListContentsChangedHandler(new ListContentsChangedProxy()
            {
                _method = method
            }.Thunk);
        }

        private void Thunk(IList senderList, UIListContentsChangedArgs internalArgs)
        {
            ListContentsChangeType type;
            switch (internalArgs.Type)
            {
                case UIListContentsChangeType.Add:
                    type = ListContentsChangeType.Add;
                    break;
                case UIListContentsChangeType.AddRange:
                    type = ListContentsChangeType.AddRange;
                    break;
                case UIListContentsChangeType.Remove:
                    type = ListContentsChangeType.Remove;
                    break;
                case UIListContentsChangeType.Move:
                    type = ListContentsChangeType.Move;
                    break;
                case UIListContentsChangeType.Insert:
                    type = ListContentsChangeType.Insert;
                    break;
                case UIListContentsChangeType.InsertRange:
                    type = ListContentsChangeType.InsertRange;
                    break;
                case UIListContentsChangeType.Clear:
                    type = ListContentsChangeType.Clear;
                    break;
                case UIListContentsChangeType.Modified:
                    type = ListContentsChangeType.Modified;
                    break;
                case UIListContentsChangeType.Reset:
                    type = ListContentsChangeType.Reset;
                    break;
                default:
                    throw new ArgumentException("Unexpected ListContentsChangeType");
            }
            ListContentsChangedArgs args = new ListContentsChangedArgs(type, internalArgs.OldIndex, internalArgs.NewIndex, internalArgs.Count);
            _method(senderList, args);
        }
    }
}
