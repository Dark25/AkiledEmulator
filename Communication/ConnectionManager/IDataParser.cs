using System;

namespace SharedPacketLib
{
    public interface IDataParser : IDisposable, ICloneable
    {
        void handlePacketData(byte[] packet, bool deciphered = false);
    }
}
