using System;

namespace Akiled.Utilities
{
    public class HabboEncoding
    {
        public static int DecodeInt32(byte[] v)
        {

            if ((v[0] | v[1] | v[2] | v[3]) < 0)
            {
                return 0;
            }
            return (v[0] << 24) + (v[1] << 16) + (v[2] << 8) + (v[3]);

        }

        public static Int16 DecodeInt16(byte[] v)
        {
            if ((v[0] | v[1]) < 0)
            {
                return 0;
            }
            int result = (v[0] << 8) + (v[1]);
            return (Int16)result;
        }

    }
}