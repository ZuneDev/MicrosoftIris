// Decompiled with JetBrains decompiler
// Type: SSVParseLib.SSYaccCache
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Collections;

namespace SSVParseLib
{
    internal class SSYaccCache : Queue
    {
        public bool hasElements() => Count != 0;

        public SSLexLexeme remove()
        {
            SSLexLexeme ssLexLexeme = null;
            if (Count != 0)
                ssLexLexeme = (SSLexLexeme)Dequeue();
            return ssLexLexeme;
        }
    }
}
