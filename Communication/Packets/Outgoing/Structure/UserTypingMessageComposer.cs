using System.Net.NetworkInformation;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class UserTypingMessageComposer : ServerPacket
    {
        public UserTypingMessageComposer(int virtualId, bool typing)
            : base(ServerPacketHeader.UserTypingMessageComposer)
        {
            this.WriteInteger(virtualId);
            this.WriteInteger(typing ? 1 : 0);
        }
    }
}
