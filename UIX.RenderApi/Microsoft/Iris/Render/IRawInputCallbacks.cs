// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.IRawInputCallbacks
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Input;

namespace Microsoft.Iris.Render
{
    public interface IRawInputCallbacks
    {
        InputHandlerFlags InputHandlerMask { get; }

        void HandleRawKeyboardInput(uint messageID, InputModifiers modifiers, ref RawKeyboardData args);

        void HandleRawMouseInput(uint messageID, InputModifiers modifiers, ref RawMouseData args);

        void HandleRawHidInput(ref RawHidData args);

        void HandleRawDragInput(uint messageID, InputModifiers modifiers, ref RawDragData args);

        void HandleAppCommand(ref RawHidData args);
    }
}
