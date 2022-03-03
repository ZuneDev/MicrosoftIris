// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocol.RenderException
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;

namespace Microsoft.Iris.Render.Protocol
{
    [Serializable]
    internal class RenderException : InvalidOperationException
    {
        private const int ErrorPrefix = -2147221504;
        private RenderException.ErrorCode m_code;

        public RenderException()
        {
        }

        public RenderException(string stReason)
          : base(stReason)
        {
        }

        public RenderException(string stReason, Exception innerException)
          : base(stReason, innerException)
        {
        }

        public RenderException(RenderException.ErrorCode code) => this.m_code = code;

        public RenderException(RenderException.ErrorCode code, string stReason)
          : base(stReason)
          => this.m_code = code;

        public RenderException.ErrorCode Error => this.m_code;

        public enum ErrorCode
        {
            OutOfKernelResources = -2147221503, // 0x80040001
            OutOfGdiResources = -2147221502, // 0x80040002
            Generic = -2147221494, // 0x8004000A
            Busy = -2147221493, // 0x8004000B
            Unusable = -2147221492, // 0x8004000C
            NoContext = -2147221484, // 0x80040014
            InvalidContext = -2147221474, // 0x8004001E
            ReadOnlyContext = -2147221473, // 0x8004001F
            ThreadingAlreadySet = -2147221472, // 0x80040020
            CannotUseStandardMessaging = -2147221471, // 0x80040021
            BadCoordinateMap = -2147221464, // 0x80040028
            CannotFindMsgID = -2147221454, // 0x80040032
            NotBuffered = -2147221444, // 0x8004003C
            StartDestroy = -2147221434, // 0x80040046
            ObjectLocked = -2147221433, // 0x80040047
            InvalidOperation = -2147221432, // 0x80040048
            NotInitialized = -2147221424, // 0x80040050
            NotFound = -2147221414, // 0x8004005A
            IdAlreadyUsed = -2147221413, // 0x8004005B
            FileNotFound = -2147221412, // 0x8004005C
            OutOfRange = -2147221411, // 0x8004005D
            MismatchedTypes = -2147221404, // 0x80040064
            CannotLoadGdiplus = -2147221394, // 0x8004006E
            CannotLoadDirect3D = -2147221393, // 0x8004006F
            ClassAlreadyRegistered = -2147221384, // 0x80040078
            MessageNotFound = -2147221383, // 0x80040079
            MessageNotImplemented = -2147221382, // 0x8004007A
            ClassNotImplemented = -2147221381, // 0x8004007B
            MessageFailed = -2147221380, // 0x8004007C
            MessageData = -2147221379, // 0x8004007D
            NoContent = -2147221374, // 0x80040082
            NoStorage = -2147221373, // 0x80040083
            GenericWin32 = -2147221364, // 0x8004008C
            GenericGdiPlus = -2147221363, // 0x8004008D
            GenericDriver = -2147221362, // 0x8004008E
            UnableToConnect = -2147221354, // 0x80040096
        }
    }
}
