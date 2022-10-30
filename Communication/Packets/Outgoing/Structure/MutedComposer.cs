using System;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    internal class MutedComposer : ServerPacket
    {
        public MutedComposer(double TimeMuted)
            : base(1092)
        {
            WriteInteger(Convert.ToInt32(TimeMuted));
        }
    }
}
