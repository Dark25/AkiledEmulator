namespace Akiled.Communication.Packets.Outgoing.Structure
{
    internal class UserNameChangeMessageComposer : ServerPacket
    {
        public UserNameChangeMessageComposer(int RoomId, int VirtualId, string Username)
          : base(2182)
        {
            this.WriteInteger(RoomId);
            this.WriteInteger(VirtualId);
            this.WriteString(Username);
        }
    }
}
