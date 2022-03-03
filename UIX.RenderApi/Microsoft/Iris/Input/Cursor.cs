// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Input.Cursor
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

namespace Microsoft.Iris.Input
{
    public class Cursor
    {
        public const int NoneResourceId = 1;
        public const int MoveResourceId = 2;
        public const int CopyResourceId = 3;
        public static readonly Cursor Arrow = new Cursor(32512, CursorID.Arrow);
        public static readonly Cursor AppStarting = new Cursor(32550, CursorID.AppStarting);
        public static readonly Cursor Crosshair = new Cursor(32515, CursorID.Crosshair);
        public static readonly Cursor Default = Arrow;
        public static readonly Cursor Hand = new Cursor(32649, CursorID.Hand);
        public static readonly Cursor Help = new Cursor(32651, CursorID.Help);
        public static readonly Cursor IBeam = new Cursor(32513, CursorID.IBeam);
        public static readonly Cursor No = new Cursor(32648, CursorID.No);
        public static readonly Cursor Size = new Cursor(32646, CursorID.Size);
        public static readonly Cursor SizeNS = new Cursor(32645, CursorID.SizeNS);
        public static readonly Cursor SizeWE = new Cursor(32644, CursorID.SizeWE);
        public static readonly Cursor SizeNWSE = new Cursor(32642, CursorID.SizeNWSE);
        public static readonly Cursor SizeNESW = new Cursor(32643, CursorID.SizeNESW);
        public static readonly Cursor WaitCursor = new Cursor(32514, CursorID.Wait);
        public static readonly Cursor UpArrow = new Cursor(32516, CursorID.UpArrow);
        public static readonly Cursor NullCursor = new Cursor(0, CursorID.None);
        public static readonly Cursor Cancel = new Cursor(1, CursorID.Cancel);
        public static readonly Cursor Move = new Cursor(2, CursorID.Move);
        public static readonly Cursor Copy = new Cursor(3, CursorID.Copy);
        private int m_idResource;
        private CursorID m_idCursor;

        public Cursor(int idResource, CursorID idCursor)
        {
            this.m_idResource = idResource;
            this.m_idCursor = idCursor;
        }

        public int ResourceId => this.m_idResource;

        public CursorID CursorID => this.m_idCursor;

        public static Cursor GetCursor(CursorID cursor)
        {
            switch (cursor)
            {
                case CursorID.Arrow:
                    return Arrow;
                case CursorID.Cancel:
                    return Cancel;
                case CursorID.Copy:
                    return Copy;
                case CursorID.Crosshair:
                    return Crosshair;
                case CursorID.IBeam:
                    return IBeam;
                case CursorID.Hand:
                    return Hand;
                case CursorID.Move:
                    return Move;
                case CursorID.No:
                    return No;
                case CursorID.None:
                    return NullCursor;
                case CursorID.Size:
                    return Size;
                case CursorID.SizeNS:
                    return SizeNS;
                case CursorID.SizeWE:
                    return SizeWE;
                case CursorID.SizeNWSE:
                    return SizeNWSE;
                case CursorID.SizeNESW:
                    return SizeNESW;
                case CursorID.UpArrow:
                    return UpArrow;
                case CursorID.Wait:
                    return WaitCursor;
                default:
                    return null;
            }
        }
    }
}
