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
    public class GiveScore : IWiredEffect, IWired
    {
        private int maxCountPerGame;
        private int currentGameCount;
        private int scoreToGive;
        private GameManager gameManager;
        private RoomEventDelegate delegateFunction;
        private readonly int itemID;

        public GiveScore(int maxCountPerGame, int scoreToGive, GameManager gameManager, int itemID)
        {
            this.maxCountPerGame = maxCountPerGame;
            this.currentGameCount = 0;
            this.scoreToGive = scoreToGive;
            this.delegateFunction = new RoomEventDelegate(this.gameManager_OnGameStart);
            this.gameManager = gameManager;
            this.itemID = itemID;
            gameManager.OnGameStart += this.delegateFunction;
        }

        private void gameManager_OnGameStart(object sender, EventArgs e)
        {
            this.currentGameCount = 0;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (user == null || user.Team == Team.none || this.maxCountPerGame <= this.currentGameCount)
                return;
            this.currentGameCount++;
            this.gameManager.AddPointToTeam(user.Team, this.scoreToGive, user);
        }

        public void Dispose()
        {
            this.gameManager.OnGameStart -= this.delegateFunction;
            this.gameManager = (GameManager)null;
            this.delegateFunction = (RoomEventDelegate)null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.itemID, this.scoreToGive.ToString(), this.maxCountPerGame.ToString(), false, null);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            dbClient.SetQuery("SELECT trigger_data, trigger_data_2 FROM wired_items WHERE trigger_id = @id ");
            dbClient.AddParameter("id", this.itemID);
            DataRow row = dbClient.GetRow();
            if (row != null)
            {
                this.maxCountPerGame = Convert.ToInt32(row[0].ToString());
                this.scoreToGive = Convert.ToInt32(row[1].ToString());
            }
            else
            {
                this.maxCountPerGame = 0;
                this.scoreToGive = 0;
            }
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message11 = new ServerPacket(ServerPacketHeader.WiredEffectConfigMessageComposer);
            Message11.WriteBoolean(false);
            Message11.WriteInteger(5);
            Message11.WriteInteger(0);
            Message11.WriteInteger(SpriteId);
            Message11.WriteInteger(this.itemID);
            Message11.WriteString("");
            Message11.WriteInteger(2);
            Message11.WriteInteger(this.scoreToGive);
            Message11.WriteInteger(this.maxCountPerGame);
            Message11.WriteInteger(0);
            Message11.WriteInteger(6);
            Message11.WriteInteger(0);
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
