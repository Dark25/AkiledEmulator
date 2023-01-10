using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Games;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Data;

namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Triggers
{
    public class ScoreAchieved : IWired
    {
        private Item item;
        private WiredHandler handler;
        private int scoreLevel;
        //private bool used;
        private readonly TeamScoreChangedDelegate scoreChangedDelegate;
        //private RoomEventDelegate gameEndDeletgate;

        public ScoreAchieved(Item item, WiredHandler handler, int scoreLevel, GameManager gameManager)
        {
            this.item = item;
            this.handler = handler;
            this.scoreLevel = scoreLevel;
            //this.used = false;
            this.scoreChangedDelegate = new TeamScoreChangedDelegate(this.gameManager_OnScoreChanged);
            //this.gameEndDeletgate = new RoomEventDelegate(this.gameManager_OnGameEnd);
            gameManager.OnScoreChanged += this.scoreChangedDelegate;
            //gameManager.OnGameEnd += this.gameEndDeletgate;
        }

        //private void gameManager_OnGameEnd(object sender, EventArgs e)
        //{
        //this.used = false;
        //}

        private void gameManager_OnScoreChanged(object sender, TeamScoreChangedArgs e)
        {
            if (e.Points <= (this.scoreLevel - 1))// || this.used)
                return;
            //this.used = true;
            this.handler.ExecutePile(this.item.Coordinate, e.user, null);
        }

        public void Dispose()
        {
            this.handler.GetRoom().GetGameManager().OnScoreChanged -= this.scoreChangedDelegate;
            //this.handler.GetRoom().GetGameManager().OnGameEnd -= this.gameEndDeletgate;
            this.item = (Item)null;
            this.handler = (WiredHandler)null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, string.Empty, this.scoreLevel.ToString(), false, null);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            dbClient.SetQuery("SELECT trigger_data FROM wired_items WHERE trigger_id = @id ");
            dbClient.AddParameter("id", this.item.Id);
            DataRow row = dbClient.GetRow();
            if (row != null)
                this.scoreLevel = Convert.ToInt32(row[0].ToString());
            else
                this.scoreLevel = 200;
        }

        public void DeleteFromDatabase(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = '" + this.item.Id + "'");
        }
        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message7 = new ServerPacket(ServerPacketHeader.WiredTriggerConfigMessageComposer);
            Message7.WriteBoolean(false);
            Message7.WriteInteger(5);
            Message7.WriteInteger(0);
            Message7.WriteInteger(SpriteId);
            Message7.WriteInteger(this.item.Id);
            Message7.WriteString("");
            Message7.WriteInteger(1);
            Message7.WriteInteger(this.scoreLevel);
            Message7.WriteInteger(0);
            Message7.WriteInteger(10);
            Message7.WriteInteger(0);
            Message7.WriteInteger(0);
            Session.SendPacket(Message7);
        }
    }
}
