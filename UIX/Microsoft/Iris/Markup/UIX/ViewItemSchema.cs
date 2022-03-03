// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.ViewItemSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Animations;
using Microsoft.Iris.Drawing;
using Microsoft.Iris.Layout;
using Microsoft.Iris.Library;
using Microsoft.Iris.Navigation;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;
using System;
using System.Collections;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class ViewItemSchema
    {
        public static UIXTypeSchema Type;

        private static object GetAlpha(object instanceObj) => ((ViewItem)instanceObj).Alpha;

        private static void SetAlpha(ref object instanceObj, object valueObj)
        {
            ViewItem viewItem = (ViewItem)instanceObj;
            float num = (float)valueObj;
            Result result = SingleSchema.Validate0to1(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                viewItem.Alpha = num;
        }

        private static object GetAnimations(object instanceObj) => ListProxy.GetAnimation((ViewItem)instanceObj);

        private static object GetCenterPointPercent(object instanceObj) => ((ViewItem)instanceObj).CenterPointPercent;

        private static void SetCenterPointPercent(ref object instanceObj, object valueObj) => ((ViewItem)instanceObj).CenterPointPercent = (Vector3)valueObj;

        private static object GetDebugOutline(object instanceObj) => ((ViewItem)instanceObj).DebugOutline;

        private static void SetDebugOutline(ref object instanceObj, object valueObj) => ((ViewItem)instanceObj).DebugOutline = (Color)valueObj;

        private static object GetFocusOrder(object instanceObj) => ((ViewItem)instanceObj).FocusOrder;

        private static void SetFocusOrder(ref object instanceObj, object valueObj)
        {
            ViewItem viewItem = (ViewItem)instanceObj;
            int num = (int)valueObj;
            Result result = Int32Schema.ValidateNotNegative(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                viewItem.FocusOrder = num;
        }

        private static object GetAlignment(object instanceObj) => ((ViewItem)instanceObj).Alignment;

        private static void SetAlignment(ref object instanceObj, object valueObj) => ((ViewItem)instanceObj).Alignment = (ItemAlignment)valueObj;

        private static object GetChildAlignment(object instanceObj) => ((ViewItem)instanceObj).ChildAlignment;

        private static void SetChildAlignment(ref object instanceObj, object valueObj) => ((ViewItem)instanceObj).ChildAlignment = (ItemAlignment)valueObj;

        private static object GetLayout(object instanceObj) => ((ViewItem)instanceObj).Layout;

        private static void SetLayout(ref object instanceObj, object valueObj) => ((ViewItem)instanceObj).Layout = (ILayout)valueObj;

        private static void SetLayoutInput(ref object instanceObj, object valueObj)
        {
            ViewItem viewItem = (ViewItem)instanceObj;
            ILayoutInput layoutInput = (ILayoutInput)valueObj;
            if (layoutInput == null)
                ErrorManager.ReportError("Script runtime failure: Invalid 'null' value for '{0}'", "LayoutInput");
            else
                viewItem.LayoutInput = layoutInput;
        }

        private static object GetLayoutOutput(object instanceObj) => ((ViewItem)instanceObj).LayoutOutput;

        private static object GetMargins(object instanceObj) => ((ViewItem)instanceObj).Margins;

        private static void SetMargins(ref object instanceObj, object valueObj) => ((ViewItem)instanceObj).Margins = (Inset)valueObj;

        private static object GetMaximumSize(object instanceObj) => ((ViewItem)instanceObj).MaximumSizeObject;

        private static void SetMaximumSize(ref object instanceObj, object valueObj)
        {
            ViewItem viewItem = (ViewItem)instanceObj;
            Result result = SizeSchema.ValidateNotNegative(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                viewItem.MaximumSizeObject = valueObj;
        }

        private static object GetMinimumSize(object instanceObj) => ((ViewItem)instanceObj).MinimumSizeObject;

        private static void SetMinimumSize(ref object instanceObj, object valueObj)
        {
            ViewItem viewItem = (ViewItem)instanceObj;
            Result result = SizeSchema.ValidateNotNegative(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                viewItem.MinimumSizeObject = valueObj;
        }

        private static object GetMouseInteractive(object instanceObj) => BooleanBoxes.Box(((ViewItem)instanceObj).MouseInteractive);

        private static void SetMouseInteractive(ref object instanceObj, object valueObj) => ((ViewItem)instanceObj).MouseInteractive = (bool)valueObj;

        private static object GetName(object instanceObj) => ((ViewItem)instanceObj).Name;

        private static void SetName(ref object instanceObj, object valueObj) => ((ViewItem)instanceObj).Name = (string)valueObj;

        private static object GetNavigation(object instanceObj) => ((ViewItem)instanceObj).Navigation;

        private static void SetNavigation(ref object instanceObj, object valueObj) => ((ViewItem)instanceObj).Navigation = (NavigationPolicies)valueObj;

        private static object GetPadding(object instanceObj) => ((ViewItem)instanceObj).Padding;

        private static void SetPadding(ref object instanceObj, object valueObj) => ((ViewItem)instanceObj).Padding = (Inset)valueObj;

        private static object GetRotation(object instanceObj) => ((ViewItem)instanceObj).Rotation;

        private static void SetRotation(ref object instanceObj, object valueObj) => ((ViewItem)instanceObj).Rotation = (Rotation)valueObj;

        private static object GetScale(object instanceObj) => ((ViewItem)instanceObj).Scale;

        private static void SetScale(ref object instanceObj, object valueObj) => ((ViewItem)instanceObj).Scale = (Vector3)valueObj;

        private static object GetSharedSize(object instanceObj) => ((ViewItem)instanceObj).SharedSize;

        private static void SetSharedSize(ref object instanceObj, object valueObj) => ((ViewItem)instanceObj).SharedSize = (SharedSize)valueObj;

        private static object GetSharedSizePolicy(object instanceObj) => ((ViewItem)instanceObj).SharedSizePolicy;

        private static void SetSharedSizePolicy(ref object instanceObj, object valueObj) => ((ViewItem)instanceObj).SharedSizePolicy = (SharedSizePolicy)valueObj;

        private static object GetVisible(object instanceObj) => BooleanBoxes.Box(((ViewItem)instanceObj).Visible);

        private static void SetVisible(ref object instanceObj, object valueObj) => ((ViewItem)instanceObj).Visible = (bool)valueObj;

        private static object GetBackground(object instanceObj) => ((ViewItem)instanceObj).Background;

        private static void SetBackground(ref object instanceObj, object valueObj) => ((ViewItem)instanceObj).Background = (Color)valueObj;

        private static object GetCamera(object instanceObj) => ((ViewItem)instanceObj).Camera;

        private static void SetCamera(ref object instanceObj, object valueObj) => ((ViewItem)instanceObj).Camera = (Camera)valueObj;

        private static object CallAttachAnimationIAnimation(object instanceObj, object[] parameters)
        {
            ViewItem viewItem = (ViewItem)instanceObj;
            IAnimationProvider parameter = (IAnimationProvider)parameters[0];
            if (parameter == null)
            {
                ErrorManager.ReportError("Script runtime failure: Invalid 'null' value for '{0}'", "animation");
                return null;
            }
            viewItem.AttachAnimation(parameter);
            return null;
        }

        private static object CallAttachAnimationIAnimationAnimationHandle(
          object instanceObj,
          object[] parameters)
        {
            ViewItem viewItem = (ViewItem)instanceObj;
            IAnimationProvider parameter1 = (IAnimationProvider)parameters[0];
            AnimationHandle parameter2 = (AnimationHandle)parameters[1];
            if (parameter1 == null)
            {
                ErrorManager.ReportError("Script runtime failure: Invalid 'null' value for '{0}'", "animation");
                return null;
            }
            if (parameter2 == null)
            {
                ErrorManager.ReportError("Script runtime failure: Invalid 'null' value for '{0}'", "handle");
                return null;
            }
            viewItem.AttachAnimation(parameter1, parameter2);
            return null;
        }

        private static object CallDetachAnimationAnimationEventType(
          object instanceObj,
          object[] parameters)
        {
            ((ViewItem)instanceObj).DetachAnimation((AnimationEventType)parameters[0]);
            return null;
        }

        private static object CallPlayAnimationIAnimation(object instanceObj, object[] parameters)
        {
            ViewItem viewItem = (ViewItem)instanceObj;
            IAnimationProvider parameter = (IAnimationProvider)parameters[0];
            if (parameter == null)
            {
                ErrorManager.ReportError("Script runtime failure: Invalid 'null' value for '{0}'", "animation");
                return null;
            }
            viewItem.PlayAnimation(parameter, null);
            return null;
        }

        private static object CallPlayAnimationIAnimationAnimationHandle(
          object instanceObj,
          object[] parameters)
        {
            ViewItem viewItem = (ViewItem)instanceObj;
            IAnimationProvider parameter1 = (IAnimationProvider)parameters[0];
            AnimationHandle parameter2 = (AnimationHandle)parameters[1];
            if (parameter1 == null)
            {
                ErrorManager.ReportError("Script runtime failure: Invalid 'null' value for '{0}'", "animation");
                return null;
            }
            if (parameter2 == null)
            {
                ErrorManager.ReportError("Script runtime failure: Invalid 'null' value for '{0}'", "handle");
                return null;
            }
            viewItem.PlayAnimation(parameter1, parameter2);
            return null;
        }

        private static object CallPlayAnimationAnimationEventType(
          object instanceObj,
          object[] parameters)
        {
            ((ViewItem)instanceObj).PlayAnimation((AnimationEventType)parameters[0]);
            return null;
        }

        private static object CallForceContentChange(object instanceObj, object[] parameters)
        {
            ((ViewItem)instanceObj).ForceContentChange();
            return null;
        }

        private static object CallSnapshotPosition(object instanceObj, object[] parameters) => ((ViewItem)instanceObj).SnapshotPosition();

        private static object CallNavigateInto(object instanceObj, object[] parameters)
        {
            ((ViewItem)instanceObj).NavigateInto();
            return null;
        }

        private static object CallNavigateIntoBoolean(object instanceObj, object[] parameters)
        {
            ((ViewItem)instanceObj).NavigateInto((bool)parameters[0]);
            return null;
        }

        private static object CallScrollIntoView(object instanceObj, object[] parameters)
        {
            ((ViewItem)instanceObj).ScrollIntoView();
            return null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(239, "ViewItem", null, -1, typeof(ViewItem), UIXTypeFlags.Disposable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(239, "Alpha", 194, -1, ExpressionRestriction.None, false, SingleSchema.Validate0to1, true, new GetValueHandler(GetAlpha), new SetValueHandler(SetAlpha), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(239, "Animations", 138, 104, ExpressionRestriction.NoAccess, false, null, true, new GetValueHandler(GetAnimations), null, false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(239, "CenterPointPercent", 234, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetCenterPointPercent), new SetValueHandler(SetCenterPointPercent), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(239, "DebugOutline", 35, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetDebugOutline), new SetValueHandler(SetDebugOutline), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(239, "FocusOrder", 115, -1, ExpressionRestriction.None, false, Int32Schema.ValidateNotNegative, true, new GetValueHandler(GetFocusOrder), new SetValueHandler(SetFocusOrder), false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(239, "Alignment", sbyte.MaxValue, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetAlignment), new SetValueHandler(SetAlignment), false);
            UIXPropertySchema uixPropertySchema7 = new UIXPropertySchema(239, "ChildAlignment", sbyte.MaxValue, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetChildAlignment), new SetValueHandler(SetChildAlignment), false);
            UIXPropertySchema uixPropertySchema8 = new UIXPropertySchema(239, "Layout", 132, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetLayout), new SetValueHandler(SetLayout), false);
            UIXPropertySchema uixPropertySchema9 = new UIXPropertySchema(239, "LayoutInput", 133, -1, ExpressionRestriction.None, false, null, true, null, new SetValueHandler(SetLayoutInput), false);
            UIXPropertySchema uixPropertySchema10 = new UIXPropertySchema(239, "LayoutOutput", 134, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetLayoutOutput), null, false);
            UIXPropertySchema uixPropertySchema11 = new UIXPropertySchema(239, "Margins", 114, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetMargins), new SetValueHandler(SetMargins), false);
            UIXPropertySchema uixPropertySchema12 = new UIXPropertySchema(239, "MaximumSize", 195, -1, ExpressionRestriction.None, false, SizeSchema.ValidateNotNegative, true, new GetValueHandler(GetMaximumSize), new SetValueHandler(SetMaximumSize), false);
            UIXPropertySchema uixPropertySchema13 = new UIXPropertySchema(239, "MinimumSize", 195, -1, ExpressionRestriction.None, false, SizeSchema.ValidateNotNegative, true, new GetValueHandler(GetMinimumSize), new SetValueHandler(SetMinimumSize), false);
            UIXPropertySchema uixPropertySchema14 = new UIXPropertySchema(239, "MouseInteractive", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetMouseInteractive), new SetValueHandler(SetMouseInteractive), false);
            UIXPropertySchema uixPropertySchema15 = new UIXPropertySchema(239, "Name", 208, -1, ExpressionRestriction.ReadOnly, false, null, true, new GetValueHandler(GetName), new SetValueHandler(SetName), false);
            UIXPropertySchema uixPropertySchema16 = new UIXPropertySchema(239, "Navigation", 151, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetNavigation), new SetValueHandler(SetNavigation), false);
            UIXPropertySchema uixPropertySchema17 = new UIXPropertySchema(239, "Padding", 114, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetPadding), new SetValueHandler(SetPadding), false);
            UIXPropertySchema uixPropertySchema18 = new UIXPropertySchema(239, "Rotation", 176, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetRotation), new SetValueHandler(SetRotation), false);
            UIXPropertySchema uixPropertySchema19 = new UIXPropertySchema(239, "Scale", 234, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetScale), new SetValueHandler(SetScale), false);
            UIXPropertySchema uixPropertySchema20 = new UIXPropertySchema(239, "SharedSize", 190, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetSharedSize), new SetValueHandler(SetSharedSize), false);
            UIXPropertySchema uixPropertySchema21 = new UIXPropertySchema(239, "SharedSizePolicy", 191, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetSharedSizePolicy), new SetValueHandler(SetSharedSizePolicy), false);
            UIXPropertySchema uixPropertySchema22 = new UIXPropertySchema(239, "Visible", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetVisible), new SetValueHandler(SetVisible), false);
            UIXPropertySchema uixPropertySchema23 = new UIXPropertySchema(239, "Background", 35, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetBackground), new SetValueHandler(SetBackground), false);
            UIXPropertySchema uixPropertySchema24 = new UIXPropertySchema(239, "Camera", 21, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetCamera), new SetValueHandler(SetCamera), false);
            UIXMethodSchema uixMethodSchema1 = new UIXMethodSchema(239, "AttachAnimation", new short[1]
            {
         104
            }, 240, new InvokeHandler(CallAttachAnimationIAnimation), false);
            UIXMethodSchema uixMethodSchema2 = new UIXMethodSchema(239, "AttachAnimation", new short[2]
            {
         104,
         11
            }, 240, new InvokeHandler(CallAttachAnimationIAnimationAnimationHandle), false);
            UIXMethodSchema uixMethodSchema3 = new UIXMethodSchema(239, "DetachAnimation", new short[1]
            {
         10
            }, 240, new InvokeHandler(CallDetachAnimationAnimationEventType), false);
            UIXMethodSchema uixMethodSchema4 = new UIXMethodSchema(239, "PlayAnimation", new short[1]
            {
         104
            }, 240, new InvokeHandler(CallPlayAnimationIAnimation), false);
            UIXMethodSchema uixMethodSchema5 = new UIXMethodSchema(239, "PlayAnimation", new short[2]
            {
         104,
         11
            }, 240, new InvokeHandler(CallPlayAnimationIAnimationAnimationHandle), false);
            UIXMethodSchema uixMethodSchema6 = new UIXMethodSchema(239, "PlayAnimation", new short[1]
            {
         10
            }, 240, new InvokeHandler(CallPlayAnimationAnimationEventType), false);
            UIXMethodSchema uixMethodSchema7 = new UIXMethodSchema(239, "ForceContentChange", null, 240, new InvokeHandler(CallForceContentChange), false);
            UIXMethodSchema uixMethodSchema8 = new UIXMethodSchema(239, "SnapshotPosition", null, 171, new InvokeHandler(CallSnapshotPosition), false);
            UIXMethodSchema uixMethodSchema9 = new UIXMethodSchema(239, "NavigateInto", null, 240, new InvokeHandler(CallNavigateInto), false);
            UIXMethodSchema uixMethodSchema10 = new UIXMethodSchema(239, "NavigateInto", new short[1]
            {
         15
            }, 240, new InvokeHandler(CallNavigateIntoBoolean), false);
            UIXMethodSchema uixMethodSchema11 = new UIXMethodSchema(239, "ScrollIntoView", null, 240, new InvokeHandler(CallScrollIntoView), false);
            Type.Initialize(null, null, new PropertySchema[24]
            {
         uixPropertySchema6,
         uixPropertySchema1,
         uixPropertySchema2,
         uixPropertySchema23,
         uixPropertySchema24,
         uixPropertySchema3,
         uixPropertySchema7,
         uixPropertySchema4,
         uixPropertySchema5,
         uixPropertySchema8,
         uixPropertySchema9,
         uixPropertySchema10,
         uixPropertySchema11,
         uixPropertySchema12,
         uixPropertySchema13,
         uixPropertySchema14,
         uixPropertySchema15,
         uixPropertySchema16,
         uixPropertySchema17,
         uixPropertySchema18,
         uixPropertySchema19,
         uixPropertySchema20,
         uixPropertySchema21,
         uixPropertySchema22
            }, new MethodSchema[11]
            {
         uixMethodSchema1,
         uixMethodSchema2,
         uixMethodSchema3,
         uixMethodSchema4,
         uixMethodSchema5,
         uixMethodSchema6,
         uixMethodSchema7,
         uixMethodSchema8,
         uixMethodSchema9,
         uixMethodSchema10,
         uixMethodSchema11
            }, null, null, null, null, null, null, null, null);
        }

        internal class ListProxy : IList, ICollection, IEnumerable
        {
            private ViewItem _subject;
            private ViewItemSchema.ListProxyMode _mode;
            private static ViewItemSchema.ListProxy s_shared = new ViewItemSchema.ListProxy();

            public static IList GetChildren(ViewItem subject)
            {
                s_shared.SetSubject(subject, ListProxyMode.Children);
                return s_shared;
            }

            public static IList GetAnimation(ViewItem subject)
            {
                s_shared.SetSubject(subject, ListProxyMode.Animation);
                return s_shared;
            }

            public int Add(object value)
            {
                switch (_mode)
                {
                    case ListProxyMode.Children:
                        _subject.Children.Add((Microsoft.Iris.Library.TreeNode)value);
                        break;
                    case ListProxyMode.Animation:
                        if (value != null)
                        {
                            _subject.AttachAnimation((IAnimationProvider)value);
                            break;
                        }
                        break;
                }
                _subject = null;
                return 0;
            }

            public void Clear() => throw new NotImplementedException();

            public bool Contains(object value) => throw new NotImplementedException();

            public int IndexOf(object value) => throw new NotImplementedException();

            public void Insert(int index, object value) => throw new NotImplementedException();

            public void Remove(object value) => throw new NotImplementedException();

            public void RemoveAt(int index) => throw new NotImplementedException();

            public bool IsFixedSize => throw new NotImplementedException();

            public bool IsReadOnly => throw new NotImplementedException();

            public object this[int index]
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            public void CopyTo(Array array, int index) => throw new NotImplementedException();

            public int Count => throw new NotImplementedException();

            public bool IsSynchronized => throw new NotImplementedException();

            public object SyncRoot => throw new NotImplementedException();

            public IEnumerator GetEnumerator() => throw new NotImplementedException();

            private void SetSubject(ViewItem subject, ViewItemSchema.ListProxyMode mode)
            {
                _subject = subject;
                _mode = mode;
            }

            private ListProxy()
            {
            }
        }

        internal enum ListProxyMode
        {
            Children,
            Animation,
        }
    }
}
