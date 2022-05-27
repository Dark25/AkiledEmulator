namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class BuildersClubMembershipComposer : ServerPacket
    {
        public BuildersClubMembershipComposer()
            : base(ServerPacketHeader.BuildersClubMembershipMessageComposer)
        {
            WriteInteger(99999999);
            WriteInteger(100);
            WriteInteger(2);
            WriteInteger(99999999);
        }
    }
}
