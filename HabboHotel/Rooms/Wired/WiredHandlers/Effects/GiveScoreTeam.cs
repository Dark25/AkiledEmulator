using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Games;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Data;

namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Effects
{
    public class GiveScoreTeam : IWiredEffect, IWired
    {
        private int maxCountPerGame;
        private int currentGameCount;
        private int scoreToGive;
        private GameManager gameManager;
        private RoomEventDelegate delegateFunction;
        private readonly int itemID;
        private Team team;

        public GiveScoreTeam(int TeamId, int maxCountPerGame, int scoreToGive, GameManager gameManager, int itemID)
        {
            if (TeamId < 1 || TeamId > 4)
                TeamId = 1;

            this.maxCountPerGame = maxCountPerGame;
            this.currentGameCount = 0;
            this.scoreToGive = scoreToGive;
            this.delegateFunction = new RoomEventDelegate(this.gameManager_OnGameStart);
            this.gameManager = gameManager;
            this.itemID = itemID;
            gameManager.OnGameStart += this.delegateFunction;
            this.team = (Team)TeamId;
        }

        private void gameManager_OnGameStart(object sender, EventArgs e)
        {
            this.currentGameCount = 0;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (this.maxCountPerGame <= this.currentGameCount)
                return;
            this.currentGameCount++;
            this.gameManager.AddPointToTeam(this.team, this.scoreToGive, user);
        }

        public void Dispose()
        {
            this.gameManager.OnGameStart -= this.delegateFunction;
            this.gameManager = (GameManager)null;
            this.delegateFunction = (RoomEventDelegate)null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.itemID, ((int)this.team).ToString(), this.maxCountPerGame.ToString() + ":" + this.scoreToGive.ToString(), false, null);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            dbClient.SetQuery("SELECT trigger_data, trigger_data_2 FROM wired_items WHERE trigger_id = @id ");
            dbClient.AddParameter("id", this.itemID);
            DataRow row = dbClient.GetRow();
            if (row == null)
                return;
            string data = row[0].ToString();
            string data2 = row[1].ToString();
            if (data.Contains(":"))
            {
                string[] datasplit = data.Split(':');
                this.maxCountPerGame = Convert.ToInt32(datasplit[0]);
                this.scoreToGive = Convert.ToInt32(datasplit[1]);
            }

            int number;
            bool result = Int32.TryParse(data2, out number);
            if (result)
            {
                this.team = (Team)number;
            }

        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message11 = new ServerPacket(ServerPacketHeader.WiredEffectConfigMessageComposer);
            Message11.WriteBoolean(false);
            Message11.WriteInteger(0);
            Message11.WriteInteger(0);
            Message11.WriteInteger(SpriteId);
            Message11.WriteInteger(this.itemID);
            Message11.WriteString("");
            Message11.WriteInteger(3);
            Message11.WriteInteger(this.scoreToGive);
            Message11.WriteInteger(this.maxCountPerGame);
            Message11.WriteInteger((int)this.team);

            Message11.WriteInteger(0);
            Message11.WriteInteger(14);
            Message11.WriteInteger(0);
            Message11.WriteInteger(0);
            Session.SendPacket(Message11);
        }

        public void DeleteFromDatabase(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = '" + this.itemID + "'");
        }
    }
}
