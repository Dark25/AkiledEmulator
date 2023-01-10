using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System;

namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Triggers
{
    public class UserCommand : IWired
    {
        private Item item;
        private WiredHandler handler;
        private readonly RoomEventDelegate delegateFunction;

        public UserCommand(Item item, WiredHandler handler, Room room)
        {
            this.item = item;
            this.handler = handler;
            this.delegateFunction = new RoomEventDelegate(this.roomUserManager_OnUserSays);
            room.OnUserCmd += this.delegateFunction;
        }

        private void roomUserManager_OnUserSays(object sender, EventArgs e)
        {
            RoomUser user = (RoomUser)sender;
            if (user == null || user.IsBot)
                return;

            this.handler.ExecutePile(this.item.Coordinate, user, null);
        }
        public void Dispose()
        {
            this.handler.GetRoom().OnUserCmd -= this.delegateFunction;
            this.item = (Item)null;
            this.handler = (WiredHandler)null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, string.Empty, string.Empty, false, null);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {

        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.WiredTriggerConfigMessageComposer);
            Message.WriteBoolean(false);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(SpriteId);
            Message.WriteInteger(this.item.Id);
            Message.WriteString("");
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(8);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Session.SendPacket(Message);
        }

        public void DeleteFromDatabase(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = '" + this.item.Id + "'");
        }
    }
}
