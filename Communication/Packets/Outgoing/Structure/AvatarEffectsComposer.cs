using System.Collections.Generic;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class AvatarEffectsComposer : ServerPacket
    {
        public AvatarEffectsComposer(List<int> Enable)
            : base(ServerPacketHeader.AvatarEffectsMessageComposer)
        {
            WriteInteger(Enable.Count);

            foreach (int EffectId in Enable)
            {
                WriteInteger(EffectId);//Effect Id
                WriteInteger(1);//Type, 0 = Hand, 1 = Full
                WriteInteger(0);
                WriteInteger(1);
                WriteInteger(-1);
                WriteBoolean(true);//Permanent
            }
        }
    }
}
