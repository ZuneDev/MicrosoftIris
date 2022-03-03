// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Navigation.NavigationServices
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Debug;
using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI.Drawing;
using Microsoft.Iris.Session;
using System.Collections;
using System.Diagnostics;

namespace Microsoft.Iris.Navigation
{
    internal static class NavigationServices
    {
        private static float s_originNear = 0.0f;
        private static float s_originSize = 0.0f;
        private static NavigationServices.SearchOrientation s_originOrientation = SearchOrientation.None;

        public static bool FindNextPeer(
          INavigationSite originSite,
          Direction searchDirection,
          RectangleF startRectangleF,
          out INavigationSite resultSite)
        {
            INavigationSite navigationSite1 = originSite;
            Debug.Trace.IsCategoryEnabled(TraceCategory.Navigation, 2);
            if (startRectangleF.IsEmpty && !GetDefaultOutboundStartRect(originSite, out startRectangleF))
            {
                resultSite = null;
                return false;
            }
            ProcessDirectionalMemory(ref startRectangleF, searchDirection);
            INavigationSite parentTabGroup = FindParentTabGroup(navigationSite1, searchDirection);
            if (parentTabGroup != null)
                navigationSite1 = parentTabGroup;
            INavigationSite boundingSite = FindBoundingSite(navigationSite1, searchDirection);
            INavigationSite navigationSite2 = null;
            NavigationItem itemForSite1 = NavigationItem.CreateItemForSite(navigationSite1, searchDirection, false);
            if (itemForSite1 != null)
            {
                NavigationItem navigationItem1 = itemForSite1.SearchUpTree(startRectangleF, null, null);
                if (navigationItem1 != null)
                    navigationSite2 = navigationItem1.Subject;
                if (navigationSite2 == null && boundingSite != null && NavigationItem.IsWrappingSite(boundingSite, searchDirection))
                {
                    Vector3 positionPxlVector;
                    Vector3 sizePxlVector;
                    boundingSite.ComputeBounds(out positionPxlVector, out sizePxlVector);
                    RectangleF excludeRectangleF = new RectangleF(positionPxlVector.X, positionPxlVector.Y, sizePxlVector.X, sizePxlVector.Y);
                    AdjustStartRectForSimulatedEntry(searchDirection, excludeRectangleF, ref startRectangleF);
                    NavigationItem itemForSite2 = NavigationItem.CreateItemForSite(boundingSite, searchDirection, true);
                    if (itemForSite2 != null)
                    {
                        NavigationItem navigationItem2 = itemForSite2.SearchDownTree(startRectangleF, true, boundingSite, originSite);
                        if (navigationItem2 != null)
                            navigationSite2 = navigationItem2.Subject;
                    }
                    if (navigationSite2 == originSite)
                        navigationSite2 = null;
                }
            }
            bool flag = navigationSite2 != null;
            resultSite = navigationSite2;
            return flag || boundingSite != null;
        }

        public static bool FindFromPoint(
          INavigationSite originSite,
          PointF pt,
          out INavigationSite result)
        {
            return FindFromPoint(originSite, Direction.Next, pt, out result);
        }

        public static bool FindFromPoint(
          INavigationSite originSite,
          Direction bias,
          PointF pt,
          out INavigationSite result)
        {
            ResetDirectionalMemory();
            return new FindFromPointWorker(originSite, bias).FindFromPoint(pt, out result);
        }

        public static bool FindNextWithin(
          INavigationSite originSite,
          Direction searchDirection,
          RectangleF startRectangleF,
          out INavigationSite resultSite)
        {
            Debug.Trace.IsCategoryEnabled(TraceCategory.Navigation, 2);
            if (startRectangleF.IsEmpty && !GetDefaultInboundStartRect(originSite, searchDirection, out startRectangleF))
            {
                resultSite = null;
                return false;
            }
            ProcessDirectionalMemory(ref startRectangleF, searchDirection);
            INavigationSite navigationSite = null;
            NavigationItem itemForSite = NavigationItem.CreateItemForSite(originSite, searchDirection, true);
            if (itemForSite != null)
            {
                NavigationItem navigationItem = itemForSite.SearchDownTree(startRectangleF, true, null, null);
                if (navigationItem != null)
                    navigationSite = navigationItem.Subject;
            }
            bool flag = navigationSite != null;
            resultSite = navigationSite;
            return flag;
        }

        public static void SeedDefaultFocus(INavigationSite focusSite)
        {
            if (focusSite == null)
                return;
            NavigationItem.RememberFocus(focusSite);
        }

        public static void ClearDefaultFocus(INavigationSite startSite) => NavigationItem.ClearFocus(startSite);

        public static bool GetDefaultInboundStartRect(
          INavigationSite originSite,
          Direction searchDirection,
          out RectangleF startRectangleF)
        {
            if (!GetDefaultOutboundStartRect(originSite, out startRectangleF))
                return false;
            AdjustStartRectForSimulatedEntry(searchDirection, startRectangleF, ref startRectangleF);
            return true;
        }

