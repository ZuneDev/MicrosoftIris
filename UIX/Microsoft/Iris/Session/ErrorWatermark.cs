﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Session.ErrorWatermark
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Session
{
    public struct ErrorWatermark
    {
        private uint _currentErrorCount;

        public ErrorWatermark(uint currentErrorCount) => _currentErrorCount = currentErrorCount;

        public bool ErrorsDetected => (int)_currentErrorCount != (int)ErrorManager.TotalErrorsReported;
    }
}
