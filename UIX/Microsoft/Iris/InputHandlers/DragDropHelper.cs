// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.InputHandlers.DragDropHelper
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Input;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.InputHandlers
{
    internal static class DragDropHelper
    {
        private static bool _draggingInternally;
        private static DragSourceHandler _sourceHandler;
        private static DropTargetHandler _targetHandler;
        private static DropAction _dropAction;

        public static bool DraggingInternally => _draggingInternally;

        public static DragSourceHandler SourceHandler => _sourceHandler;

        public static DropTargetHandler TargetHandler
        {
            get => _targetHandler;
            set
            {
                if (_targetHandler == value)
                    return;
                DropTargetHandler targetHandler = _targetHandler;
                _targetHandler = value;
                OnAllowedDropActionsChanged();
            }
        }

        public static DropAction AllowedDropActions => _targetHandler == null ? DropAction.None : _targetHandler.AllowedDropActions;

        public static InputModifiers Modifiers => UISession.Default.InputManager.DragModifiers;

        public static void OnAllowedDropActionsChanged()
        {
            if (DraggingInternally)
            {
                _sourceHandler.UpdateCurrentAction();
            }
            else
            {
                uint allowedDropActions = (uint)AllowedDropActions;
                UISession.Default.Form.SetDragDropResult(allowedDropActions, allowedDropActions);
            }
        }

        public static void BeginDrag(
          DragSourceHandler sourceHandler,
          ICookedInputSite source,
          IRawInputSite target,
          int formX,
          int formY,
          InputModifiers modifiers)
        {
            _sourceHandler = sourceHandler;
            _draggingInternally = true;
            UISession.Default.Form.IsDragInProgress = true;
            UISession.Default.InputManager.SimulateDragEnter(source, target, _sourceHandler.Value, formX, formY, modifiers);
        }

        public static void Requery(InputModifiers modifiers)
        {
            UISession.Default.InputManager.SimulateDragOver(modifiers);
            _sourceHandler.UpdateCurrentAction();
        }

        public static void Requery(
          IRawInputSite target,
          int formX,
          int formY,
          InputModifiers modifiers)
        {
            UISession.Default.InputManager.SimulateDragOver(target, formX, formY, modifiers);
        }

        public static void EndDrag(IRawInputSite target, InputModifiers modifiers, DropAction action)
        {
            DragOperation formOperation = DragOperation.Drop;
            _dropAction = action;
            _draggingInternally = false;
            if (action == DropAction.None)
            {
                target = null;
                formOperation = DragOperation.Leave;
            }
            UISession.Default.InputManager.SimulateDragEnd(target, modifiers, formOperation);
            UISession.Default.Form.IsDragInProgress = false;
        }

        public static void OnDragComplete()
        {
            _sourceHandler.OnEndDrag(_dropAction);
            _sourceHandler = null;
            _dropAction = DropAction.None;
        }

        public static object GetValue() => UISession.Default.InputManager.GetDragDropValue();
    }
}
