// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Data.StringUtility
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Text;

namespace Microsoft.Iris.Data
{
    internal static class StringUtility
    {
        public static string[] SplitAndTrim(char separator, string splitee)
        {
            string[] strArray = splitee.Split(separator);
            for (int index = 0; index < strArray.Length; ++index)
                strArray[index] = strArray[index].Trim();
            return strArray;
        }

        public static string Unescape(string source, out int errorIndex, out string invalidSequence)
        {
            invalidSequence = null;
            errorIndex = -1;
            if (source.IndexOf('\\') == -1)
                return source;
            StringBuilder stringBuilder = new StringBuilder(source.Length);
            int index = 0;
            int startIndex = 0;
            int length = source.Length;
            bool flag = true;
            while (flag && index < length)
            {
                startIndex = index;
                char ch1 = source[index++];
                if (ch1 != '\\')
                {
                    stringBuilder.Append(ch1);
                }
                else
                {
                    if (index == length)
                    {
                        flag = false;
                        break;
                    }
                    char mode = source[index++];
                    char ch2 = char.MinValue;
                    char ch3 = char.MinValue;
                    switch (mode)
                    {
                        case '"':
                            ch2 = '"';
                            break;
                        case '\'':
                            ch2 = '\'';
                            break;
                        case '0':
                            ch2 = char.MinValue;
                            break;
                        case 'U':
                        case 'u':
                        case 'x':
                            uint result;
                            flag = ReadHexSequence(mode, source, ref index, length, out result);
                            if (flag)
                            {
                                if (result <= ushort.MaxValue)
                                {
                                    ch2 = (char)result;
                                    break;
                                }
                                if (result <= 1114111U)
                                {
                                    ch2 = (char)((result - 65536U) / 1024U + 55296U);
                                    ch3 = (char)((result - 65536U) % 1024U + 56320U);
                                    break;
                                }
                                flag = false;
                                break;
                            }
                            break;
                        case '\\':
                            ch2 = '\\';
                            break;
                        case 'a':
                            ch2 = '\a';
                            break;
                        case 'b':
                            ch2 = '\b';
                            break;
                        case 'f':
                            ch2 = '\f';
                            break;
                        case 'n':
                            ch2 = '\n';
                            break;
                        case 'r':
                            ch2 = '\r';
                            break;
                        case 't':
                            ch2 = '\t';
                            break;
                        case 'v':
                            ch2 = '\v';
                            break;
                        default:
                            flag = false;
                            break;
                    }
                    if (flag)
                    {
                        stringBuilder.Append(ch2);
                        if (ch3 != char.MinValue)
                            stringBuilder.Append(ch3);
                    }
                }
            }
            string str;
            if (!flag)
            {
                str = null;
                errorIndex = startIndex;
                invalidSequence = source.Substring(startIndex, index - startIndex);
            }
            else
            {
                str = stringBuilder.ToString();
                errorIndex = -1;
            }
            return str;
        }

        private static bool ReadHexSequence(
          char mode,
          string source,
          ref int index,
          int indexLast,
          out uint result)
        {
            int num1;
            int num2;
            switch (mode)
            {
                case 'u':
                    num1 = 4;
                    num2 = 4;
                    break;
                case 'x':
                    num1 = 1;
                    num2 = 4;
                    break;
                default:
                    num1 = 8;
                    num2 = 8;
                    break;
            }
            int num3 = 0;
            uint num4 = 0;
            for (; index < indexLast && num3 < num2; ++num3)
            {
                char ch = source[index];
                uint num5;
                if (ch >= '0' && ch <= '9')
                    num5 = ch - 48U;
                else if (ch >= 'a' && ch <= 'f')
                    num5 = (uint)(10 + (ch - 97));
                else if (ch >= 'A' && ch <= 'F')
                    num5 = (uint)(10 + (ch - 65));
                else
                    break;
                num4 = (num4 << 4) + num5;
                ++index;
            }
            bool flag = num3 >= num1;
            result = flag ? num4 : 0U;
            return flag;
        }
    }
}
