using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Core;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Items;
using JNogueira.Discord.Webhook.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Akiled.HabboHotel.Rooms
{
    public class Trade
    {
        private readonly TradeUser[] Users;
        private int TradeStage;

        private readonly int RoomId;
        private readonly int oneId;
        private readonly int twoId;

        public bool AllUsersAccepted
        {
            get
            {
                for (int index = 0; index < this.Users.Length; ++index)
                {
                    if (this.Users[index] != null && !this.Users[index].HasAccepted)
                        return false;
                }
                return true;
            }
        }

        public Trade(int UserOneId, int UserTwoId, int RoomId)
        {
            this.oneId = UserOneId;
            this.twoId = UserTwoId;
            this.Users = new TradeUser[2];
            this.Users[0] = new TradeUser(UserOneId, RoomId);
            this.Users[1] = new TradeUser(UserTwoId, RoomId);
            this.TradeStage = 1;
            this.RoomId = RoomId;

            ServerPacket Message = new ServerPacket(ServerPacketHeader.TradingStartMessageComposer);
            Message.WriteInteger(UserOneId);
            Message.WriteInteger(1);
            Message.WriteInteger(UserTwoId);
            Message.WriteInteger(1);
            this.SendMessageToUsers(Message);

            foreach (TradeUser tradeUser in this.Users)
            {
                if (!tradeUser.GetRoomUser().Statusses.ContainsKey("/trd"))
                {
                    tradeUser.GetRoomUser().SetStatus("/trd", "");
                    tradeUser.GetRoomUser().UpdateNeeded = true;
                }
            }
        }

        public bool ContainsUser(int Id)
        {
            for (int index = 0; index < this.Users.Length; ++index)
            {
                if (this.Users[index] != null && this.Users[index].UserId == Id)
                    return true;
            }
            return false;
        }

        public TradeUser GetTradeUser(int Id)
        {
            for (int index = 0; index < this.Users.Length; ++index)
            {
                if (this.Users[index] != null && this.Users[index].UserId == Id)
                    return this.Users[index];
            }
            return (TradeUser)null;
        }

        public void OfferItem(int UserId, Item Item)
        {
            TradeUser tradeUser = this.GetTradeUser(UserId);
            if (tradeUser == null || Item == null || (!Item.GetBaseItem().AllowTrade || tradeUser.HasAccepted) || this.TradeStage != 1)
                return;
            this.ClearAccepted();
            if (!tradeUser.OfferedItems.Contains(Item))
                tradeUser.OfferedItems.Add(Item);
            this.UpdateTradeWindow();
        }

        public void TakeBackItem(int UserId, Item Item)
        {
            TradeUser tradeUser = this.GetTradeUser(UserId);
            if (tradeUser == null || Item == null || (tradeUser.HasAccepted || this.TradeStage != 1))
                return;
            this.ClearAccepted();
            tradeUser.OfferedItems.Remove(Item);
            this.UpdateTradeWindow();
        }

        public void Accept(int UserId)
        {
            TradeUser tradeUser = this.GetTradeUser(UserId);
            if (tradeUser == null || this.TradeStage != 1)
                return;
            tradeUser.HasAccepted = true;
            ServerPacket Message = new ServerPacket(ServerPacketHeader.TradingAcceptMessageComposer);
            Message.WriteInteger(UserId);
            Message.WriteInteger(1);
            this.SendMessageToUsers(Message);
            if (!this.AllUsersAccepted)
                return;

            this.SendMessageToUsers(new ServerPacket(ServerPacketHeader.TradingCompleteMessageComposer));
            this.TradeStage++;
            this.ClearAccepted();
        }

        public void Unaccept(int UserId)
        {
            TradeUser tradeUser = this.GetTradeUser(UserId);
            if (tradeUser == null || this.TradeStage != 1 || this.AllUsersAccepted)
                return;
            tradeUser.HasAccepted = false;
            ServerPacket Message = new ServerPacket(ServerPacketHeader.TradingAcceptMessageComposer);
            Message.WriteInteger(UserId);
            Message.WriteInteger(0);
            this.SendMessageToUsers(Message);
        }

        public void CompleteTrade(int UserId)
        {
            TradeUser tradeUser = this.GetTradeUser(UserId);
            if (tradeUser == null || this.TradeStage != 2)
                return;
            tradeUser.HasAccepted = true;
            ServerPacket Message = new ServerPacket(ServerPacketHeader.TradingAcceptMessageComposer);
            Message.WriteInteger(UserId);
            Message.WriteInteger(1);
            this.SendMessageToUsers(Message);
            if (!this.AllUsersAccepted)
                return;
            this.TradeStage = 999;
            this.Finnito();
        }

        private async void Finnito()
        {
            try
            {
                await this.DeliverItemsAsync();
                this.CloseTradeClean();
            }
            catch (Exception ex)
            {
                Logging.LogThreadException((ex).ToString(), "Trade task");
            }
        }

        public void ClearAccepted()
        {
            foreach (TradeUser tradeUser in this.Users)
                tradeUser.HasAccepted = false;
        }

        public void UpdateTradeWindow()
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.TradingUpdateMessageComposer);
            for (int index = 0; index < this.Users.Length; ++index)
            {
                TradeUser tradeUser = this.Users[index];
                if (tradeUser != null)
                {
                    Message.WriteInteger(tradeUser.UserId);
                    Message.WriteInteger(tradeUser.OfferedItems.Count);
                    foreach (Item userItem in tradeUser.OfferedItems)
                    {
                        Message.WriteInteger(userItem.Id);
                        Message.WriteString(userItem.GetBaseItem().Type.ToString().ToLower());
                        Message.WriteInteger(userItem.Id);
                        Message.WriteInteger(userItem.GetBaseItem().SpriteId);
                        Message.WriteInteger(0);
                        if (userItem.LimitedNo > 0)
                        {
                            Message.WriteBoolean(false);
                            Message.WriteInteger(256);
                            Message.WriteString("");
                            Message.WriteInteger(userItem.LimitedNo);
                            Message.WriteInteger(userItem.LimitedTot);
                        }
                        else if (userItem.GetBaseItem().InteractionType == InteractionType.BADGE_DISPLAY || userItem.GetBaseItem().InteractionType == InteractionType.BADGE_TROC)
                        {
                            Message.WriteBoolean(false);
                            Message.WriteInteger(2);
                            Message.WriteInteger(4);

                            if (userItem.ExtraData.Contains(Convert.ToChar(9).ToString()))
                            {
                                string[] BadgeData = userItem.ExtraData.Split(Convert.ToChar(9));

                                Message.WriteString("0");//No idea
                                Message.WriteString(BadgeData[0]);//Badge name
                                Message.WriteString(BadgeData[1]);//Owner
                                Message.WriteString(BadgeData[2]);//Date
                            }
                            else
                            {
                                Message.WriteString("0");//No idea
                                Message.WriteString(userItem.ExtraData);//Badge name
                                Message.WriteString("");//Owner
                                Message.WriteString("");//Date
                            }
                        }
                        else
                        {
                            Message.WriteBoolean(true);
                            Message.WriteInteger(0);
                            Message.WriteString("");
                        }
                        Message.WriteInteger(0);
                        Message.WriteInteger(0);
                        Message.WriteInteger(0);
                        if (userItem.GetBaseItem().Type == 's')
                            Message.WriteInteger(0);
                    }
                    Message.WriteInteger(tradeUser.OfferedItems.Count);
                    Message.WriteInteger(0);

                }
            }
            this.SendMessageToUsers(Message);
        }

        public async Task DeliverItemsAsync()
        {
            List<Item> list1 = this.GetTradeUser(this.oneId).OfferedItems;
            List<Item> list2 = this.GetTradeUser(this.twoId).OfferedItems;
            string items1 = "";
            string items2 = "";
            string itemsname1 = ":";
            string itemsname2 = ":";
            foreach (Item userItem in list1)
            {
                if (this.GetTradeUser(this.oneId).GetClient().GetHabbo().GetInventoryComponent().GetItem(userItem.Id) == null)
                {
                    this.GetTradeUser(this.oneId).GetClient().SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("trade.failed", this.GetTradeUser(this.oneId).GetClient().Langue));
                    this.GetTradeUser(this.twoId).GetClient().SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("trade.failed", this.GetTradeUser(this.twoId).GetClient().Langue));
                    return;
                }
                items1 = items1 + userItem.Id.ToString() + "; ";
                itemsname1 = itemsname1 + userItem.GetBaseItem().ItemName.ToString() + "; ";
            }
            foreach (Item userItem in list2)
            {
                if (this.GetTradeUser(this.twoId).GetClient().GetHabbo().GetInventoryComponent().GetItem(userItem.Id) == null)
                {
                    this.GetTradeUser(this.oneId).GetClient().SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("trade.failed", this.GetTradeUser(this.oneId).GetClient().Langue));
                    this.GetTradeUser(this.twoId).GetClient().SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("trade.failed", this.GetTradeUser(this.twoId).GetClient().Langue));
                    return;
                }
                items2 = items2 + userItem.Id.ToString() + "; ";
                itemsname2 = itemsname2 + userItem.GetBaseItem().ItemName.ToString() + "; ";
            }
            foreach (Item userItem in list1)
            {
                this.GetTradeUser(this.oneId).GetClient().GetHabbo().GetInventoryComponent().RemoveItem(userItem.Id);
                this.GetTradeUser(this.twoId).GetClient().GetHabbo().GetInventoryComponent().AddItem(userItem);
            }
            foreach (Item userItem in list2)
            {
                this.GetTradeUser(this.twoId).GetClient().GetHabbo().GetInventoryComponent().RemoveItem(userItem.Id);
                this.GetTradeUser(this.oneId).GetClient().GetHabbo().GetInventoryComponent().AddItem(userItem);
            }

            ServerPacket Message1 = new ServerPacket(ServerPacketHeader.FurniListNotificationMessageComposer);
            Message1.WriteInteger(1);
            int i1 = 1;
            foreach (Item userItem in list1)
            {
                if (userItem.GetBaseItem().Type.ToString().ToLower() != "s")
                    i1 = 2;
            }
            Message1.WriteInteger(i1);
            Message1.WriteInteger(list1.Count);
            foreach (Item userItem in list1)
                Message1.WriteInteger(userItem.Id);
            this.GetTradeUser(this.twoId).GetClient().SendPacket(Message1);

            ServerPacket Message2 = new ServerPacket(ServerPacketHeader.FurniListNotificationMessageComposer);
            Message2.WriteInteger(1);
            int i2 = 1;
            foreach (Item userItem in list2)
            {
                if (userItem.GetBaseItem().Type.ToString().ToLower() != "s")
                    i2 = 2;
            }
            Message2.WriteInteger(i2);
            Message2.WriteInteger(list2.Count);
            foreach (Item userItem in list2)
                Message2.WriteInteger(userItem.Id);


            this.GetTradeUser(this.oneId).GetClient().SendPacket(Message2);

            this.GetTradeUser(this.oneId).GetClient().SendPacket(new FurniListUpdateComposer());
            this.GetTradeUser(this.twoId).GetClient().SendPacket(new FurniListUpdateComposer());

            using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.SetQuery("INSERT INTO `logs_client_trade` VALUES(null, @1id, @2id, @1items, @2items, UNIX_TIMESTAMP())");
                queryReactor.AddParameter("1id", this.oneId);
                queryReactor.AddParameter("2id", this.twoId);
                queryReactor.AddParameter("1items", items1);
                queryReactor.AddParameter("2items", items2);
                queryReactor.RunQuery();
            }

            string Webhook = AkiledEnvironment.GetConfig().data["Webhook"];
            string Webhook_trade_ProfilePicture = AkiledEnvironment.GetConfig().data["Webhook_trade_Image"];
            string Webhook_trade_Thumbnail = AkiledEnvironment.GetConfig().data["Webhook_trade_Thumbnail"];
            string Webhook_trade_UserNameD = AkiledEnvironment.GetConfig().data["Webhook_trade_Username"];
            string Webhook_trade_WebHookurl = AkiledEnvironment.GetConfig().data["Webhook_trade_URL"];

            if (Webhook == "true")
            {

                var client = new DiscordWebhookClient(Webhook_trade_WebHookurl);

                var message = new DiscordMessage(
                 "La Seguridad es importante para nosotros! " + DiscordEmoji.Grinning,
                    username: Webhook_trade_UserNameD,
                    avatarUrl: Webhook_trade_ProfilePicture,
                    tts: false,
                    embeds: new[]
        {
                                new DiscordMessageEmbed(
                                "Notificacion de trade" + DiscordEmoji.Thumbsup,
                                 color: 0,
                                author: new DiscordMessageEmbedAuthor(this.GetTradeUser(this.oneId).GetClient().GetHabbo().Username + " acepto el trade con " + this.GetTradeUser(this.twoId).GetClient().GetHabbo().Username),
                                description: "Informacion de trade",
                                fields: new[]
                                {
                                    new DiscordMessageEmbedField("Usuario 1", this.GetTradeUser(this.oneId).GetClient().GetHabbo().Username, true),
                                    new DiscordMessageEmbedField("Usuario 2", this.GetTradeUser(this.twoId).GetClient().GetHabbo().Username, true),
                                    new DiscordMessageEmbedField("Items 1", itemsname1 ?? "Nada", true),
                                    new DiscordMessageEmbedField("Items 2", itemsname2 ?? "Nada", true),

                                },
                                thumbnail: new DiscordMessageEmbedThumbnail(Webhook_trade_Thumbnail),
                                footer: new DiscordMessageEmbedFooter("Creado por: "+Webhook_trade_UserNameD, Webhook_trade_ProfilePicture)
        )
        }
        );
                await client.SendToDiscord(message);

                Console.WriteLine("Trade enviado a Discord ", ConsoleColor.DarkCyan);

            }
        }

        public void CloseTradeClean()
        {
            for (int index = 0; index < this.Users.Length; ++index)
            {
                TradeUser tradeUser = this.Users[index];
                if (tradeUser != null && tradeUser.GetRoomUser() != null)
                {
                    tradeUser.GetRoomUser().RemoveStatus("/trd");
                    tradeUser.GetRoomUser().UpdateNeeded = true;
                }
            }
            this.SendMessageToUsers(new ServerPacket(ServerPacketHeader.TradingFinishMessageComposer));
            this.GetRoom().ActiveTrades.Remove(this);
        }

        public void CloseTrade(int UserId)
        {
            for (int index = 0; index < this.Users.Length; ++index)
            {
                TradeUser tradeUser = this.Users[index];
                if (tradeUser != null && tradeUser.GetRoomUser() != null)
                {
                    tradeUser.GetRoomUser().RemoveStatus("/trd");
                    tradeUser.GetRoomUser().UpdateNeeded = true;
                }
            }
            ServerPacket Message = new ServerPacket(ServerPacketHeader.TradingClosedMessageComposer);
            Message.WriteInteger(UserId);
            Message.WriteInteger(2);
            this.SendMessageToUsers(Message);
        }

        public void SendMessageToUsers(ServerPacket Message)
        {
            if (this.Users == null)
                return;
            for (int index = 0; index < this.Users.Length; ++index)
            {
                TradeUser tradeUser = this.Users[index];
                if (tradeUser != null && tradeUser != null && tradeUser.GetClient() != null)
                    tradeUser.GetClient().SendPacket(Message);
            }
        }

        private Room GetRoom()
        {
            return AkiledEnvironment.GetGame().GetRoomManager().GetRoom(this.RoomId);
        }
    }
}