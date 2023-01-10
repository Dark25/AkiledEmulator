using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Effects
{
    public class BotFollowAvatar : IWired, IWiredEffect
    {
        private WiredHandler handler;
        private readonly int itemID;
        private string NameBot;
        private bool IsFollow;

        public BotFollowAvatar(string namebot, bool isfollow, WiredHandler handler, int itemID)
        {
            this.itemID = itemID;
            this.handler = handler;
            this.NameBot = namebot;
            this.IsFollow = isfollow;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (string.IsNullOrEmpty(this.NameBot))
                return;

            Room room = handler.GetRoom();
            RoomUser Bot = room.GetRoomUserManager().GetBotOrPetByName(this.NameBot);
            if (Bot == null)
                return;

            if (user != null && !user.IsBot && user.GetClient() != null)
            {
                if (this.IsFollow)
                {
                    if (Bot.BotData.FollowUser != user.VirtualId)
                        Bot.BotData.FollowUser = user.VirtualId;
                }
                else
                    Bot.BotData.FollowUser = 0;
            }
        }

        public void Dispose()
        {
            this.NameBot = (string)null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.itemID, string.Empty, this.NameBot, this.IsFollow, null);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            dbClient.SetQuery("SELECT trigger_data, all_user_triggerable FROM wired_items WHERE trigger_id = @id ");
            dbClient.AddParameter("id", this.itemID);
            DataRow row = dbClient.GetRow();
            if (row == null)
                return;
            this.IsFollow = (bool)(row["all_user_triggerable"]);

            this.NameBot = row["trigger_data"].ToString();
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message15 = new ServerPacket(ServerPacketHeader.WiredEffectConfigMessageComposer);
            Message15.WriteBoolean(false);
            Message15.WriteInteger(0);
            Message15.WriteInteger(0);
            Message15.WriteInteger(SpriteId);
            Message15.WriteInteger(this.itemID);
            Message15.WriteString(this.NameBot);
            Message15.WriteInteger(1);
            Message15.WriteInteger(this.IsFollow ? 1 : 0);

            Message15.WriteInteger(0);
            Message15.WriteInteger(25); //7
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
