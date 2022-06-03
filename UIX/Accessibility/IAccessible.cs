#if NET6_0_OR_GREATER

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Accessibility
{
	[ComImport, Guid("618736E0-3C3D-11CF-810C-00AA00389B71"), InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface IAccessible
	{
		[DispId(-5000)]
		object accParent { [return: MarshalAs(UnmanagedType.IDispatch)][MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-5000)] get; }

		[DispId(-5001)]
		int accChildCount { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-5001)] get; }

		[return: MarshalAs(UnmanagedType.IDispatch)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-5002)]
		object get_accChild(object varChild);

		[return: MarshalAs(UnmanagedType.BStr)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-5003)]
		string get_accName(object varChild);

		[return: MarshalAs(UnmanagedType.BStr)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-5004)]
		string get_accValue(object varChild);

		[return: MarshalAs(UnmanagedType.BStr)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-5005)]
		string get_accDescription(object varChild);

		[return: MarshalAs(UnmanagedType.Struct)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-5006)]
		object get_accRole(object varChild);

		[return: MarshalAs(UnmanagedType.Struct)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-5007)]
		object get_accState(object varChild);

		[return: MarshalAs(UnmanagedType.BStr)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-5008)]
		string get_accHelp(object varChild);

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-5009)]
		int get_accHelpTopic([MarshalAs(UnmanagedType.BStr)] ref string pszHelpFile, object varChild);

		[return: MarshalAs(UnmanagedType.BStr)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-5010)]
		string get_accKeyboardShortcut(object varChild);

		[DispId(-5011)]
		object accFocus { [return: MarshalAs(UnmanagedType.Struct)][MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-5011)] get; }

		[DispId(-5012)]
		object accSelection { [return: MarshalAs(UnmanagedType.Struct)][MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-5012)] get; }

		[return: MarshalAs(UnmanagedType.BStr)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-5013)]
		string get_accDefaultAction(object varChild);

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-5014)]
		void accSelect([In] int flagsSelect, [In, Optional, MarshalAs(UnmanagedType.Struct)] object varChild);

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-5015)]
		void accLocation(out int pxLeft, out int pyTop, out int pcxWidth, out int pcyHeight, [In, Optional, MarshalAs(UnmanagedType.Struct)] object varChild);

		[return: MarshalAs(UnmanagedType.Struct)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-5016)]
		object accNavigate([In] int navDir, [In, Optional, MarshalAs(UnmanagedType.Struct)] object varStart);

		[return: MarshalAs(UnmanagedType.Struct)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-5017)]
		object accHitTest([In] int xLeft, [In] int yTop);

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-5018)]
		void accDoDefaultAction([In, Optional, MarshalAs(UnmanagedType.Struct)] object varChild);

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-5003)]
		void put_accName(object varChild, [In, MarshalAs(UnmanagedType.BStr)] string szName);

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-5004)]
		void put_accValue(object varChild, [In, MarshalAs(UnmanagedType.BStr)] string szValue);
	}
}

#endif
