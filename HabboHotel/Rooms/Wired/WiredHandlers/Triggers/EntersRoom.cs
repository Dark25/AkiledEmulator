using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Data;

namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Triggers
{
    public class EntersRoom : IWired
    {
        private Item item;
        private WiredHandler handler;
        private bool isOneUser;
        private string userName;
        private readonly RoomEventDelegate delegateFunction;

        public EntersRoom(Item item, WiredHandler handler, RoomUserManager roomUserManager, bool isOneUser, string userName)
        {
            this.item = item;
            this.handler = handler;
            this.isOneUser = isOneUser;
            this.userName = userName;
            this.delegateFunction = new RoomEventDelegate(this.roomUserManager_OnUserEnter);
            roomUserManager.OnUserEnter += this.delegateFunction;
        }

        private void roomUserManager_OnUserEnter(object sender, EventArgs e)
        {
            RoomUser user = (RoomUser)sender;
            if (user == null)
                return;
            if ((user.IsBot || !this.isOneUser || (string.IsNullOrEmpty(this.userName) || !(user.GetUsername() == this.userName))) && this.isOneUser)
                return;
            if (this.handler != null)
                this.handler.ExecutePile(this.item.Coordinate, user, null);
        }

        public void Dispose()
        {
            this.handler = (WiredHandler)null;
            this.userName = (string)null;
            if (this.item != null && this.item.GetRoom() != null)
                this.item.GetRoom().GetRoomUserManager().OnUserEnter -= this.delegateFunction;
            this.item = (Item)null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, string.Empty, this.userName, false, null);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            dbClient.SetQuery("SELECT trigger_data FROM wired_items WHERE trigger_id = @id ");
            dbClient.AddParameter("id", this.item.Id);
            DataRow row = dbClient.GetRow();
            this.userName = row == null ? string.Empty : row[0].ToString();
            this.isOneUser = !string.IsNullOrEmpty(this.userName);
        }

        public void DeleteFromDatabase(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = '" + this.item.Id + "'");
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message2 = new ServerPacket(ServerPacketHeader.WiredTriggerConfigMessageComposer);
            Message2.WriteBoolean(false);
            Message2.WriteInteger(0);
            Message2.WriteInteger(0);
            Message2.WriteInteger(SpriteId);
            Message2.WriteInteger(this.item.Id);
            Message2.WriteString(this.userName);
            Message2.WriteInteger(0);
            Message2.WriteInteger(0);
            Message2.WriteInteger(7);
            Message2.WriteInteger(0);
            Message2.WriteInteger(0);
            Message2.WriteInteger(0);
            Session.SendPacket(Message2);
        }

    }
}
