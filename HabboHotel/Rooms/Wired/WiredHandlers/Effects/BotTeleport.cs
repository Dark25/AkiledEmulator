using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Effects
{
    public class BotTeleport : IWired, IWiredEffect
    {
        private WiredHandler handler;
        private readonly int itemID;
        private string NameBot;
        private List<Item> items;
        private Gamemap gamemap;

        public BotTeleport(string namebot, List<Item> items, Gamemap gamemap, WiredHandler handler, int itemID)
        {
            this.itemID = itemID;
            this.handler = handler;
            this.NameBot = namebot;
            this.items = items;
            this.gamemap = gamemap;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (this.NameBot == "" || this.items.Count == 0)
                return;
            Room room = handler.GetRoom();
            RoomUser Bot = room.GetRoomUserManager().GetBotOrPetByName(this.NameBot);
            if (Bot == null)
                return;

            Item roomItem = this.items[AkiledEnvironment.GetRandomNumber(0, this.items.Count - 1)];
            if (roomItem == null)
                return;
            if (roomItem.Coordinate != Bot.Coordinate)
            {
                this.gamemap.TeleportToItem(Bot, roomItem);
            }
        }

        public void Dispose()
        {
            this.handler = (WiredHandler)null;
            this.gamemap = (Gamemap)null;
            if (this.items != null)
                this.items.Clear();
            this.items = (List<Item>)null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.itemID, string.Empty, this.NameBot, false, this.items);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            dbClient.SetQuery("SELECT trigger_data, triggers_item FROM wired_items WHERE trigger_id = @id ");
            dbClient.AddParameter("id", this.itemID);
            DataRow row = dbClient.GetRow();
            if (row == null)
                return;
            this.NameBot = row["trigger_data"].ToString();

            string itemslist = row["triggers_item"].ToString();

            if (itemslist == "")
                return;
            foreach (string item in itemslist.Split(';'))
            {
                Item roomItem = insideRoom.GetRoomItemHandler().GetItem(Convert.ToInt32(item));
                if (roomItem != null && !this.items.Contains(roomItem) && roomItem.Id != this.itemID)
                {
                    this.items.Add(roomItem);
                }
            }
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message15 = new ServerPacket(ServerPacketHeader.WiredEffectConfigMessageComposer);
            Message15.WriteBoolean(false);
            Message15.WriteInteger(10);
            Message15.WriteInteger(this.items.Count);
            foreach (Item roomItem in this.items)
                Message15.WriteInteger(roomItem.Id);
            Message15.WriteInteger(SpriteId);
            Message15.WriteInteger(this.itemID);
            Message15.WriteString(this.NameBot);
            Message15.WriteInteger(0);
            Message15.WriteInteger(0);
            Message15.WriteInteger(21); //7
            Message15.WriteInteger(0);
            Message15.WriteInteger(0);
            Message15.WriteInteger(0);
            Session.SendPacket(Message15);
        }

        public void DeleteFromDatabase(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = '" + this.itemID + "'");
        }
    }
}
