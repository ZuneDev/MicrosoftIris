// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Internal.InputSystem
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Input;
using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Protocols.Splash.Desktop;
using System;

namespace Microsoft.Iris.Render.Internal
{
    internal sealed class InputSystem : RenderObject, IRenderHandleOwner, IInputSystem, IInputCallback
    {
        private const uint NG_KEYBOARD = 1;
        private const uint NG_HIDCOMMAND = 2;
        private const uint NG_APPCOMMAND = 3;
        private const uint NG_WINDOWINPUT = 4;
        private const uint NG_DRAGINPUT = 5;
        private static ushort[] s_ByteOrder_KeyboardCommandMessage;
        private static ushort[] s_ByteOrder_RemoteCallbackMessage;
        private static ushort[] s_ByteOrder_HIDCommandMessage;
        private static ushort[] s_ByteOrder_AppCommandMessage;
        private static ushort[] s_ByteOrder_WindowInputCallbackMessage;
        private static ushort[] s_ByteOrder_DragInputCallbackMessage;
        private RenderSession m_session;
        private RemoteInputRouter m_remoteInputRouter;
        private IRawInputCallbacks m_inputHandlers;
        private InputHandlerFlags m_maskInputHandlers;

        internal InputSystem(RenderSession session, RenderWindow window)
        {
            this.m_session = session;
            this.Initialize(window);
        }

        internal void Initialize(RenderWindow window) => this.m_remoteInputRouter = this.m_session.BuildRemoteInputRouter(this, window);

        protected override void Dispose(bool fInDispose)
        {
            try
            {
                if (fInDispose && this.m_remoteInputRouter != null)
                    this.m_remoteInputRouter.Dispose();
                this.m_remoteInputRouter = null;
            }
            finally
            {
                base.Dispose(fInDispose);
            }
        }

        RENDERHANDLE IRenderHandleOwner.RenderHandle => this.m_remoteInputRouter.RenderHandle;

        void IRenderHandleOwner.OnDisconnect() => this.m_remoteInputRouter = null;

        unsafe void IInputCallback.OnInput(RENDERHANDLE target, Message* inputInfoPtr)
        {
            InputSystem.RemoteCallbackMessage* remoteCallbackMessagePtr = (InputSystem.RemoteCallbackMessage*)inputInfoPtr;
            if (this.m_session.IsForeignByteOrderOnWindowing)
                MarshalHelper.SwapByteOrder((byte*)remoteCallbackMessagePtr, ref s_ByteOrder_RemoteCallbackMessage, typeof(InputSystem.RemoteCallbackMessage), 0, 0);
            switch (remoteCallbackMessagePtr->ownerMessage)
            {
                case 1:
                    if (this.m_session.IsForeignByteOrderOnWindowing)
                        MarshalHelper.SwapByteOrder((byte*)remoteCallbackMessagePtr, ref s_ByteOrder_KeyboardCommandMessage, typeof(InputSystem.KeyboardCommandMessage), sizeof(InputSystem.RemoteCallbackMessage), 0);
                    this.HandleKeyboardInput((InputSystem.KeyboardCommandMessage*)remoteCallbackMessagePtr);
                    break;
                case 2:
                    if (this.m_session.IsForeignByteOrderOnWindowing)
                        MarshalHelper.SwapByteOrder((byte*)remoteCallbackMessagePtr, ref s_ByteOrder_HIDCommandMessage, typeof(InputSystem.HIDCommandMessage), sizeof(InputSystem.RemoteCallbackMessage), 0);
                    this.HandleHIDCommandInput((InputSystem.HIDCommandMessage*)remoteCallbackMessagePtr);
                    break;
                case 3:
                    if (this.m_session.IsForeignByteOrderOnWindowing)
                        MarshalHelper.SwapByteOrder((byte*)remoteCallbackMessagePtr, ref s_ByteOrder_AppCommandMessage, typeof(InputSystem.AppCommandMessage), sizeof(InputSystem.RemoteCallbackMessage), 0);
                    this.HandleAppCommandInput((InputSystem.AppCommandMessage*)remoteCallbackMessagePtr);
                    break;
                case 4:
                    if (this.m_session.IsForeignByteOrderOnWindowing)
                        MarshalHelper.SwapByteOrder((byte*)remoteCallbackMessagePtr, ref s_ByteOrder_WindowInputCallbackMessage, typeof(InputSystem.WindowInputCallbackMessage), sizeof(InputSystem.RemoteCallbackMessage), 0);
                    this.HandleWindowInput((InputSystem.WindowInputCallbackMessage*)remoteCallbackMessagePtr);
                    break;
                case 5:
                    if (this.m_session.IsForeignByteOrderOnWindowing)
                        MarshalHelper.SwapByteOrder((byte*)remoteCallbackMessagePtr, ref s_ByteOrder_DragInputCallbackMessage, typeof(InputSystem.DragInputCallbackMessage), sizeof(InputSystem.RemoteCallbackMessage), 0);
                    this.HandleDragInput((InputSystem.DragInputCallbackMessage*)remoteCallbackMessagePtr);
                    break;
                default:
                    Debug2.Throw(false, "unexpected input type in IInputCallback.OnInput");
                    break;
            }
        }

        public void RegisterRawInputCallbacks(IRawInputCallbacks handlers)
        {
            Debug2.Validate(handlers != null, typeof(ArgumentException), "must provide valid IRawInputCallbacks callback interface");
            Debug2.Validate(this.m_inputHandlers == null, typeof(InvalidOperationException), "Not allowed to register twice on one session");
            this.m_inputHandlers = handlers;
            this.m_maskInputHandlers = this.m_inputHandlers.InputHandlerMask;
            Debug2.Validate(this.m_maskInputHandlers != InputHandlerFlags.None, typeof(ArgumentException), "must register interest for at least one input type");
        }

