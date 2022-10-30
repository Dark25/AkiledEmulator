using Akiled.Communication.Packets.Outgoing;

namespace Akiled.HabboHotel.Quests.Composer
{
    public class QuestAbortedComposer
    {
        public static ServerPacket Compose()
        {
            ServerPacket serverMessage = new ServerPacket(ServerPacketHeader.QuestAbortedMessageComposer);
            serverMessage.WriteBoolean(false);
            return serverMessage;
        }
    }
}
