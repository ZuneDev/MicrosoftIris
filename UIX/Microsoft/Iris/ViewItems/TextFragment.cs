// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ViewItems.TextFragment
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Collections;

namespace Microsoft.Iris.ViewItems
{
    internal class TextFragment
    {
        private string _tagName;
        private IDictionary _attributes;
        private ArrayList _runs;
        private Text _textViewItem;

        public TextFragment(string tagName, IDictionary attributes, Text textViewItem)
        {
            _tagName = tagName;
            _attributes = attributes;
            _textViewItem = textViewItem;
        }

        public IList Runs => _runs;

        public string TagName => _tagName;

        public string Content
        {
            get
            {
                string str = null;
                if (_runs != null)
                {
                    foreach (TextRunData run in _runs)
                        str = str != null ? str + run.Run.Content : run.Run.Content;
                }
                return str;
            }
        }

        public IDictionary Attributes => _attributes;

        internal ArrayList InternalRuns
        {
            get
            {
                if (_runs == null)
                    _runs = new ArrayList();
                return _runs;
            }
        }

        public bool IsLayoutEquivalentTo(TextFragment rhs)
        {
            if (rhs == null || TagName != rhs.TagName || Attributes != rhs.Attributes)
                return false;
            int num1 = _runs != null ? _runs.Count : 0;
            int num2 = rhs._runs != null ? rhs._runs.Count : 0;
            if (num1 != num2)
                return false;
            for (int index = 0; index < num1; ++index)
            {
                if (!AreRunsLayoutEquivalent((TextRunData)_runs[index], (TextRunData)rhs._runs[index]))
                    return false;
            }
            return true;
        }

        private bool AreRunsLayoutEquivalent(TextRunData lhs, TextRunData rhs) => rhs == null && lhs == null || rhs != null && lhs != null && (!(lhs.Position != rhs.Position) && !(lhs.Size != rhs.Size)) && !(lhs.Run.Content != rhs.Run.Content);

        public void NotifyPaintInvalid()
        {
            if (_runs == null)
                return;
            for (int index = 0; index < _runs.Count; ++index)
                ((TextRunData)_runs[index]).NotifyPaintInvalid();
        }
    }
}
