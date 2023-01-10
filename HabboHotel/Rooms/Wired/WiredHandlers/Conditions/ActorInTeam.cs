using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Games;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Data;

namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Conditions
{
    public class ActorInTeam : IWiredCondition, IWired
    {
        private Team team;
        private bool isDisposed;
        private readonly int ItemId;

        public ActorInTeam(int itemid, int TeamId)
        {
            if (TeamId < 1 || TeamId > 4)
                TeamId = 1;

            this.ItemId = itemid;

            this.team = (Team)TeamId;
            this.isDisposed = false;
        }

        public bool AllowsExecution(RoomUser user, Item TriggerItem)
        {
            if (user == null)
                return false;
            if (user.Team != team)
                return false;
            return true;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.ItemId, string.Empty, ((int)this.team).ToString(), false, null);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            dbClient.SetQuery("SELECT trigger_data FROM wired_items WHERE trigger_id = @id ");
            dbClient.AddParameter("id", this.ItemId);
            DataRow row = dbClient.GetRow();
            if (row == null)
                return;
            int number;
            bool result = Int32.TryParse(row[0].ToString(), out number);
            if (result)
            {
                this.team = (Team)number;
            }
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.WiredConditionConfigMessageComposer);
            Message.WriteBoolean(false);
            Message.WriteInteger(5);
            Message.WriteInteger(0);
            Message.WriteInteger(SpriteId);
            Message.WriteInteger(this.ItemId);
            Message.WriteString("");
            Message.WriteInteger(1);
            Message.WriteInteger((int)this.team);
            Message.WriteInteger(0);

            Message.WriteInteger(6);
            Session.SendPacket(Message);
        }

        public void DeleteFromDatabase(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = '" + this.ItemId + "'");
        }

        public void Dispose()
        {
            this.isDisposed = true;
        }

        public bool Disposed()
        {
            return this.isDisposed;
        }
    }
}
