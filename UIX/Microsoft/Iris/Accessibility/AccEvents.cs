// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Accessibility.AccEvents
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Accessibility
{
    internal enum AccEvents
    {
        None = 0,
        SystemSound = 1,
        SystemAlert = 2,
        SystemForeground = 3,
        SystemMenuStart = 4,
        SystemMenuEnd = 5,
        SystemMenuPopupStart = 6,
        SystemMenuPopupEnd = 7,
        SystemCaptureStart = 8,
        SystemCaptureEnd = 9,
        SystemMoveSizeStart = 10, // 0x0000000A
        SystemMoveSizeEnd = 11, // 0x0000000B
        SystemContextHelpStart = 12, // 0x0000000C
        SystemContextHelpEnd = 13, // 0x0000000D
        SystemDragDropStart = 14, // 0x0000000E
        SystemDragDropEnd = 15, // 0x0000000F
        SystemDialogStart = 16, // 0x00000010
        SystemDialogEnd = 17, // 0x00000011
        SystemScrollingStart = 18, // 0x00000012
        SystemScrollingEnd = 19, // 0x00000013
        SystemSwitchStart = 20, // 0x00000014
        SystemSwitchEnd = 21, // 0x00000015
        SystemMinimizeStart = 22, // 0x00000016
        SystemMinimizeEnd = 23, // 0x00000017
        ObjectCreate = 32768, // 0x00008000
        ObjectDestroy = 32769, // 0x00008001
        ObjectShow = 32770, // 0x00008002
        ObjectHide = 32771, // 0x00008003
        ObjectReorder = 32772, // 0x00008004
        ObjectFocus = 32773, // 0x00008005
        ObjectSelection = 32774, // 0x00008006
        ObjectSelectionAdd = 32775, // 0x00008007
        ObjectSelectionRemove = 32776, // 0x00008008
        ObjectSelectionWithin = 32777, // 0x00008009
        ObjectStateChange = 32778, // 0x0000800A
        ObjectLocationChange = 32779, // 0x0000800B
        ObjectNameChange = 32780, // 0x0000800C
        ObjectDescriptionChange = 32781, // 0x0000800D
        ObjectValueChange = 32782, // 0x0000800E
        ObjectParentChange = 32783, // 0x0000800F
        ObjectHelpChange = 32784, // 0x00008010
        ObjectDefaultActionChange = 32785, // 0x00008011
        ObjectAcceleratorChange = 32786, // 0x00008012
    }
}
