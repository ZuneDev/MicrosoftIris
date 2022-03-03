// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Input.DragDropInfo
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Input
{
    internal class DragDropInfo : MouseInfo
    {
        private static InputInfo.InfoType s_poolType = InfoType.DragDrop;
        private InputModifiers _modifiers;
        private DragOperation _operation;

        static DragDropInfo() => SetPoolLimitMode(s_poolType, false);

        private DragDropInfo()
        {
        }

        public static DragDropInfo Create(
          IRawInputSite rawSource,
          int x,
          int y,
          InputModifiers modifiers,
          DragOperation operation)
        {
            DragDropInfo dragDropInfo = (DragDropInfo)GetFromPool(s_poolType) ?? new DragDropInfo();
            dragDropInfo.Initialize(rawSource, x, y, modifiers, operation);
            return dragDropInfo;
        }

        public void Initialize(
          IRawInputSite rawSource,
          int x,
          int y,
          InputModifiers modifiers,
          DragOperation operation)
        {
            _modifiers = modifiers;
            _operation = operation;
            Initialize(rawSource, x, y, EventTypeForDragOperation(operation));
        }

        protected override InputInfo.InfoType PoolType => s_poolType;

        public InputModifiers Modifiers => _modifiers;

        public DragOperation Operation => _operation;

        private static InputEventType EventTypeForDragOperation(DragOperation operation)
        {
            switch (operation)
            {
                case DragOperation.Enter:
                    return InputEventType.DragEnter;
                case DragOperation.Over:
                    return InputEventType.DragOver;
                case DragOperation.Leave:
                    return InputEventType.DragLeave;
                case DragOperation.Drop:
                    return InputEventType.DragDropped;
                case DragOperation.DragComplete:
                    return InputEventType.DragComplete;
                default:
                    return InputEventType.Invalid;
            }
        }
    }
}
