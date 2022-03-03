// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Animations.StopCommandSet
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Text;

namespace Microsoft.Iris.Animations
{
    internal class StopCommandSet
    {
        private StopCommand[] _stopCommandsList;

        public StopCommandSet(StopCommand defaultCommand)
        {
            _stopCommandsList = new StopCommand[20];
            for (uint index = 0; index < _stopCommandsList.Length; ++index)
                _stopCommandsList[index] = defaultCommand;
        }

        public StopCommand this[AnimationType type]
        {
            get => _stopCommandsList[(uint)type];
            set => _stopCommandsList[(uint)type] = value;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("StopCommandSet(");
            int num = 0;
            for (uint index = 0; index < _stopCommandsList.Length; ++index)
            {
                StopCommand stopCommands = _stopCommandsList[index];
                if (num > 0)
                    stringBuilder.Append(", ");
                ++num;
                stringBuilder.Append((AnimationType)index);
                stringBuilder.Append(":");
                stringBuilder.Append(stopCommands);
            }
            stringBuilder.Append(")");
            return stringBuilder.ToString();
        }
    }
}
