// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.FormStyleInfo
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

namespace Microsoft.Iris.Render
{
    public struct FormStyleInfo
    {
        public uint uStyleRestored;
        public uint uExStyleRestored;
        public uint uStyleMinimized;
        public uint uExStyleMinimized;
        public uint uStyleMaximized;
        public uint uExStyleMaximized;
        public uint uStyleFullscreen;
        public uint uExStyleFullscreen;

        public override bool Equals(object o) => o is FormStyleInfo formStyleInfo && (int)this.uStyleRestored == (int)formStyleInfo.uStyleRestored && ((int)this.uExStyleRestored == (int)formStyleInfo.uExStyleRestored && (int)this.uStyleMinimized == (int)formStyleInfo.uStyleMinimized) && ((int)this.uExStyleMinimized == (int)formStyleInfo.uExStyleMinimized && (int)this.uStyleMaximized == (int)formStyleInfo.uStyleMaximized && ((int)this.uExStyleMaximized == (int)formStyleInfo.uExStyleMaximized && (int)this.uStyleFullscreen == (int)formStyleInfo.uStyleFullscreen)) && (int)this.uExStyleFullscreen == (int)formStyleInfo.uExStyleFullscreen;

        public override int GetHashCode() => (int)this.uStyleRestored ^ (int)this.uExStyleRestored;

        public static bool operator ==(FormStyleInfo fsi1, FormStyleInfo fsi2) => fsi1.Equals(fsi2);

        public static bool operator !=(FormStyleInfo fsi1, FormStyleInfo fsi2) => !(fsi1 == fsi2);
    }
}
