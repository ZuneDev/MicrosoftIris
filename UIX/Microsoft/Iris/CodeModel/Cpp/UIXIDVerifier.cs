// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.CodeModel.Cpp.UIXIDVerifier
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Session;

namespace Microsoft.Iris.CodeModel.Cpp
{
    internal class UIXIDVerifier
    {
        private Map<uint, uint> _uniqueIDs;
        private DllLoadResult _loadResult;

        public UIXIDVerifier(DllLoadResult loadResult)
        {
            _loadResult = loadResult;
            _uniqueIDs = new Map<uint, uint>();
        }

        public bool RegisterID(uint ID) => CheckForSchemaMatch(ID) && CheckForDuplicate(ID);

        private bool CheckForSchemaMatch(uint ID)
        {
            bool flag = (int)UIXID.GetSchemaComponent(ID) == _loadResult.SchemaComponent;
            if (!flag)
                ErrorManager.ReportError("Schema component on ID '0x{0:X8}' doesn't match schema's ID '0x{1:X8}' on '{2}'", ID, _loadResult.SchemaComponent, _loadResult.Uri);
            return flag;
        }

        private bool CheckForDuplicate(uint ID)
        {
            uint num = 0;
            bool flag = _uniqueIDs.TryGetValue(ID, out num);
            if (flag && num == 0U)
            {
                ErrorManager.ReportError("Duplicate ID '0x{0:X8}' found in schema from '{1}'", ID, _loadResult.Uri);
                ++num;
            }
            _uniqueIDs[ID] = num;
            return !flag;
        }
    }
}
