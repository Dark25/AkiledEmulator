namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class AvatarEffectComposer : ServerPacket
    {
        public AvatarEffectComposer(int playerID, int effectID)
            : base(ServerPacketHeader.AvatarEffectMessageComposer)
        {
            WriteInteger(playerID);
            WriteInteger(effectID);
            WriteInteger(0);
        }
    }
}
