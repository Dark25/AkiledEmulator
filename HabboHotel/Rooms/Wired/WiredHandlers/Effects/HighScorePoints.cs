using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Effects
{
    public class HighScorePoints : IWired, IWiredEffect
    {
        private readonly Item item;

        public HighScorePoints(Item item)
        {
            this.item = item;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (user == null || user.IsBot || user.GetClient() == null)
                return;
            Dictionary<string, int> Scores = this.item.Scores;

            List<string> ListUsernameScore = new List<string>() { user.GetUsername() };

            if (Scores.ContainsKey(ListUsernameScore[0]))
            {
                if (user.WiredPoints > Scores[ListUsernameScore[0]])
                    Scores[ListUsernameScore[0]] = user.WiredPoints;
            }
            else
            {
                Scores.Add(ListUsernameScore[0], user.WiredPoints);
            }

            Room room = this.item.GetRoom();
            if (room == null)
                return;

            room.SendPacket(new ObjectUpdateComposer(this.item, room.RoomData.OwnerId));

        }

        public void Dispose()
        {
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                this.SaveToDatabase(queryreactor);
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            string triggerdata = "";

            int i = 0;
            foreach (KeyValuePair<string, int> score in this.item.Scores.OrderByDescending(x => x.Value).Take(20))
            {
                if (i != 0)
                    triggerdata += ";";

                triggerdata += score.Key + ":" + score.Value;

                i++;
            }

            WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, string.Empty, triggerdata, false, null);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            string triggerdata = null;

            dbClient.SetQuery("SELECT trigger_data FROM wired_items WHERE trigger_id = @id");
            dbClient.AddParameter("id", this.item.Id);
            DataRow row = dbClient.GetRow();
            if (row != null)
            {
                triggerdata = row["trigger_data"].ToString();
            }

            if (string.IsNullOrEmpty(triggerdata))
                return;

            foreach (string score in triggerdata.Split(';'))
            {
                string[] score2 = score.Split(':');
                int ScoreNum = 0;
                int.TryParse(score2[score2.Count() - 1], out ScoreNum);
                string Pseudo = "";
                for (int i = 0; i < score2.Count() - 1; i++)
                {
                    if (i == 0)
                        Pseudo = score2[i];
                    else
                        Pseudo += ':' + score2[i];
                }

                //List<string> ListUsernameScore = new List<string>() { score2[0] };
                if (!this.item.Scores.ContainsKey(Pseudo))
                    this.item.Scores.Add(Pseudo, ScoreNum);
            }
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            int NumMode = 0;

            int.TryParse(this.item.ExtraData, out NumMode);

            if (NumMode != 1)
                NumMode = 1;
            else
                NumMode = 0;

            this.item.ExtraData = NumMode.ToString();
            this.item.UpdateState(false, true);
        }

        public void DeleteFromDatabase(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = '" + this.item.Id + "'");
        }
    }
}
