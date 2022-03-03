// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIXEnumData
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup
{
    internal static class UIXEnumData
    {
        public static Map<string, int> GetAccessibleRoleEnumData() => new Map<string, int>(64)
        {
            ["Alert"] = 8,
            ["Animation"] = 54,
            ["Application"] = 14,
            ["Border"] = 19,
            ["ButtonDropDown"] = 56,
            ["ButtonDropDownGrid"] = 58,
            ["ButtonMenu"] = 57,
            ["Caret"] = 7,
            ["Character"] = 32,
            ["Chart"] = 17,
            ["CheckButton"] = 44,
            ["Client"] = 10,
            ["Clock"] = 61,
            ["Column"] = 27,
            ["ColumnHeader"] = 25,
            ["ComboBox"] = 46,
            ["Cursor"] = 6,
            ["Diagram"] = 53,
            ["Dial"] = 49,
            ["Dialog"] = 18,
            ["Document"] = 15,
            ["DropList"] = 47,
            ["Equation"] = 55,
            ["Graphic"] = 40,
            ["Grip"] = 4,
            ["Grouping"] = 20,
            ["HelpBalloon"] = 31,
            ["HotkeyField"] = 50,
            ["IPAddress"] = 63,
            ["Indicator"] = 39,
            ["Link"] = 30,
            ["List"] = 33,
            ["ListItem"] = 34,
            ["MenuBar"] = 2,
            ["MenuItem"] = 12,
            ["MenuPopup"] = 11,
            ["None"] = 0,
            ["Outline"] = 35,
            ["OutlineButton"] = 64,
            ["OutlineItem"] = 36,
            ["PageTab"] = 37,
            ["PageTabList"] = 60,
            ["Pane"] = 16,
            ["ProgressBar"] = 48,
            ["PropertyPage"] = 38,
            ["PushButton"] = 43,
            ["RadioButton"] = 45,
            ["Row"] = 28,
            ["RowHeader"] = 26,
            ["ScrollBar"] = 3,
            ["Separator"] = 21,
            ["Slider"] = 51,
            ["Sound"] = 5,
            ["SpinButton"] = 52,
            ["SplitButton"] = 62,
            ["StaticText"] = 41,
            ["StatusBar"] = 23,
            ["Table"] = 24,
            ["Text"] = 42,
            ["TitleBar"] = 1,
            ["ToolBar"] = 22,
            ["ToolTip"] = 13,
            ["Whitespace"] = 59,
            ["Window"] = 9
        };

        public static Map<string, int> GetAlignmentEnumData() => new Map<string, int>(5)
        {
            ["Center"] = 2,
            ["Far"] = 3,
            ["Fill"] = 4,
            ["Near"] = 1,
            ["Unspecified"] = 0
        };

        public static Map<string, int> GetAnimationEventTypeEnumData() => new Map<string, int>(12)
        {
            ["Alpha"] = 6,
            ["ContentChangeHide"] = 10,
            ["ContentChangeShow"] = 9,
            ["GainFocus"] = 7,
            ["Hide"] = 1,
            ["Idle"] = 11,
            ["LoseFocus"] = 8,
            ["Move"] = 2,
            ["Rotate"] = 5,
            ["Scale"] = 4,
            ["Show"] = 0,
            ["Size"] = 3
        };

        public static Map<string, int> GetInputHandlerTransitionEnumData() => new Map<string, int>(2)
        {
            ["Down"] = 0,
            ["Up"] = 1
        };

        public static Map<string, int> GetInputHandlerModifiersEnumData() => new Map<string, int>(6)
        {
            ["All"] = 15,
            ["Alt"] = 4,
            ["Ctrl"] = 1,
            ["None"] = 0,
            ["Shift"] = 2,
            ["Windows"] = 8
        };

        public static Map<string, int> GetClickTypeEnumData() => new Map<string, int>(12)
        {
            ["Any"] = sbyte.MaxValue,
            ["EnterKey"] = 16,
            ["GamePad"] = 96,
            ["GamePadA"] = 32,
            ["GamePadStart"] = 64,
            ["Key"] = 24,
            ["LeftMouse"] = 1,
            ["MiddleMouse"] = 4,
            ["Mouse"] = 7,
            ["None"] = 0,
            ["RightMouse"] = 2,
            ["SpaceKey"] = 8
        };

        public static Map<string, int> GetClickCountEnumData() => new Map<string, int>(2)
        {
            ["Double"] = 1,
            ["Single"] = 0
        };

        public static Map<string, int> GetInvokePriorityEnumData() => new Map<string, int>(2)
        {
            ["Low"] = 1,
            ["Normal"] = 0
        };

        public static Map<string, int> GetCursorEnumData() => new Map<string, int>(16)
        {
            ["Arrow"] = 2,
            ["Cancel"] = 3,
            ["Copy"] = 4,
            ["Crosshair"] = 5,
            ["Hand"] = 7,
            ["IBeam"] = 6,
            ["Move"] = 9,
            ["No"] = 10,
            ["NotSpecified"] = 0,
            ["Size"] = 12,
            ["SizeNESW"] = 16,
            ["SizeNS"] = 13,
            ["SizeNWSE"] = 15,
            ["SizeWE"] = 14,
            ["UpArrow"] = 17,
            ["Wait"] = 18
        };

        public static Map<string, int> GetDataQueryStatusEnumData() => new Map<string, int>(5)
        {
            ["Complete"] = 3,
            ["Error"] = 4,
            ["Idle"] = 0,
            ["ProcessingData"] = 2,
            ["RequestingData"] = 1
        };

        public static Map<string, int> GetDebugOutlineScopeEnumData() => new Map<string, int>(4)
        {
            ["All"] = 3,
            ["FlaggedOnly"] = 0,
            ["Hosts"] = 2,
            ["Input"] = 1
        };

        public static Map<string, int> GetDebugLabelFormatEnumData() => new Map<string, int>(3)
        {
            ["Full"] = 2,
            ["Name"] = 1,
            ["None"] = 0
        };

        public static Map<string, int> GetDropActionEnumData() => new Map<string, int>(4)
        {
            ["All"] = 3,
            ["Copy"] = 1,
            ["Move"] = 2,
            ["None"] = 0
        };

        public static Map<string, int> GetBeginDragPolicyEnumData() => new Map<string, int>(2)
        {
            ["Down"] = 0,
            ["Move"] = 1
        };

        public static Map<string, int> GetColorOperationEnumData() => new Map<string, int>(13)
        {
            ["Add"] = 0,
            ["ColorBurn"] = 6,
            ["ColorDodge"] = 5,
            ["Darken"] = 4,
            ["HardLight"] = 10,
            ["Lighten"] = 3,
            ["LinearBurn"] = 11,
            ["LinearDodge"] = 0,
            ["Multiply"] = 2,
            ["Overlay"] = 8,
            ["Screen"] = 7,
            ["SoftLight"] = 9,
            ["Subtract"] = 1
        };

        public static Map<string, int> GetAlphaOperationEnumData() => new Map<string, int>(5)
        {
            ["Add"] = 0,
            ["Multiply"] = 2,
            ["Source1"] = 3,
            ["Source2"] = 4,
            ["Subtract"] = 1
        };

        public static Map<string, int> GetGaussianBlurModeEnumData() => new Map<string, int>(3)
        {
            ["Horizontal"] = 1,
            ["Normal"] = 0,
            ["Vertical"] = 2
        };

        public static Map<string, int> GetEmbossDirectionEnumData() => new Map<string, int>(2)
        {
            ["LeftToRight"] = 0,
            ["RightToLeft"] = 1
        };

        public static Map<string, int> GetColorSchemeEnumData() => new Map<string, int>(3)
        {
            ["Default"] = 0,
            ["HighContrast1"] = 1,
            ["HighContrast2"] = 2
        };

        public static Map<string, int> GetGraphicsDeviceTypeEnumData() => new Map<string, int>(2)
        {
            ["DX9"] = 1,
            ["GDI"] = 0
        };

        public static Map<string, int> GetOrientationEnumData() => new Map<string, int>(2)
        {
            ["Horizontal"] = 0,
            ["Vertical"] = 1
        };

        public static Map<string, int> GetRepeatPolicyEnumData() => new Map<string, int>(4)
        {
            ["Always"] = 3,
            ["Never"] = 0,
            ["WhenTooBig"] = 1,
            ["WhenTooSmall"] = 2
        };

        public static Map<string, int> GetStripAlignmentEnumData() => new Map<string, int>(3)
        {
            ["Center"] = 1,
            ["Far"] = 2,
            ["Near"] = 0
        };

        public static Map<string, int> GetMissingItemPolicyEnumData() => new Map<string, int>(4)
        {
            ["SizeToAverage"] = 0,
            ["SizeToLargest"] = 2,
            ["SizeToSmallest"] = 1,
            ["Wait"] = 3
        };

        public static Map<string, int> GetFocusChangeReasonEnumData() => new Map<string, int>(6)
        {
            ["Any"] = 15,
            ["Directional"] = 2,
            ["Key"] = 6,
            ["Mouse"] = 1,
            ["Other"] = 8,
            ["Tab"] = 4
        };

        public static Map<string, int> GetFontStylesEnumData() => new Map<string, int>(3)
        {
            ["Bold"] = 1,
            ["Italic"] = 2,
            ["None"] = 0
        };

        public static Map<string, int> GetSizingPolicyEnumData() => new Map<string, int>(3)
        {
            ["SizeToChildren"] = 1,
            ["SizeToConstraint"] = 2,
            ["SizeToContent"] = 0
        };

        public static Map<string, int> GetStretchingPolicyEnumData() => new Map<string, int>(4)
        {
            ["Fill"] = 1,
            ["None"] = 0,
            ["Uniform"] = 2,
            ["UniformToFill"] = 3
        };

        public static Map<string, int> GetHostStatusEnumData() => new Map<string, int>(4)
        {
            ["FailureLoadingSource"] = 2,
            ["FailureRunningScript"] = 3,
            ["LoadingSource"] = 1,
            ["Normal"] = 0
        };

        public static Map<string, int> GetImageStatusEnumData() => new Map<string, int>(4)
        {
            ["Complete"] = 2,
            ["Error"] = 3,
            ["Loading"] = 1,
            ["PendingLoad"] = 0
        };

        public static Map<string, int> GetInputHandlerStageEnumData() => new Map<string, int>(3)
        {
            ["Bubbled"] = 4,
            ["Direct"] = 2,
            ["Routed"] = 1
        };

        public static Map<string, int> GetInterpolationTypeEnumData() => new Map<string, int>(9)
        {
            ["Bezier"] = 6,
            ["Cosine"] = 5,
            ["EaseIn"] = 7,
            ["EaseOut"] = 8,
            ["Exp"] = 2,
            ["Linear"] = 0,
            ["Log"] = 3,
            ["SCurve"] = 1,
            ["Sine"] = 4
        };

        public static Map<string, int> GetKeyHandlerKeyEnumData() => new Map<string, int>(143)
        {
            ["A"] = 65,
            ["Add"] = 107,
            ["Alt"] = 18,
            ["Any"] = -1,
            ["Apps"] = 93,
            ["B"] = 66,
            ["Backslash"] = 226,
            ["Backspace"] = 8,
            ["C"] = 67,
            ["CapsLock"] = 20,
            ["Clear"] = 12,
            ["CloseBrace"] = 221,
            ["Comma"] = 188,
            ["Control"] = 17,
            ["D"] = 68,
            ["D0"] = 48,
            ["D1"] = 49,
            ["D2"] = 50,
            ["D3"] = 51,
            ["D4"] = 52,
            ["D5"] = 53,
            ["D6"] = 54,
            ["D7"] = 55,
            ["D8"] = 56,
            ["D9"] = 57,
            ["Delete"] = 46,
            ["Divide"] = 111,
            ["Down"] = 40,
            ["E"] = 69,
            ["End"] = 35,
            ["Enter"] = 13,
            ["Equals"] = 187,
            ["Escape"] = 27,
            ["F"] = 70,
            ["F1"] = 112,
            ["F10"] = 121,
            ["F11"] = 122,
            ["F12"] = 123,
            ["F13"] = 124,
            ["F14"] = 125,
            ["F15"] = 126,
            ["F16"] = sbyte.MaxValue,
            ["F17"] = 128,
            ["F18"] = 129,
            ["F19"] = 130,
            ["F2"] = 113,
            ["F20"] = 131,
            ["F21"] = 132,
            ["F22"] = 133,
            ["F23"] = 134,
            ["F24"] = 135,
            ["F3"] = 114,
            ["F4"] = 115,
            ["F5"] = 116,
            ["F6"] = 117,
            ["F7"] = 118,
            ["F8"] = 119,
            ["F9"] = 120,
            ["G"] = 71,
            ["GamePadA"] = 22528,
            ["GamePadB"] = 22529,
            ["GamePadBack"] = 22597,
            ["GamePadDPadDown"] = 22545,
            ["GamePadDPadLeft"] = 22546,
            ["GamePadDPadRight"] = 22547,
            ["GamePadDPadUp"] = 22544,
            ["GamePadLShoulder"] = 22533,
            ["GamePadLThumbDown"] = 22561,
            ["GamePadLThumbDownLeft"] = 22567,
            ["GamePadLThumbDownRight"] = 22566,
            ["GamePadLThumbLeft"] = 22563,
            ["GamePadLThumbPress"] = 22550,
            ["GamePadLThumbRight"] = 22562,
            ["GamePadLThumbUp"] = 22560,
            ["GamePadLThumbUpLeft"] = 22564,
            ["GamePadLThumbUpRight"] = 22565,
            ["GamePadLTrigger"] = 22534,
            ["GamePadRShoulder"] = 22532,
            ["GamePadRThumbDown"] = 22577,
            ["GamePadRThumbDownLeft"] = 22583,
            ["GamePadRThumbDownRight"] = 22582,
            ["GamePadRThumbLeft"] = 22579,
            ["GamePadRThumbPress"] = 22551,
            ["GamePadRThumbRight"] = 22578,
            ["GamePadRThumbUp"] = 22576,
            ["GamePadRThumbUpLeft"] = 22580,
            ["GamePadRThumbUpRight"] = 22581,
            ["GamePadRTrigger"] = 22535,
            ["GamePadStart"] = 22548,
            ["GamePadX"] = 22530,
            ["GamePadY"] = 22531,
            ["H"] = 72,
            ["Home"] = 36,
            ["I"] = 73,
            ["Insert"] = 45,
            ["J"] = 74,
            ["K"] = 75,
            ["L"] = 76,
            ["Left"] = 37,
            ["M"] = 77,
            ["Multiply"] = 106,
            ["N"] = 78,
            ["None"] = 0,
            ["NumLock"] = 144,
            ["NumPad0"] = 96,
            ["NumPad1"] = 97,
            ["NumPad2"] = 98,
            ["NumPad3"] = 99,
            ["NumPad4"] = 100,
            ["NumPad5"] = 101,
            ["NumPad6"] = 102,
            ["NumPad7"] = 103,
            ["NumPad8"] = 104,
            ["NumPad9"] = 105,
            ["O"] = 79,
            ["OpenBrace"] = 219,
            ["P"] = 80,
            ["PageDown"] = 34,
            ["PageUp"] = 33,
            ["Period"] = 190,
            ["Pipe"] = 220,
            ["PrintScreen"] = 44,
            ["Q"] = 81,
            ["QuestionMark"] = 191,
            ["Quotes"] = 222,
            ["R"] = 82,
            ["Right"] = 39,
            ["S"] = 83,
            ["Semicolon"] = 186,
            ["Shift"] = 16,
            ["Space"] = 32,
            ["Subtract"] = 109,
            ["T"] = 84,
            ["Tab"] = 9,
            ["Tilde"] = 192,
            ["U"] = 85,
            ["Underscore"] = 189,
            ["Up"] = 38,
            ["V"] = 86,
            ["W"] = 87,
            ["X"] = 88,
            ["Y"] = 89,
            ["Z"] = 90
        };

        public static Map<string, int> GetNavigationPoliciesEnumData() => new Map<string, int>(20)
        {
            ["Column"] = 4,
            ["ContainAll"] = 1792,
            ["ContainDirectional"] = 768,
            ["ContainHorizontal"] = 512,
            ["ContainTabOrder"] = 1024,
            ["ContainVertical"] = 256,
            ["FlowHorizontal"] = 65536,
            ["FlowVertical"] = 131072,
            ["Group"] = 1,
            ["None"] = 0,
            ["PreferContainerFocus"] = 262144,
            ["PreferFocusOrder"] = 64,
            ["RememberFocus"] = 32,
            ["Row"] = 2,
            ["TabGroup"] = 16,
            ["WrapAll"] = 28672,
            ["WrapDirectional"] = 12288,
            ["WrapHorizontal"] = 8192,
            ["WrapTabOrder"] = 16384,
            ["WrapVertical"] = 4096
        };

        public static Map<string, int> GetMouseTargetEnumData() => new Map<string, int>(3)
        {
            ["Fixed"] = 1,
            ["Follow"] = 2,
            ["None"] = 0
        };

        public static Map<string, int> GetInterestPointEnumData() => new Map<string, int>(5)
        {
            ["BottomLeft"] = 2,
            ["BottomRight"] = 3,
            ["Center"] = 4,
            ["TopLeft"] = 0,
            ["TopRight"] = 1
        };

        public static Map<string, int> GetFlipDirectionEnumData() => new Map<string, int>(4)
        {
            ["Both"] = 3,
            ["Horizontal"] = 1,
            ["None"] = 0,
            ["Vertical"] = 2
        };

        public static Map<string, int> GetSnapshotPolicyEnumData() => new Map<string, int>(3)
        {
            ["Continuous"] = 0,
            ["OnLoop"] = 2,
            ["Once"] = 1
        };

        public static Map<string, int> GetRelativeEdgeEnumData() => new Map<string, int>(2)
        {
            ["Far"] = 1,
            ["Near"] = 0
        };

        public static Map<string, int> GetContentPositioningPolicyEnumData() => new Map<string, int>(2)
        {
            ["RespectPaddingAndLocking"] = 0,
            ["ShowMaximalContent"] = 1
        };

        public static Map<string, int> GetSharedSizePolicyEnumData() => new Map<string, int>(7)
        {
            ["ContributesToHeight"] = 2,
            ["ContributesToSize"] = 3,
            ["ContributesToWidth"] = 1,
            ["Default"] = 15,
            ["SharesHeight"] = 8,
            ["SharesSize"] = 12,
            ["SharesWidth"] = 4
        };

        public static Map<string, int> GetShortcutHandlerCommandEnumData() => new Map<string, int>(29)
        {
            ["Back"] = 16,
            ["ChannelDown"] = 3,
            ["ChannelUp"] = 2,
            ["Clear"] = 26,
            ["Enter"] = 36,
            ["FastForward"] = 7,
            ["None"] = 0,
            ["OEMExtensibility0"] = 73,
            ["OEMExtensibility1"] = 74,
            ["OEMExtensibility10"] = 83,
            ["OEMExtensibility11"] = 84,
            ["OEMExtensibility2"] = 75,
            ["OEMExtensibility3"] = 76,
            ["OEMExtensibility4"] = 77,
            ["OEMExtensibility5"] = 78,
            ["OEMExtensibility6"] = 79,
            ["OEMExtensibility7"] = 80,
            ["OEMExtensibility8"] = 81,
            ["OEMExtensibility9"] = 82,
            ["PageDown"] = 3,
            ["PageUp"] = 2,
            ["Pause"] = 5,
            ["Play"] = 4,
            ["PlayPause"] = 30,
            ["Record"] = 6,
            ["Rewind"] = 8,
            ["SkipBack"] = 10,
            ["SkipForward"] = 9,
            ["Stop"] = 11
        };

        public static Map<string, int> GetSystemSoundEventEnumData() => new Map<string, int>(30)
        {
            ["Asterisk"] = 1,
            ["CloseProgram"] = 2,
            ["CriticalBatteryAlarm"] = 3,
            ["CriticalStop"] = 4,
            ["DefaultBeep"] = 5,
            ["DeviceConnect"] = 6,
            ["DeviceDisconnect"] = 7,
            ["DeviceFailedToConnect"] = 8,
            ["Exclamation"] = 9,
            ["ExitWindows"] = 10,
            ["LowBatteryAlarm"] = 11,
            ["Maximize"] = 12,
            ["MenuCommand"] = 13,
            ["MenuPopup"] = 14,
            ["Minimize"] = 15,
            ["NewFaxNotification"] = 16,
            ["NewMailNotification"] = 17,
            ["None"] = 0,
            ["OpenProgram"] = 18,
            ["PrintComplete"] = 19,
            ["ProgramError"] = 20,
            ["Question"] = 21,
            ["RestoreDown"] = 22,
            ["RestoreUp"] = 23,
            ["Select"] = 24,
            ["ShowToolbarBand"] = 25,
            ["StartWindows"] = 26,
            ["SystemNotification"] = 27,
            ["WindowsLogoff"] = 28,
            ["WindowsLogon"] = 29
        };

        public static Map<string, int> GetStackPriorityEnumData() => new Map<string, int>(3)
        {
            ["High"] = 2,
            ["Low"] = 0,
            ["Medium"] = 1
        };

        public static Map<string, int> GetLineAlignmentEnumData() => new Map<string, int>(3)
        {
            ["Center"] = 1,
            ["Far"] = 2,
            ["Near"] = 0
        };

        public static Map<string, int> GetTextBoundsEnumData() => new Map<string, int>(5)
        {
            ["AlignToAscender"] = 1,
            ["AlignToBaseline"] = 2,
            ["Full"] = 0,
            ["Tight"] = 7,
            ["TrimLeftSideBearing"] = 4
        };

        public static Map<string, int> GetTextSharpnessEnumData() => new Map<string, int>(2)
        {
            ["Sharp"] = 0,
            ["Soft"] = 1
        };

        public static Map<string, int> GetKeyframeFilterEnumData() => new Map<string, int>(6)
        {
            ["All"] = 0,
            ["Alpha"] = 5,
            ["Position"] = 1,
            ["Rotate"] = 4,
            ["Scale"] = 3,
            ["Size"] = 2
        };

        public static Map<string, int> GetTransformAttributeEnumData() => new Map<string, int>(5)
        {
            ["Height"] = 2,
            ["Index"] = 0,
            ["Width"] = 1,
            ["X"] = 3,
            ["Y"] = 4
        };

        public static Map<string, int> GetWindowStateEnumData() => new Map<string, int>(3)
        {
            ["Maximized"] = 2,
            ["Minimized"] = 1,
            ["Normal"] = 0
        };

        public static Map<string, int> GetMaximizeModeEnumData() => new Map<string, int>(2)
        {
            ["FullScreen"] = 1,
            ["Normal"] = 0
        };

        public static Map<string, int> GetDataForType(short typeID)
        {
            switch (typeID)
            {
                case 1:
                    return GetAccessibleRoleEnumData();
                case 3:
                    return GetAlignmentEnumData();
                case 5:
                    return GetAlphaOperationEnumData();
                case 10:
                    return GetAnimationEventTypeEnumData();
                case 12:
                    return GetBeginDragPolicyEnumData();
                case 31:
                    return GetClickCountEnumData();
                case 33:
                    return GetClickTypeEnumData();
                case 38:
                    return GetColorOperationEnumData();
                case 39:
                    return GetColorSchemeEnumData();
                case 41:
                    return GetContentPositioningPolicyEnumData();
                case 44:
                    return GetCursorEnumData();
                case 47:
                    return GetDataQueryStatusEnumData();
                case 50:
                    return GetDebugLabelFormatEnumData();
                case 51:
                    return GetDebugOutlineScopeEnumData();
                case 64:
                    return GetDropActionEnumData();
                case 84:
                    return GetEmbossDirectionEnumData();
                case 89:
                    return GetFlipDirectionEnumData();
                case 91:
                    return GetFocusChangeReasonEnumData();
                case 94:
                    return GetFontStylesEnumData();
                case 96:
                    return GetGaussianBlurModeEnumData();
                case 98:
                    return GetGraphicsDeviceTypeEnumData();
                case 102:
                    return GetHostStatusEnumData();
                case 108:
                    return GetImageStatusEnumData();
                case 111:
                    return GetInputHandlerModifiersEnumData();
                case 112:
                    return GetInputHandlerStageEnumData();
                case 113:
                    return GetInputHandlerTransitionEnumData();
                case 118:
                    return GetInterestPointEnumData();
                case 122:
                    return GetInterpolationTypeEnumData();
                case 126:
                    return GetInvokePriorityEnumData();
                case 129:
                    return GetKeyHandlerKeyEnumData();
                case 131:
                    return GetKeyframeFilterEnumData();
                case 137:
                    return GetLineAlignmentEnumData();
                case 146:
                    return GetMaximizeModeEnumData();
                case 148:
                    return GetMissingItemPolicyEnumData();
                case 149:
                    return GetMouseTargetEnumData();
                case 151:
                    return GetNavigationPoliciesEnumData();
                case 154:
                    return GetOrientationEnumData();
                case 170:
                    return GetRelativeEdgeEnumData();
                case 172:
                    return GetRepeatPolicyEnumData();
                case 191:
                    return GetSharedSizePolicyEnumData();
                case 193:
                    return GetShortcutHandlerCommandEnumData();
                case 199:
                    return GetSizingPolicyEnumData();
                case 200:
                    return GetSnapshotPolicyEnumData();
                case 206:
                    return GetStackPriorityEnumData();
                case 207:
                    return GetStretchingPolicyEnumData();
                case 209:
                    return GetStripAlignmentEnumData();
                case 211:
                    return GetSystemSoundEventEnumData();
                case 213:
                    return GetTextBoundsEnumData();
                case 219:
                    return GetTextSharpnessEnumData();
                case 223:
                    return GetTransformAttributeEnumData();
                case 242:
                    return GetWindowStateEnumData();
                default:
                    return null;
            }
        }
    }
}
