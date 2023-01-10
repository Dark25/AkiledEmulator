using SharedPacketLib;
using System;

namespace Akiled.Net
{
    public class InitialPacketParser : IDataParser, IDisposable, ICloneable
    {
        public byte[] currentData;

        public event InitialPacketParser.NoParamDelegate SwitchParserRequest;

        public void handlePacketData(byte[] packet, bool deciphered = false)
        {
            if (this.SwitchParserRequest == null)
                return;

            this.currentData = packet;
            this.SwitchParserRequest();
        }

        public void Dispose()
        {
            this.SwitchParserRequest = (InitialPacketParser.NoParamDelegate)null;
        }

        public object Clone()
        {
            return new InitialPacketParser();
        }

        public delegate void NoParamDelegate();
    }
}
