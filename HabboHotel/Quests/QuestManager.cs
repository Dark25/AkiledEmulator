using Akiled.Communication.Packets.Incoming;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Quests.Composer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Akiled.HabboHotel.Quests
{
    public class QuestManager
    {
        private Dictionary<int, Quest> quests;
        private Dictionary<string, int> questCount;

        public void Init()
        {
            this.quests = new Dictionary<int, Quest>();
            this.questCount = new Dictionary<string, int>();
            this.ReloadQuests();
        }

        public void ReloadQuests()
        {
            this.quests.Clear();
            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM quests");
                foreach (DataRow dataRow in dbClient.GetTable().Rows)
                {
                    int num1 = Convert.ToInt32(dataRow["id"]);
                    string str = (string)dataRow["category"];
                    int Number = (int)dataRow["series_number"];
                    int num2 = (int)dataRow["goal_type"];
                    int GoalData = Convert.ToInt32(dataRow["goal_data"]);
                    string Name = (string)dataRow["name"];
                    int Reward = (int)dataRow["reward"];
                    string DataBit = (string)dataRow["data_bit"];
                    Quest quest = new Quest(num1, str, Number, (QuestType)num2, GoalData, Name, Reward, DataBit);
                    this.quests.Add(num1, quest);
                    this.AddToCounter(str);
                }
            }
            Console.WriteLine("Busquedas del Hotel -> Listo!");
        }

        private void AddToCounter(string category)
        {
            int num = 0;
            if (this.questCount.TryGetValue(category, out num))
                this.questCount[category] = num + 1;
            else
                this.questCount.Add(category, 1);
        }

        public Quest GetQuest(int Id)
        {
            Quest quest = (Quest)null;
            this.quests.TryGetValue(Id, out quest);
            return quest;
        }

        public int GetAmountOfQuestsInCategory(string Category)
        {
            int num = 0;
            this.questCount.TryGetValue(Category, out num);
            return num;
        }

        public void ProgressUserQuest(GameClient Session, QuestType QuestType, int EventData = 0)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().CurrentQuestId <= 0)
                return;
            Quest quest = this.GetQuest(Session.GetHabbo().CurrentQuestId);
            if (quest == null || quest.GoalType != QuestType)
                return;
            int questProgress = Session.GetHabbo().GetQuestProgress(quest.Id);
            bool flag = false;
            int num;
            if (QuestType != QuestType.EXPLORE_FIND_ITEM)
            {
                num = questProgress + 1;
                if ((long)num >= (long)quest.GoalData)
                    flag = true;
            }
            else
            {
                if (EventData != quest.GoalData)
                    return;
                num = quest.GoalData;
                flag = true;
            }
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                queryreactor.RunQuery("UPDATE user_quests SET progress = " + num + " WHERE user_id = " + Session.GetHabbo().Id + " AND quest_id =  " + quest.Id);

            Session.GetHabbo().quests[Session.GetHabbo().CurrentQuestId] = num;
            Session.SendPacket(QuestStartedComposer.Compose(Session, quest));

            if (!flag) return;

            Session.GetHabbo().CurrentQuestId = 0;
            Session.GetHabbo().LastCompleted = quest.Id;
            Session.SendPacket(QuestCompletedComposer.Compose(Session, quest));
            Session.GetHabbo().Duckets += quest.Reward;
            Session.GetHabbo().UpdateActivityPointsBalance();
            this.GetList(Session, (ClientPacket)null);
        }

        public Quest GetNextQuestInSeries(string Category, int Number)
        {
            foreach (Quest quest in this.quests.Values)
            {
                if (quest.Category == Category && quest.Number == Number)
                    return quest;
            }
            return (Quest)null;
        }

        public void GetList(GameClient Session, ClientPacket Message)
        {
            Session.SendPacket(QuestListComposer.Compose(Session, Enumerable.ToList<Quest>((IEnumerable<Quest>)this.quests.Values), Message != null));
        }

        public void ActivateQuest(GameClient Session, ClientPacket Message)
        {
            Quest quest = this.GetQuest(Message.PopInt());
            if (quest == null)
                return;
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.RunQuery("REPLACE INTO user_quests VALUES (" + Session.GetHabbo().Id + ", " + quest.Id + ", 0)");
            }
            Session.GetHabbo().CurrentQuestId = quest.Id;
            this.GetList(Session, (ClientPacket)null);
            Session.SendPacket(QuestStartedComposer.Compose(Session, quest));
        }

        public void GetCurrentQuest(GameClient Session, ClientPacket Message)
        {
            if (!Session.GetHabbo().InRoom)
                return;
            Quest quest = this.GetQuest(Session.GetHabbo().LastCompleted);
            Quest nextQuestInSeries = this.GetNextQuestInSeries(quest.Category, quest.Number + 1);
            if (nextQuestInSeries == null)
                return;
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.RunQuery("REPLACE INTO user_quests VALUES (" + Session.GetHabbo().Id + ", " + nextQuestInSeries.Id + ", 0)");
            }
            Session.GetHabbo().CurrentQuestId = nextQuestInSeries.Id;
            this.GetList(Session, (ClientPacket)null);
            Session.SendPacket(QuestStartedComposer.Compose(Session, nextQuestInSeries));
        }

        public void CancelQuest(GameClient Session, ClientPacket Message)
        {
            Quest quest = this.GetQuest(Session.GetHabbo().CurrentQuestId);
            if (quest == null)
                return;
            Session.GetHabbo().CurrentQuestId = 0;
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.RunQuery(string.Concat(new object[4]
                {
           "DELETE FROM user_quests WHERE user_id = ",
           Session.GetHabbo().Id,
           " AND quest_id = ",
           quest.Id
                }));
            }
            Session.SendPacket(QuestAbortedComposer.Compose());
            this.GetList(Session, (ClientPacket)null);
        }
    }
}
