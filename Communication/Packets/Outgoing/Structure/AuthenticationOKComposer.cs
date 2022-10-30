namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class AuthenticationOKComposer : ServerPacket
    {
        public AuthenticationOKComposer()
            : base(ServerPacketHeader.AuthenticationOKMessageComposer)
        {

        }
    }
}
