// Decompiled with JetBrains decompiler
// Type: UIXControls.RegistryHelper
// Assembly: UIXControls, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: 78800EA5-2757-404C-BA30-C33FCFC2852A
// Assembly location: C:\Program Files\Zune\UIXcontrols.dll

using System;
using System.Collections;
using System.Globalization;
using System.Text;
using Microsoft.Iris.Data.Registry;

#nullable disable
namespace UIXControls
{
    public class RegistryHelper
    {
        private static string s_settingsRegistryPath;

        public static string SettingsRegistryPath
        {
            get => s_settingsRegistryPath;
            set => s_settingsRegistryPath = value;
        }

        // Opens (and closes) the key fresh per call rather than caching a
        // handle, matching how Registry.GetValue/SetValue behaved before this
        // was ported to IRegistryProvider — SettingsRegistryPath can change at
        // any time, and the real registry has no cost advantage to keeping a
        // handle open.
        private static IRegistryProvider OpenForWrite() =>
            string.IsNullOrEmpty(SettingsRegistryPath) ? null : RegistryProviderFactory.TryOpen(SettingsRegistryPath, writable: true, createIfMissing: true);

        private static IRegistryProvider OpenForRead() =>
            string.IsNullOrEmpty(SettingsRegistryPath) ? null : RegistryProviderFactory.TryOpen(SettingsRegistryPath, writable: false, createIfMissing: false);

        public static void SaveString(string keyName, string value)
        {
            using IRegistryProvider key = OpenForWrite();
            key?.SetStringValue(keyName, value);
        }

        public static string GetString(string keyName, string defaultValue)
        {
            using IRegistryProvider key = OpenForRead();
            return key?.GetStringValue(keyName, defaultValue) ?? defaultValue;
        }

        public static void SaveInt(string keyName, int value)
        {
            using IRegistryProvider key = OpenForWrite();
            key?.SetIntValue(keyName, value);
        }

        public static int GetInt(string keyName, int min, int max, int defaultValue)
        {
            if (string.IsNullOrEmpty(keyName))
                return defaultValue;
            using IRegistryProvider key = OpenForRead();
            if (key == null)
                return defaultValue;
            int num = key.GetIntValue(keyName, defaultValue);
            return num < min || num > max ? defaultValue : num;
        }

        private static void SaveList(string keyName, IList values, RegistryHelper.ToStringer toString)
        {
            using IRegistryProvider key = OpenForWrite();
            if (key == null)
                return;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (object obj in (IEnumerable)values)
            {
                if (stringBuilder.Length > 0)
                    stringBuilder.Append(';');
                stringBuilder.Append(toString(obj));
            }
            key.SetStringValue(keyName, stringBuilder.ToString());
        }

        public static void SaveIntList(string keyName, IList values)
        {
            SaveList(keyName, values, value => ((int)value).ToString(NumberFormatInfo.InvariantInfo));
        }

        public static void SaveFloatList(string keyName, IList values)
        {
            SaveList(keyName, values, value => ((float)value).ToString(NumberFormatInfo.InvariantInfo));
        }

        private static IList GetList(
          string keyName,
          int expectedCount,
          RegistryHelper.TryParser tryParse)
        {
            using IRegistryProvider key = OpenForRead();
            string str = key?.GetStringValue(keyName, null);
            if (string.IsNullOrEmpty(str))
                return null;
            string[] strArray = str.Split(';');
            if (strArray.Length != expectedCount)
                return null;
            ArrayList list = new ArrayList(expectedCount);
            for (int index = 0; index < expectedCount; ++index)
            {
                object obj;
                if (!tryParse(strArray[index], out obj))
                    return null;
                list.Add(obj);
            }
            return list;
        }

        public static IList GetIntList(string keyName, int expectedCount)
        {
            return GetList(keyName, expectedCount, (string s, out object value) =>
            {
                int result;
                bool intList = int.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result);
                value = result;
                return intList;
            });
        }

        public static IList GetPositiveIntList(string keyName, int expectedCount)
        {
            IList positiveIntList = GetIntList(keyName, expectedCount);
            if (positiveIntList != null)
            {
                foreach (int num in (IEnumerable)positiveIntList)
                {
                    if (num <= 0)
                    {
                        positiveIntList = null;
                        break;
                    }
                }
            }
            return positiveIntList;
        }

        public static IList GetReorderedIntList(string keyName, int expectedCount)
        {
            IList reorderedIntList = GetIntList(keyName, expectedCount);
            if (reorderedIntList != null)
            {
                BitArray bitArray = new BitArray(expectedCount);
                foreach (int index in (IEnumerable)reorderedIntList)
                {
                    if (index < 0 || index >= expectedCount || bitArray[index])
                    {
                        reorderedIntList = null;
                        break;
                    }
                    bitArray[index] = true;
                }
            }
            return reorderedIntList;
        }

        public static IList GetFloatList(string keyName, int expectedCount)
        {
            return GetList(keyName, expectedCount, (string s, out object value) =>
            {
                float result;
                bool floatList = float.TryParse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out result);
                value = result;
                return floatList;
            });
        }

        public static IList GetPositionList(string keyName, int expectedCount)
        {
            IList positionList = GetFloatList(keyName, expectedCount);
            if (positionList != null)
            {
                float num1 = 0.0f;
                foreach (float num2 in (IEnumerable)positionList)
                {
                    if ((double)num2 < (double)num1 || (double)num2 > 1.0)
                    {
                        positionList = null;
                        break;
                    }
                    num1 = num2;
                }
            }
            return positionList;
        }

        private delegate string ToStringer(object value);

        private delegate bool TryParser(string s, out object value);
    }
}
