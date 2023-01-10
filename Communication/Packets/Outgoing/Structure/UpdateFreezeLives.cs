namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class UpdateFreezeLives : ServerPacket
    {
        public UpdateFreezeLives()
            : base(ServerPacketHeader.FreezeLivesComposer)
        {

        }
    }
}
