namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class AvatarEffectActivatedMessageComposer : ServerPacket
    {
        public AvatarEffectActivatedMessageComposer()
            : base(ServerPacketHeader.AvatarEffectActivatedMessageComposer)
        {

        }
    }
}
