// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.LoadResultCache
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using System.Collections.Generic;

namespace Microsoft.Iris.Markup
{
    public static class LoadResultCache
    {
        private static Dictionary<string, LoadResult> s_cache = new Dictionary<string, LoadResult>(InvariantString.OrdinalIgnoreCaseComparer);

        public static LoadResult Read(string uri)
        {
            LoadResult loadResult;
            s_cache.TryGetValue(uri, out loadResult);
            return loadResult;
        }

        public static void Write(string uri, LoadResult loadResult)
        {
            s_cache[uri] = loadResult;
            loadResult.RegisterUsage(s_cache);
        }

        public static void Remove(uint islandId)
        {
            Vector<string> vector = new Vector<string>();
            foreach (KeyValuePair<string, LoadResult> keyValuePair in s_cache)
            {
                LoadResult loadResult = keyValuePair.Value;
                if ((loadResult.IslandReferences & islandId) > 0U)
                    loadResult.RemoveReferenceDeep(islandId);
                if (loadResult.IslandReferences == 0U)
                {
                    loadResult.UnregisterUsage(s_cache);
                    vector.Add(keyValuePair.Key);
                }
            }
            foreach (string key in vector)
                s_cache.Remove(key);
        }

        public static void Clear()
        {
            foreach (LoadResult loadResult in s_cache.Values)
            {
                loadResult.RemoveAllReferences();
                loadResult.UnregisterUsage(s_cache);
            }
            s_cache.Clear();
        }
    }
}
