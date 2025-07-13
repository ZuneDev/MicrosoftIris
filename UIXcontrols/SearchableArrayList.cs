// Decompiled with JetBrains decompiler
// Type: UIXControls.SearchableArrayList
// Assembly: UIXControls, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: 78800EA5-2757-404C-BA30-C33FCFC2852A
// Assembly location: C:\Program Files\Zune\UIXcontrols.dll

using Microsoft.Iris;
using System;
using System.Collections;

#nullable disable
namespace UIXControls
{
    public class SearchableArrayList : ArrayListDataSet, ISearchableList
    {
        int ISearchableList.SearchForString(string str)
        {
            if (Source is ArrayList source)
            {
                for (int index = 0; index < source.Count; ++index)
                {
                    string str1 = source[index].ToString();
                    if (str.Length <= str1.Length && string.Compare(str1.Substring(0, str.Length), str, StringComparison.OrdinalIgnoreCase) == 0)
                        return index;
                }
            }
            return -1;
        }
    }
}
