namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class OnGuideSessionInvitedToGuideRoom : ServerPacket
    {
        public OnGuideSessionInvitedToGuideRoom()
            : base(ServerPacketHeader.OnGuideSessionInvitedToGuideRoom)
        {

        }
    }
}
