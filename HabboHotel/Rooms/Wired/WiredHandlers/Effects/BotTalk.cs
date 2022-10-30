using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Effects
{
    public class BotTalk : IWired, IWiredEffect
    {
        private WiredHandler handler;
        private readonly int itemID;
        private string NomBot;
        private string message;
        private bool IsCrier;

        public BotTalk(string nombot, string message, bool iscrier, WiredHandler handler, int itemID)
        {
            this.itemID = itemID;
            this.handler = handler;
            this.message = message;
            this.NomBot = nombot;
            this.IsCrier = iscrier;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (this.NomBot == "" || this.message == "")
                return;
            Room room = handler.GetRoom();
            RoomUser Bot = room.GetRoomUserManager().GetBotOrPetByName(this.NomBot);
            if (Bot == null)
                return;

            string TextMessage = this.message;
            if (user != null)
            {
                TextMessage = TextMessage.Replace("#username#", user.GetUsername());
                TextMessage = TextMessage.Replace("#point#", user.WiredPoints.ToString());
                TextMessage = TextMessage.Replace("#roomname#", this.handler.GetRoom().RoomData.Name.ToString());
                TextMessage = TextMessage.Replace("#vote_yes#", this.handler.GetRoom().VotedYesCount.ToString());
                TextMessage = TextMessage.Replace("#vote_no#", this.handler.GetRoom().VotedNoCount.ToString());
            }

            Bot.OnChat(TextMessage, (Bot.IsPet) ? 0 : 2, this.IsCrier);

        }

        public void Dispose()
        {
            this.message = (string)null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.itemID, string.Empty, this.NomBot + '\t' + this.message, this.IsCrier, null);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            dbClient.SetQuery("SELECT trigger_data, all_user_triggerable FROM wired_items WHERE trigger_id = @id ");
            dbClient.AddParameter("id", this.itemID);
            DataRow row = dbClient.GetRow();
            if (row == null)
                return;

            this.IsCrier = (bool)(row["all_user_triggerable"]);

            string Data = row["trigger_data"].ToString();

            if (string.IsNullOrWhiteSpace(Data) || !Data.Contains("\t"))
                return;

            string[] SplitData = Data.Split('\t');

            this.NomBot = SplitData[0].ToString();
            this.message = SplitData[1].ToString();
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message15 = new ServerPacket(ServerPacketHeader.WiredEffectConfigMessageComposer);
            Message15.WriteBoolean(false);
            Message15.WriteInteger(0);
            Message15.WriteInteger(0);
            Message15.WriteInteger(SpriteId);
            Message15.WriteInteger(this.itemID);
            Message15.WriteString(this.NomBot + '\t' + this.message);
            Message15.WriteInteger(1);
            Message15.WriteInteger(this.IsCrier ? 1 : 0);
            Message15.WriteInteger(0);
            Message15.WriteInteger(23); //7
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