        public static bool GetDefaultOutboundStartRect(
          INavigationSite originSite,
          out RectangleF startRectangleF)
        {
            if (!originSite.Visible)
            {
                startRectangleF = RectangleF.Zero;
                return false;
            }
            Vector3 positionPxlVector;
            Vector3 sizePxlVector;
            originSite.ComputeBounds(out positionPxlVector, out sizePxlVector);
            startRectangleF = new RectangleF(positionPxlVector.X, positionPxlVector.Y, sizePxlVector.X, sizePxlVector.Y);
            return true;
        }

        private static void ResetDirectionalMemory() => UpdateDirectionalMemoryInfo(RectangleF.Zero, Direction.Next);

        private static bool UpdateDirectionalMemoryInfo(
          RectangleF startRectangleF,
          Direction searchDirection)
        {
            float num1 = 0.0f;
            float num2 = 0.0f;
            NavigationServices.SearchOrientation searchOrientation = SearchOrientation.None;
            switch (searchDirection)
            {
                case Direction.North:
                case Direction.South:
                    searchOrientation = SearchOrientation.Vertical;
                    num1 = startRectangleF.X;
                    num2 = startRectangleF.Width;
                    break;
                case Direction.East:
                case Direction.West:
                    searchOrientation = SearchOrientation.Horizontal;
                    num1 = startRectangleF.Y;
                    num2 = startRectangleF.Height;
                    break;
                case Direction.Previous:
                case Direction.Next:
                    searchOrientation = SearchOrientation.None;
                    break;
            }
            bool flag = s_originOrientation != searchOrientation;
            if (flag)
            {
                s_originOrientation = searchOrientation;
                s_originNear = num1;
                s_originSize = num2;
            }
            return flag;
        }

        private static void ProcessDirectionalMemory(
          ref RectangleF startRectangleF,
          Direction searchDirection)
        {
            if (UpdateDirectionalMemoryInfo(startRectangleF, searchDirection))
                return;
            bool flag = true;
            switch (s_originOrientation)
            {
                case SearchOrientation.Horizontal:
                    startRectangleF.Y = s_originNear;
                    startRectangleF.Height = s_originSize;
                    break;
                case SearchOrientation.Vertical:
                    startRectangleF.X = s_originNear;
                    startRectangleF.Width = s_originSize;
                    break;
                case SearchOrientation.None:
                    flag = false;
                    break;
            }
            int num = flag ? 1 : 0;
        }

        private static INavigationSite FindBoundingSite(
          INavigationSite searchSite,
          Direction searchDirection)
        {
            while (searchSite != null && !NavigationItem.IsBoundingSite(searchSite, searchDirection))
                searchSite = searchSite.Parent;
            return searchSite;
        }

        private static INavigationSite FindParentTabGroup(
          INavigationSite targetSite,
          Direction searchDirection)
        {
            switch (searchDirection)
            {
                case Direction.Previous:
                case Direction.Next:
                    for (; targetSite != null; targetSite = targetSite.Parent)
                    {
                        if (NavigationItem.IsBoundingSite(targetSite, searchDirection))
                            return null;
                        if (NavigationItem.IsTabGroup(targetSite))
                            break;
                    }
                    return targetSite;
                default:
                    return null;
            }
        }

        private static void AdjustStartRectForSimulatedEntry(
          Direction searchDirection,
          RectangleF excludeRectangleF,
          ref RectangleF startRectangleF)
        {
            switch (searchDirection)
            {
                case Direction.North:
                    startRectangleF.Y = excludeRectangleF.Bottom;
                    break;
                case Direction.South:
                    startRectangleF.Y = excludeRectangleF.Top - startRectangleF.Height;
                    break;
                case Direction.East:
                    startRectangleF.X = excludeRectangleF.Left - startRectangleF.Width;
                    break;
                case Direction.West:
                    startRectangleF.X = excludeRectangleF.Right;
                    break;
            }
        }

        [Conditional("DEBUG")]
        private static void DumpSiteTree(INavigationSite branchSite, INavigationSite markSite)
        {
            int num = branchSite.IsLogicalJunction ? 1 : 0;
            if (branchSite.Navigability != NavigationClass.None)
                InvariantString.Format(", Class: {0}", branchSite.Navigability);
            if (branchSite.Mode != NavigationPolicies.None)
                InvariantString.Format(", Mode: {0}", branchSite.Mode);
            foreach (INavigationSite child in branchSite.Children)
                ;
        }

        [Conditional("DEBUG")]
        private static void DumpSiteTreeContaining(INavigationSite originSite)
        {
            INavigationSite navigationSite = originSite;
            while (true)
            {
                INavigationSite parent = navigationSite.Parent;
                if (parent != null)
                    navigationSite = parent;
                else
                    break;
            }
        }

        private enum SearchOrientation
        {
            Horizontal,
            Vertical,
            None,
        }
    }
}
