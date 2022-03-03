// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Library.InvariantString
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Iris.Library
{
    public static class InvariantString
    {
        public static string Format(string format, object param) => string.Format(CultureInfo.InvariantCulture, format, param);

        public static string Format(string format, object[] param) => throw new Exception("Should never format with object array. Use one of the dedicated format methods");

        public static string Format(string format, object param1, object param2) => string.Format(CultureInfo.InvariantCulture, format, param1, param2);

        public static string Format(string format, object param1, object param2, object param3) => string.Format(CultureInfo.InvariantCulture, format, param1, param2, param3);

        public static string Format(
          string format,
          object param1,
          object param2,
          object param3,
          object param4)
        {
            return string.Format(CultureInfo.InvariantCulture, format, param1, param2, param3, param4);
        }

        public static string Format(
          string format,
          object param1,
          object param2,
          object param3,
          object param4,
          object param5)
        {
            return string.Format(CultureInfo.InvariantCulture, format, param1, param2, param3, param4, param5);
        }

        public static string Format(
          string format,
          object param1,
          object param2,
          object param3,
          object param4,
          object param5,
          object param6)
        {
            return string.Format(CultureInfo.InvariantCulture, format, param1, param2, param3, param4, param5, param6);
        }

        public static string Format(
          string format,
          object param1,
          object param2,
          object param3,
          object param4,
          object param5,
          object param6,
          object param7)
        {
            return string.Format(CultureInfo.InvariantCulture, format, param1, param2, param3, param4, param5, param6, param7);
        }

        public static string Format(
          string format,
          object param1,
          object param2,
          object param3,
          object param4,
          object param5,
          object param6,
          object param7,
          object param8)
        {
            return string.Format(CultureInfo.InvariantCulture, format, param1, param2, param3, param4, param5, param6, param7, param8);
        }

        public static string Format(
          string format,
          object param1,
          object param2,
          object param3,
          object param4,
          object param5,
          object param6,
          object param7,
          object param8,
          object param9)
        {
            return string.Format(CultureInfo.InvariantCulture, format, param1, param2, param3, param4, param5, param6, param7, param8, param9);
        }

        public static string Format(
          string format,
          object param1,
          object param2,
          object param3,
          object param4,
          object param5,
          object param6,
          object param7,
          object param8,
          object param9,
          object param10)
        {
            return string.Format(CultureInfo.InvariantCulture, format, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10);
        }

        public static string Format(
          string format,
          object param1,
          object param2,
          object param3,
          object param4,
          object param5,
          object param6,
          object param7,
          object param8,
          object param9,
          object param10,
          object param11)
        {
            return string.Format(CultureInfo.InvariantCulture, format, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11);
        }

        public static bool Equals(string leftName, string rightName) => string.Compare(leftName, rightName, StringComparison.Ordinal) == 0;

        public static bool EqualsI(string leftName, string rightName) => string.Compare(leftName, rightName, StringComparison.OrdinalIgnoreCase) == 0;

        public static bool StartsWith(string valueName, string prefixName) => valueName.StartsWith(prefixName, StringComparison.Ordinal);

        public static bool StartsWithI(string valueName, string prefixName) => valueName.StartsWith(prefixName, StringComparison.OrdinalIgnoreCase);

        public static bool EndsWith(string valueName, string suffixName) => valueName.EndsWith(suffixName, StringComparison.Ordinal);

        public static bool EndsWithI(string valueName, string suffixName) => valueName.EndsWith(suffixName, StringComparison.OrdinalIgnoreCase);

        public static string ValueToString(ushort v, string formatName) => v.ToString(formatName, CultureInfo.InvariantCulture);

        public static IEqualityComparer<string> OrdinalIgnoreCaseComparer => OrdinalIgnoreCaseStringComparer.Instance;

        public static IEqualityComparer<string> OrdinalComparer => OrdinalStringComparer.Instance;

        public class OrdinalStringComparer : IEqualityComparer<string>
        {
            private static InvariantString.OrdinalStringComparer _instance;

            private OrdinalStringComparer()
            {
            }

            public static InvariantString.OrdinalStringComparer Instance
            {
                get
                {
                    if (_instance == null)
                        _instance = new InvariantString.OrdinalStringComparer();
                    return _instance;
                }
            }

            public int Compare(string x, string y) => string.Compare(x, y, StringComparison.Ordinal);

            public int GetHashCode(string obj) => obj.GetHashCode();

            public bool Equals(string x, string y) => this.Compare(x, y) == 0;
        }

        public class OrdinalIgnoreCaseStringComparer : IEqualityComparer<string>
        {
            private static InvariantString.OrdinalIgnoreCaseStringComparer _instance;

            private OrdinalIgnoreCaseStringComparer()
            {
            }

            public static InvariantString.OrdinalIgnoreCaseStringComparer Instance
            {
                get
                {
                    if (_instance == null)
                        _instance = new InvariantString.OrdinalIgnoreCaseStringComparer();
                    return _instance;
                }
            }

            public int Compare(string x, string y) => string.Compare(x, y, StringComparison.OrdinalIgnoreCase);

            public int GetHashCode(string obj) => obj.ToLowerInvariant().GetHashCode();

            public bool Equals(string x, string y) => this.Compare(x, y) == 0;
        }
    }
}
