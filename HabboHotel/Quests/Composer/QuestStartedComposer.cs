using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Quests.Composer
{
    public class QuestStartedComposer
    {
        public static ServerPacket Compose(GameClient Session, Quest Quest)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.QuestStartedMessageComposer);
            QuestListComposer.SerializeQuest(Message, Session, Quest, Quest.Category);
            return Message;
        }
    }
}
