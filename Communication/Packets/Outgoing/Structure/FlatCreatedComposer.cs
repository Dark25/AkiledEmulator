namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class FlatCreatedComposer : ServerPacket
    {
        public FlatCreatedComposer(int roomID, string roomName)
            : base(ServerPacketHeader.FlatCreatedMessageComposer)
        {
            WriteInteger(roomID);
            WriteString(roomName);
        }
    }
}
