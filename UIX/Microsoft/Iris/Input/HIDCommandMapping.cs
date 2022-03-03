// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Input.HIDCommandMapping
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Input
{
    internal class HIDCommandMapping
    {
        private static readonly HIDCommandMapping[] s_mappings = new HIDCommandMapping[41]
        {
      new HIDCommandMapping(CommandCode.Guide, 141U, 12U),
      new HIDCommandMapping(CommandCode.ChannelUp, 156U, 12U),
      new HIDCommandMapping(CommandCode.ChannelDown, 157U, 12U),
      new HIDCommandMapping(CommandCode.Print, 520U, 12U),
      new HIDCommandMapping(CommandCode.Details, 521U, 12U),
      new HIDCommandMapping(CommandCode.Home, 13U, 65468U),
      new HIDCommandMapping(CommandCode.DVDMenu, 36U, 65468U),
      new HIDCommandMapping(CommandCode.LiveTV, 37U, 65468U),
      new HIDCommandMapping(CommandCode.AdvanceZoomMode, 39U, 65468U),
      new HIDCommandMapping(CommandCode.OnlineSpotlight, 60U, 65468U),
      new HIDCommandMapping(CommandCode.OEMExtensibilityLaunch1, 61U, 65468U),
      new HIDCommandMapping(CommandCode.ClosedCaption, 43U, 65468U),
      new HIDCommandMapping(CommandCode.MyTV, 70U, 65468U),
      new HIDCommandMapping(CommandCode.MyMusic, 71U, 65468U),
      new HIDCommandMapping(CommandCode.MyShows, 72U, 65468U),
      new HIDCommandMapping(CommandCode.MyPictures, 73U, 65468U),
      new HIDCommandMapping(CommandCode.MyVideos, 74U, 65468U),
      new HIDCommandMapping(CommandCode.DVDAngle, 75U, 65468U),
      new HIDCommandMapping(CommandCode.DVDAudio, 76U, 65468U),
      new HIDCommandMapping(CommandCode.DVDSubtitles, 77U, 65468U),
      new HIDCommandMapping(CommandCode.MyRadio, 80U, 65468U),
      new HIDCommandMapping(CommandCode.Teletext, 90U, 65468U),
      new HIDCommandMapping(CommandCode.TeletextRed, 91U, 65468U),
      new HIDCommandMapping(CommandCode.TeletextGreen, 92U, 65468U),
      new HIDCommandMapping(CommandCode.TeletextYellow, 93U, 65468U),
      new HIDCommandMapping(CommandCode.TeletextBlue, 94U, 65468U),
      new HIDCommandMapping(CommandCode.SymphonyMessenger, 100U, 65468U),
      new HIDCommandMapping(CommandCode.Messenger, 105U, 65468U),
      new HIDCommandMapping(CommandCode.MCEPower, 112U, 65468U),
      new HIDCommandMapping(CommandCode.OEMExtensibility0, 50U, 65468U),
      new HIDCommandMapping(CommandCode.OEMExtensibility1, 51U, 65468U),
      new HIDCommandMapping(CommandCode.OEMExtensibility2, 52U, 65468U),
      new HIDCommandMapping(CommandCode.OEMExtensibility3, 53U, 65468U),
      new HIDCommandMapping(CommandCode.OEMExtensibility4, 54U, 65468U),
      new HIDCommandMapping(CommandCode.OEMExtensibility5, 55U, 65468U),
      new HIDCommandMapping(CommandCode.OEMExtensibility6, 56U, 65468U),
      new HIDCommandMapping(CommandCode.OEMExtensibility7, 57U, 65468U),
      new HIDCommandMapping(CommandCode.OEMExtensibility8, 58U, 65468U),
      new HIDCommandMapping(CommandCode.OEMExtensibility9, 128U, 65468U),
      new HIDCommandMapping(CommandCode.OEMExtensibility10, 129U, 65468U),
      new HIDCommandMapping(CommandCode.OEMExtensibility11, 111U, 65468U)
        };
        private CommandCode _command;
        private uint _usage;
        private uint _usagePage;

        private HIDCommandMapping(CommandCode command, uint usage, uint usagePage)
        {
            _command = command;
            _usage = usage;
            _usagePage = usagePage;
        }

        public static CommandCode Find(uint usage, uint usagePage)
        {
            foreach (HIDCommandMapping mapping in s_mappings)
            {
                if ((int)mapping._usage == (int)usage && (int)mapping._usagePage == (int)usagePage)
                    return mapping._command;
            }
            return CommandCode.None;
        }
    }
}