        public void UnregisterRawInputCallbacks()
        {
            this.m_inputHandlers = null;
            this.m_maskInputHandlers = InputHandlerFlags.None;
        }

        private unsafe void HandleKeyboardInput(InputSystem.KeyboardCommandMessage* pmsg)
        {
            if (!this.IsRegisteredForInputType(InputHandlerFlags.Keyboard))
                return;
            RawKeyboardData args = new RawKeyboardData((Keys)(ushort)pmsg->vk, pmsg->ScanCode, (uint)pmsg->repCount, pmsg->flags, pmsg->IsRemote != 0 ? InputDeviceType.Other : InputDeviceType.Keyboard);
            this.m_inputHandlers.HandleRawKeyboardInput(pmsg->uiMsg, (InputModifiers)pmsg->stateValue, ref args);
        }

        private unsafe void HandleHIDCommandInput(InputSystem.HIDCommandMessage* pmsg)
        {
            if (!this.IsRegisteredForInputType(InputHandlerFlags.Hid))
                return;
            RawHidData args = new RawHidData(pmsg->usage, pmsg->usagePage, pmsg->wValue != 0U ? KeyAction.Down : KeyAction.Up, pmsg->IsRemote != 0 ? InputDeviceType.Other : InputDeviceType.Keyboard);
            this.m_inputHandlers.HandleRawHidInput(ref args);
        }

        private unsafe void HandleAppCommandInput(InputSystem.AppCommandMessage* pmsg)
        {
            if (!this.IsRegisteredForInputType(InputHandlerFlags.AppCommand))
                return;
            RawHidData args1 = new RawHidData(pmsg->AppCommand, 0U, KeyAction.Down, InputDeviceType.Other);
            this.m_inputHandlers.HandleAppCommand(ref args1);
            RawHidData args2 = new RawHidData(pmsg->AppCommand, 0U, KeyAction.Up, InputDeviceType.Other);
            this.m_inputHandlers.HandleAppCommand(ref args2);
        }

        private unsafe void HandleWindowInput(InputSystem.WindowInputCallbackMessage* msg)
        {
            if (!this.IsRegisteredForInputType(InputHandlerFlags.Mouse))
                return;
            RawMouseData args = new RawMouseData(this.m_session.TryGetHandleOwner(msg->subjectValue), this.m_session.TryGetHandleOwner(msg->naturalValue), msg->positionXOffset, msg->positionYOffset, msg->naturalXOffset, msg->naturalYOffset, msg->physicalXOffset, msg->physicalYOffset, msg->screenXOffset, msg->screenYOffset, (MouseButtons)msg->buttonFlag, msg->wheelDelta);
            this.m_inputHandlers.HandleRawMouseInput(msg->messageValue, (InputModifiers)msg->modifiersValue, ref args);
        }

        private unsafe void HandleDragInput(InputSystem.DragInputCallbackMessage* msg)
        {
            if (!this.IsRegisteredForInputType(InputHandlerFlags.Drag))
                return;
            RawDragData args = new RawDragData(this.m_session.TryGetHandleOwner(msg->subjectValue), msg->positionXOffset, msg->positionYOffset, msg->pDataStream);
            this.m_inputHandlers.HandleRawDragInput(msg->messageValue, (InputModifiers)msg->dragModifiers, ref args);
        }

        private bool IsRegisteredForInputType(InputHandlerFlags flag) => this.m_inputHandlers != null && (this.m_maskInputHandlers & flag) == flag;

        protected override void Invariant() => Debug2.Validate(this.m_session != null, typeof(InvalidOperationException), this.GetType().ToString());

        private struct RemoteCallbackMessage
        {
            public uint sizeCount;
            public int msgValue;
            public uint objectSubjectID;
            public uint ownerCookie;
            public uint ownerMessage;
        }

        private struct KeyboardCommandMessage
        {
            public uint sizeCount;
            public int msgValue;
            public uint objectSubjectID;
            public uint ownerHandle;
            public uint codeValue;
            public int IsRemote;
            public int ScanCode;
            public int vk;
            public uint uiMsg;
            public int repCount;
            public uint stateValue;
            public ushort flags;
        }

        private struct HIDCommandMessage
        {
            public uint sizeCount;
            public int msgValue;
            public uint objectSubjectID;
            public uint ownerHandle;
            public uint codeValue;
            public int IsRemote;
            public uint usagePage;
            public uint usage;
            public uint wValue;
        }

        private struct AppCommandMessage
        {
            public uint sizeCount;
            public int msgValue;
            public uint objectSubjectID;
            public uint ownerHandle;
            public uint codeValue;
            public uint AppCommand;
            public uint deviceValue;
            public uint keysValue;
        }

        private struct WindowInputCallbackMessage
        {
            public uint sizeCount;
            public int msgValue;
            public uint objectSubjectID;
            public uint ownerCookie;
            public uint ownerMessage;
            public uint messageValue;
            public uint subjectValue;
            public int positionXOffset;
            public int positionYOffset;
            public uint naturalValue;
            public int naturalXOffset;
            public int naturalYOffset;
            public int physicalXOffset;
            public int physicalYOffset;
            public int screenXOffset;
            public int screenYOffset;
            public uint modifiersValue;
            public uint buttonFlag;
            public short wheelDelta;
        }

        private struct DragInputCallbackMessage
        {
            public uint sizeCount;
            public int msgValue;
            public uint objectSubjectID;
            public uint ownerCookie;
            public uint ownerMessage;
            public uint messageValue;
            public uint subjectValue;
            public uint dragModifiers;
            public int positionXOffset;
            public int positionYOffset;
            public IntPtr pDataStream;
        }
    }
}
