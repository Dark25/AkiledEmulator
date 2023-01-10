using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;

namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Effects
{
    public class BotGiveHanditem : IWired, IWiredEffect
    {
        private readonly WiredHandler handler;
        private readonly int itemID;
        private string message;

        public BotGiveHanditem(string message, WiredHandler handler, int itemID)
        {
            this.itemID = itemID;
            this.handler = handler;
            this.message = message;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (this.message == "")
                return;

            if (user != null && !user.IsBot && user.GetClient() != null)
            {
                user.SendWhisperChat(this.message);
            }
        }

        public void Dispose()
        {
            this.message = (string)null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.itemID, string.Empty, this.message, false, null);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            dbClient.SetQuery("SELECT trigger_data FROM wired_items WHERE trigger_id = @id ");
            dbClient.AddParameter("id", this.itemID);
            this.message = dbClient.GetString();
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message15 = new ServerPacket(ServerPacketHeader.WiredEffectConfigMessageComposer);
            Message15.WriteBoolean(false);
            Message15.WriteInteger(0);
            Message15.WriteInteger(0);
            Message15.WriteInteger(SpriteId);
            Message15.WriteInteger(this.itemID);
            Message15.WriteString(this.message);
            Message15.WriteInteger(0);
            Message15.WriteInteger(0);
            Message15.WriteInteger(24); //7
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
