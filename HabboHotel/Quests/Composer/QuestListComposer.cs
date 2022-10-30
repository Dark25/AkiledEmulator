using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.GameClients;
using System.Collections.Generic;

namespace Akiled.HabboHotel.Quests.Composer
{
    public class QuestListComposer
    {
        public static ServerPacket Compose(GameClient Session, List<Quest> Quests, bool Send)
        {
            Dictionary<string, int> dictionary1 = new Dictionary<string, int>();
            Dictionary<string, Quest> dictionary2 = new Dictionary<string, Quest>();
            foreach (Quest quest in Quests)
            {
                if (!dictionary1.ContainsKey(quest.Category))
                {
                    dictionary1.Add(quest.Category, 1);
                    dictionary2.Add(quest.Category, (Quest)null);
                }
                if (quest.Number >= dictionary1[quest.Category])
                {
                    int questProgress = Session.GetHabbo().GetQuestProgress(quest.Id);
                    if (Session.GetHabbo().CurrentQuestId != quest.Id && (long)questProgress >= (long)quest.GoalData)
                        dictionary1[quest.Category] = quest.Number + 1;
                }
            }
            foreach (Quest quest in Quests)
            {
                foreach (KeyValuePair<string, int> keyValuePair in dictionary1)
                {
                    if (quest.Category == keyValuePair.Key && quest.Number == keyValuePair.Value)
                    {
                        dictionary2[keyValuePair.Key] = quest;
                        break;
                    }
                }
            }
            ServerPacket Message = new ServerPacket(ServerPacketHeader.QuestListMessageComposer);
            Message.WriteInteger(dictionary2.Count);
            foreach (KeyValuePair<string, Quest> keyValuePair in dictionary2)
            {
                if (keyValuePair.Value != null)
                    SerializeQuest(Message, Session, keyValuePair.Value, keyValuePair.Key);
            }
            foreach (KeyValuePair<string, Quest> keyValuePair in dictionary2)
            {
                if (keyValuePair.Value == null)
                    SerializeQuest(Message, Session, keyValuePair.Value, keyValuePair.Key);
            }
            Message.WriteBoolean(Send);
            return Message;
        }

        public static void SerializeQuest(ServerPacket Message, GameClient Session, Quest Quest, string Category)
        {
            int questsInCategory = AkiledEnvironment.GetGame().GetQuestManager().GetAmountOfQuestsInCategory(Category);
            int i = Quest == null ? questsInCategory : Quest.Number - 1;
            int num = Quest == null ? 0 : Session.GetHabbo().GetQuestProgress(Quest.Id);
            if (Quest != null && Quest.IsCompleted(num))
                ++i;
            Message.WriteString(Category);
            Message.WriteInteger(i);
            Message.WriteInteger(questsInCategory);
            Message.WriteInteger(0);
            Message.WriteInteger(Quest == null ? 0 : Quest.Id);
            Message.WriteBoolean(Quest != null && Session.GetHabbo().CurrentQuestId == Quest.Id);
            Message.WriteString(Quest == null ? string.Empty : Quest.ActionName);
            Message.WriteString(Quest == null ? string.Empty : Quest.DataBit);
            Message.WriteInteger(Quest == null ? 0 : Quest.Reward);
            Message.WriteString(Quest == null ? string.Empty : Quest.Name);
            Message.WriteInteger(num);
            Message.WriteInteger(Quest == null ? 0 : Quest.GoalData);
            Message.WriteInteger(GetIntValue(Category));
            Message.WriteString("set_kuurna");
            Message.WriteString("MAIN_CHAIN");
            Message.WriteBoolean(true);
        }

        public static int GetIntValue(string QuestCategory)
        {
            switch (QuestCategory)
            {
                case "room_builder":
                    return 2;
                case "social":
                    return 3;
                case "identity":
                    return 4;
                case "explore":
                    return 5;
                case "battleball":
                    return 7;
                case "freeze":
                    return 8;
                default:
                    return 0;
            }
        }
    }
}
