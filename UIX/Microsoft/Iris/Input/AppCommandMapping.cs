// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Input.AppCommandMapping
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Input
{
    internal class AppCommandMapping
    {
        private AppCommandMapping()
        {
        }

        public static CommandCode Find(uint appCommand)
        {
            CommandCode commandCode = CommandCode.None;
            switch (appCommand)
            {
                case 1:
                    commandCode = CommandCode.Back;
                    break;
                case 11:
                    commandCode = CommandCode.SkipForward;
                    break;
                case 12:
                    commandCode = CommandCode.SkipBack;
                    break;
                case 13:
                    commandCode = CommandCode.Stop;
                    break;
                case 14:
                    commandCode = CommandCode.PlayPause;
                    break;
                case 46:
                    commandCode = CommandCode.Play;
                    break;
                case 47:
                    commandCode = CommandCode.Pause;
                    break;
                case 48:
                    commandCode = CommandCode.Record;
                    break;
                case 49:
                    commandCode = CommandCode.FF;
                    break;
                case 50:
                    commandCode = CommandCode.Rewind;
                    break;
            }
            return commandCode;
        }
    }
}
