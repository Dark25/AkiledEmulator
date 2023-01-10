namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class AvatarEffectExpiredMessageComposer : ServerPacket
    {
        public AvatarEffectExpiredMessageComposer()
            : base(ServerPacketHeader.AvatarEffectExpiredMessageComposer)
        {

        }
    }
}
