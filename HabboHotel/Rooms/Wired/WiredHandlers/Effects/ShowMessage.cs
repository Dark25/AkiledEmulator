using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Items;
using System.Data;
using System.Threading.Tasks;

namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Effects
{
    public class ShowMessage : IWired, IWiredEffect, IWiredCycleable
    {
        private readonly WiredHandler handler;
        private readonly int itemID;
        private string message;
        public int Delay { get; set; }
        private bool disposed;

        public ShowMessage(string message, WiredHandler handler, int itemID, int mdelay)
        {
            this.itemID = itemID;
            this.handler = handler;
            this.message = message;

            this.Delay = mdelay;
            this.disposed = false;
        }

        private void HandleEffect(RoomUser user, Item TriggerItem)
        {
            if (this.message == "")
                return;

            if (user != null && !user.IsBot && user.GetClient() != null)
            {
                string TextMessage = this.message;
                TextMessage = TextMessage.Replace("#username#", user.GetUsername());
                TextMessage = TextMessage.Replace("%username%", user.GetUsername());
                TextMessage = TextMessage.Replace("#point#", user.WiredPoints.ToString());
                TextMessage = TextMessage.Replace("%Planetas%", user.WiredPoints.ToString());
                TextMessage = TextMessage.Replace("%diamantes%", user.WiredPoints.ToString());
                TextMessage = TextMessage.Replace("%diamantes%", user.WiredPoints.ToString());
                TextMessage = TextMessage.Replace("#roomname#", this.handler.GetRoom().RoomData.Name.ToString());
                TextMessage = TextMessage.Replace("%roomname%", this.handler.GetRoom().RoomData.Name.ToString());
                TextMessage = TextMessage.Replace("%roomowner%", this.handler.GetRoom().RoomData.OwnerName.ToString());
                TextMessage = TextMessage.Replace("%roomlikes%", this.handler.GetRoom().RoomData.Score.ToString());
                TextMessage = TextMessage.Replace("%userson%", AkiledEnvironment.GetGame().GetClientManager().Count.ToString());
                TextMessage = TextMessage.Replace("#vote_yes#", this.handler.GetRoom().VotedYesCount.ToString());
                TextMessage = TextMessage.Replace("#vote_no#", this.handler.GetRoom().VotedNoCount.ToString());

                user.SendWhisperChat(TextMessage);
            }
        }

        public bool OnCycle(RoomUser user, Item item)
        {
            this.HandleEffect(user, item);
            return false;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (user == null || user.IsBot || user.GetClient() == null)
                return;

            if (this.message == "")
                return;

            if (this.Delay > 0)
                this.handler.RequestCycle(new WiredCycle(this, user, TriggerItem, this.Delay));
            else
                this.HandleEffect(user, TriggerItem);
        }

        public void Dispose()
        {
            this.disposed = true;
            this.message = (string)null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.itemID, this.Delay.ToString(), this.message, false, null);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            dbClient.SetQuery("SELECT trigger_data, trigger_data_2 FROM wired_items WHERE trigger_id = @id ");
            dbClient.AddParameter("id", this.itemID);
            DataRow row = dbClient.GetRow();
            if (row == null)
                return;
            this.message = row["trigger_data"].ToString();

            int result;
            this.Delay = (int.TryParse(row["trigger_data_2"].ToString(), out result)) ? result : 0;
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
            Message15.WriteInteger(7); //7
            Message15.WriteInteger(this.Delay);
            Message15.WriteInteger(0);
            Message15.WriteInteger(0);
            Session.SendPacket(Message15);
        }

        public void DeleteFromDatabase(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = '" + this.itemID + "'");
        }

        public bool Disposed()
        {
            return this.disposed;
        }
    }
}
