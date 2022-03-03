// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Key
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;

namespace Microsoft.Iris.Render
{
    internal static class Key
    {
        public const string PublicKey = "0x0024000004800000940000000602000000240000525341310004000001000100b5fc90e7027f67871e773a8fde8938c81dd402ba65b9201d60593e96c492651e889cc13f1415ebb53fac1131ae0bd333c5ee6021672d9718ea31a8aebd0da0072f25d87dba6fc90ffd598ed4da35e44c398c454307e8e33b8426143daec9f596836f97c8f74750e5975c64e2189f45def46b2a2b1247adc3652bf5c308055da9";
        public const string PublicKeyToken = "31bf3856ad364e35";
        public const string PublicKeyTokenAttrib = ",PublicKeyToken=31bf3856ad364e35";
        public const string PublicKeyAttrib = ",PublicKey=0024000004800000940000000602000000240000525341310004000001000100b5fc90e7027f67871e773a8fde8938c81dd402ba65b9201d60593e96c492651e889cc13f1415ebb53fac1131ae0bd333c5ee6021672d9718ea31a8aebd0da0072f25d87dba6fc90ffd598ed4da35e44c398c454307e8e33b8426143daec9f596836f97c8f74750e5975c64e2189f45def46b2a2b1247adc3652bf5c308055da9";
        private static readonly byte[] s_arPublicKeyBinary = ComputeBinaryKey();

        public static byte[] PublicKeyBinary => (byte[])s_arPublicKeyBinary.Clone();

        private static byte[] ComputeBinaryKey()
        {
            int length = "0x0024000004800000940000000602000000240000525341310004000001000100b5fc90e7027f67871e773a8fde8938c81dd402ba65b9201d60593e96c492651e889cc13f1415ebb53fac1131ae0bd333c5ee6021672d9718ea31a8aebd0da0072f25d87dba6fc90ffd598ed4da35e44c398c454307e8e33b8426143daec9f596836f97c8f74750e5975c64e2189f45def46b2a2b1247adc3652bf5c308055da9".Length / 2 - 1;
            byte[] numArray = new byte[length];
            int index1 = 2;
            int index2 = 0;
            while (index2 < length)
            {
                numArray[index2] = (byte)(ParseNibble("0x0024000004800000940000000602000000240000525341310004000001000100b5fc90e7027f67871e773a8fde8938c81dd402ba65b9201d60593e96c492651e889cc13f1415ebb53fac1131ae0bd333c5ee6021672d9718ea31a8aebd0da0072f25d87dba6fc90ffd598ed4da35e44c398c454307e8e33b8426143daec9f596836f97c8f74750e5975c64e2189f45def46b2a2b1247adc3652bf5c308055da9"[index1]) << 4 | ParseNibble("0x0024000004800000940000000602000000240000525341310004000001000100b5fc90e7027f67871e773a8fde8938c81dd402ba65b9201d60593e96c492651e889cc13f1415ebb53fac1131ae0bd333c5ee6021672d9718ea31a8aebd0da0072f25d87dba6fc90ffd598ed4da35e44c398c454307e8e33b8426143daec9f596836f97c8f74750e5975c64e2189f45def46b2a2b1247adc3652bf5c308055da9"[index1 + 1]));
                ++index2;
                index1 += 2;
            }
            return numArray;
        }

        private static uint ParseNibble(char c)
        {
            if (c >= '0' && c <= '9')
                return c - 48U;
            if (c >= 'a' && c <= 'f')
                return (uint)(10 + (c - 97));
            if (c >= 'A' && c <= 'F')
                return (uint)(10 + (c - 65));
            Debug2.Throw(false, "invalid number format");
            return 0;
        }
    }
}
