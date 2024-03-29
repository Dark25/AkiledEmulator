﻿using Akiled.Communication.Encryption.Crypto.Prng;
using Akiled.Communication.Packets.Incoming;
using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.Core;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.Users;
using Akiled.HabboHotel.Users.UserData;
using Akiled.Net;
using ConnectionManager;
using JNogueira.Discord.Webhook.Client;
using SharedPacketLib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Akiled.HabboHotel.GameClients
{
    public class GameClient
    {
        private ConnectionInformation Connection;
        private GamePacketParser packetParser;
        private Habbo Habbo;

        public string MachineId;
        public Language Langue;

        public ARC4 RC4Client = null;

        public int ConnectionID;

        public GameClient(int ClientId, ConnectionInformation pConnection)
        {
            this.ConnectionID = ClientId;
            this.Langue = Language.SPANISH;
            this.Connection = pConnection;
            this.packetParser = new GamePacketParser(this);
        }

        private void SwitchParserRequest()
        {
            this.packetParser.OnNewPacket += new GamePacketParser.HandlePacket(this.parser_onNewPacket);

            byte[] packet = (this.Connection.parser as InitialPacketParser).currentData;
            this.Connection.parser.Dispose();
            this.Connection.parser = (IDataParser)this.packetParser;
            this.Connection.parser.handlePacketData(packet);
        }

        public async Task TryAuthenticateAsync(string AuthTicket)
        {
            if (string.IsNullOrEmpty(AuthTicket))
                return;

            try
            {
                string ip = GetConnection().getIp();
                UserData userData = UserDataFactory.GetUserData(AuthTicket, ip, this.MachineId);

                if (userData == null) return;

                else
                {
                    AkiledEnvironment.GetGame().GetClientManager().LogClonesOut(userData.userID);
                    this.Habbo = userData.user;
                    this.Langue = this.Habbo.Langue;

                    AkiledEnvironment.GetGame().GetClientManager().RegisterClient(this, userData.userID, this.Habbo.Username);

                    if (this.Langue == Language.FRANCAIS) AkiledEnvironment.GetGame().GetClientManager().OnlineUsersFr++;
                    else if (this.Langue == Language.ANGLAIS) AkiledEnvironment.GetGame().GetClientManager().OnlineUsersEn++;
                    else if (this.Langue == Language.PORTUGAIS) AkiledEnvironment.GetGame().GetClientManager().OnlineUsersBr++;
                    else if (this.Langue == Language.SPANISH) AkiledEnvironment.GetGame().GetClientManager().OnlineUsersEs++;

                    if (this.Habbo.MachineId != this.MachineId)
                    {
                        using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            queryreactor.SetQuery("UPDATE users SET machine_id = @machineid WHERE id = '" + this.Habbo.Id + "'");
                            queryreactor.AddParameter("machineid", this.MachineId);
                            queryreactor.RunQuery();
                        }
                    }

                    this.Habbo.Init(this, userData);
                    this.Habbo.LoadData(userData);
                    this.Habbo.InitProcess();
                    this.Habbo.Look = AkiledEnvironment.GetFigureManager().ProcessFigure(this.Habbo.Look, this.Habbo.Gender, true);
                    await IsNewUserAsync().ConfigureAwait(true);
                    SendPacket(new AuthenticationOKComposer());
                    SendPacket(new FavouritesComposer(this.Habbo.FavoriteRooms));
                    SendPacket(new FigureSetIdsComposer(this.Habbo.GetClothing().GetClothingParts));

                    if (this.Habbo.HasFuse("fuse_mod"))
                    {
                        SendPacket(new UserRightsComposer(this.Habbo.Rank < 2 ? 2 : GetHabbo().Rank));
                    }
                    else
                    {
                        SendPacket(new UserRightsComposer(this.Habbo.Rank < 2 ? 2 : 2));
                    }

                    SendPacket(new AvailabilityStatusComposer());
                    SendPacket(new AchievementScoreComposer(this.Habbo.AchievementPoints));
                    SendPacket(new BuildersClubMembershipComposer());
                    SendPacket(new ActivityPointsComposer(Habbo.Duckets, Habbo.AkiledPoints));
                    SendPacket(new CfhTopicsInitComposer(AkiledEnvironment.GetGame().GetModerationManager().UserActionPresets));
                    SendPacket(new AvatarEffectsComposer(AkiledEnvironment.GetGame().GetEffectsInventoryManager().GetEffects()));
                    SendPacket(new SoundSettingsComposer(this.Habbo.ClientVolume, false, false, false, 1));
                    SendPacket(new UserHomeRoomComposer(this.Habbo.HomeRoom, this.Habbo.HomeRoom));


                    this.Habbo.UpdateActivityPointsBalance();
                    this.Habbo.UpdateCreditsBalance();
                    this.Habbo.UpdateDiamondsBalance();



                    string last_success_login = "Tu último inicio de sesiòn exitoso fue el ";
                    var last_success_login1 = last_success_login;
                    SendMessage(RoomNotificationComposer.SendBubble("last_success_login", last_success_login1 + new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(GetHabbo().LastOnline + 7200.0) + " Horas."));




                    string premiox20horasid = (AkiledEnvironment.GetConfig().data["premiox20horasid"]);
                    string premiox20horasname = (AkiledEnvironment.GetConfig().data["premiox20horasname"]);



                    if (this.Habbo.HasFuse("fuse_mod"))
                    {
                        AkiledEnvironment.GetGame().GetClientManager().AddUserStaff(Habbo.Id);
                        SendPacket(AkiledEnvironment.GetGame().GetModerationManager().SerializeTool());
                    }

                    if (!GetHabbo().Nuxenable)
                    {


                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        Console.WriteLine("Usuario Conectado: " + GetHabbo().Username + " al Hotel, con la IP :" + GetHabbo().IP + " ",
                        ConsoleColor.DarkGreen);

                        string Webhook = AkiledEnvironment.GetConfig().data["Webhook"];
                        string Webhook_login_logout_ProfilePicture = AkiledEnvironment.GetConfig().data["Webhook_on_off_Image"];
                        string Webhook_login_logout_UserNameD = AkiledEnvironment.GetConfig().data["Webhook_on_off_Username"];
                        string Webhook_login_logout_WebHookurl = AkiledEnvironment.GetConfig().data["Webhook_on_off_URL"];
                        string Webhooka_avatar = AkiledEnvironment.GetConfig().data["Webhook_avatar"];


                        if (Webhook == "true")
                        {

                            var client = new DiscordWebhookClient(Webhook_login_logout_WebHookurl);

                            var message = new DiscordMessage(
                             "La Seguridad es importante para nosotros! " + DiscordEmoji.Grinning,
                                username: Webhook_login_logout_UserNameD,
                                avatarUrl: Webhook_login_logout_ProfilePicture,
                                tts: false,
                                embeds: new[]
                    {
                                new DiscordMessageEmbed(
                                "Notificacion de login" + DiscordEmoji.Thumbsup,
                                 color: 4833120,
                                author: new DiscordMessageEmbedAuthor(GetHabbo().Username),
                                description: "Ha ingresado al cliente del hotel",
                                thumbnail: new DiscordMessageEmbedThumbnail(Webhooka_avatar + GetHabbo().Look),
                                footer: new DiscordMessageEmbedFooter("Creado por "+Webhook_login_logout_UserNameD, Webhook_login_logout_ProfilePicture)
        )
                    }
                    );

                            //agrega un catch

                            await client.SendToDiscord(message).ContinueWith(task => { if (task.IsFaulted) { Console.WriteLine("Error al enviar el mensaje: " + task.Exception.Message); } }).ConfigureAwait(true);

                            Console.WriteLine("login enviado a Discord ", ConsoleColor.DarkCyan);

                        }

                    }



                    if (GetHabbo().AccountCreated > AkiledEnvironment.GetUnixTimestamp() - 259200.0 && GetHabbo().Nuxenable2 && GetHabbo().AchievementPoints > 20000)
                    {
                        SendNotification("<font color = '#B40404'><font><b>Enhorabuena!</b></font></font>\n\nQueremos felicitarte por ser un amig@ fiel, y por haber completado 20 horas de conexión y  recompensa > 20000 en el hotel, te hemos obsequiado un Trono Clásico..\r");


                        using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                        {

                            dbClient.RunQuery("UPDATE users SET nux_enable2 = '0' WHERE id = " + GetHabbo().Id);


                        }

                        ItemData ItemData = null;
                        if (!AkiledEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(premiox20horasid), out ItemData))
                            return;

                        List<Item> Items = ItemFactory.CreateMultipleItems(ItemData, GetHabbo(), "", 1);
                        foreach (Item PurchasedItem in Items)
                        {
                            if (GetHabbo().GetInventoryComponent().TryAddItem(PurchasedItem))
                            {
                                SendPacket(new FurniListNotificationComposer(PurchasedItem.Id, 1));
                                SendPacket(RoomNotificationComposer.SendBubble("rewards", "Acabas de recibir un:\n\n " + premiox20horasname + ".\n\n¡Corre, " + GetHabbo().Username + ", revisa tu inventario, hay algo nuevo al parecer! \n\nClick Aquí", "inventory/open/furni"));
                                Console.ForegroundColor = ConsoleColor.DarkGreen;
                                Console.WriteLine("El Usuario: " + GetHabbo().Username + " ha recibido su premio x estar conectado 20 horas. ", "Akiled.Users",
                                ConsoleColor.DarkGreen);
                            }

                            return;
                        }


                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.LogException("Invalid Dario bug duing user login: " + (ex).ToString());
            }
        }

        private async Task IsNewUserAsync()
        {

            string sala_boy = (AkiledEnvironment.GetConfig().data["sala_boy"]);
            string sala_girl = (AkiledEnvironment.GetConfig().data["sala_girl"]);
            string modelsala_boy = (AkiledEnvironment.GetConfig().data["modelsala_boy"]);
            string modelsala_girl = (AkiledEnvironment.GetConfig().data["modelsala_girl"]);
            string Webhook = AkiledEnvironment.GetConfig().data["Webhook"];
            string ProfilePicture = AkiledEnvironment.GetConfig().data["Webhook_Image"];
            string UserNameD = AkiledEnvironment.GetConfig().data["Webhook_Username"];
            string WebHookurl = AkiledEnvironment.GetConfig().data["Webhook_URL"];
            string Webhooka_avatar = AkiledEnvironment.GetConfig().data["Webhook_avatar"];


            if (GetHabbo().NewUser && GetHabbo().Gender.ToUpper() != "M")
            {
                GetHabbo().NewUser = false;
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("Nuevo Usuario Conectado: " + GetHabbo().Username + " al Hotel, con la IP :" + GetHabbo().IP + " ", "Akiled.Users",
                ConsoleColor.DarkGreen);

                int RoomId = 0;


                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("INSERT INTO rooms (caption,description,owner,model_name,category,state, icon_bg, icon_fg, icon_items, wallpaper, floor, landscape, allow_hidewall, wallthick, floorthick) SELECT @caption, @desc, @username, @model, category, state, icon_bg, icon_fg, icon_items, wallpaper, floor, landscape, allow_hidewall, wallthick, floorthick FROM rooms WHERE id = '" + sala_girl + "'");
                    dbClient.AddParameter("caption", GetHabbo().Username + " - Tú Sala de Bienvenida");
                    dbClient.AddParameter("desc", AkiledEnvironment.GetLanguageManager().TryGetValue("room.welcome.desc", this.Langue));
                    dbClient.AddParameter("username", GetHabbo().Username);
                    dbClient.AddParameter("model", modelsala_girl);
                    RoomId = (int)dbClient.InsertQuery();
                    if (RoomId == 0)
                        return;

                    dbClient.RunQuery("INSERT INTO items (user_id, room_id, base_item, extra_data, x, y, z, rot) SELECT '" + GetHabbo().Id + "', '" + RoomId + "', base_item, extra_data, x, y, z, rot FROM items WHERE room_id = '" + sala_girl + "'");

                    dbClient.RunQuery("UPDATE users SET nux_enable = '0', home_room = '" + RoomId + "' WHERE id = " + GetHabbo().Id);


                }

                GetHabbo().UsersRooms.Add(AkiledEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId));
                GetHabbo().HomeRoom = RoomId;

                ServerPacket nuxStatus = new ServerPacket(ServerPacketHeader.NuxAlertComposer);
                nuxStatus.WriteInteger(2);
                SendPacket(nuxStatus);

                SendPacket(new NuxAlertComposer("nux/lobbyoffer/hide"));
                if (Webhook == "true")
                {
                    var client = new DiscordWebhookClient(WebHookurl);

                    var message = new DiscordMessage(
                     "La seguridad es importate para nosotros " + DiscordEmoji.Grinning,
                        username: UserNameD,
                        avatarUrl: ProfilePicture,
                        tts: false,
                        embeds: new[]
            {
                                new DiscordMessageEmbed(
                                "Notificación nuevo usuario" + DiscordEmoji.Thumbsup,
                                 color: 1824480,
                                author: new DiscordMessageEmbedAuthor(GetHabbo().Username),
                                description: "Ha ingresado al cliente del hotel por primera vez",
                                thumbnail: new DiscordMessageEmbedThumbnail(Webhooka_avatar + GetHabbo().Look),
                                footer: new DiscordMessageEmbedFooter("Creado por "+UserNameD, ProfilePicture)
        )
            }
            );
                    await client.SendToDiscord(message);
                    Console.WriteLine("login de nuevo usuario enviado a Discord ", ConsoleColor.DarkYellow);

                }
            }

            if (GetHabbo().NewUser && GetHabbo().Gender.ToUpper() != "F")
            {
                GetHabbo().NewUser = false;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Nuevo Usuario Conectado: " + GetHabbo().Username + " al Hotel, con la IP :" + GetHabbo().IP + " ", "Akiled.Users",
                ConsoleColor.DarkGreen);

                int RoomId = 0;

                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("INSERT INTO rooms (caption,description,owner,model_name,category,state, icon_bg, icon_fg, icon_items, wallpaper, floor, landscape, allow_hidewall, wallthick, floorthick) SELECT @caption, @desc, @username, @model, category, state, icon_bg, icon_fg, icon_items, wallpaper, floor, landscape, allow_hidewall, wallthick, floorthick FROM rooms WHERE id = '" + sala_boy + "'");
                    dbClient.AddParameter("caption", GetHabbo().Username + " - Tú Sala de Bienvenida");
                    dbClient.AddParameter("desc", AkiledEnvironment.GetLanguageManager().TryGetValue("room.welcome.desc", this.Langue));
                    dbClient.AddParameter("username", GetHabbo().Username);
                    dbClient.AddParameter("model", modelsala_boy);
                    RoomId = (int)dbClient.InsertQuery();
                    if (RoomId == 0)
                        return;

                    dbClient.RunQuery("INSERT INTO items (user_id, room_id, base_item, extra_data, x, y, z, rot) SELECT '" + GetHabbo().Id + "', '" + RoomId + "', base_item, extra_data, x, y, z, rot FROM items WHERE room_id = '" + sala_boy + "'");

                    dbClient.RunQuery("UPDATE users SET nux_enable = '0' WHERE id = " + GetHabbo().Id);


                }

                GetHabbo().UsersRooms.Add(AkiledEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId));
                GetHabbo().HomeRoom = RoomId;

                ServerPacket nuxStatus = new(ServerPacketHeader.NuxAlertComposer);
                nuxStatus.WriteInteger(2);
                SendPacket(nuxStatus);

                SendPacket(new NuxAlertComposer("nux/lobbyoffer/hide"));
                if (Webhook == "true")
                {
                    var client = new DiscordWebhookClient(WebHookurl);

                    var message = new DiscordMessage(
                     "La seguridad es importate para nosotros " + DiscordEmoji.Grinning,
                        username: UserNameD,
                        avatarUrl: ProfilePicture,
                        tts: false,
                        embeds: new[]
            {
                                new DiscordMessageEmbed(
                                "Notificación nuevo usuario" + DiscordEmoji.Thumbsup,
                                 color: 1824480,
                                author: new DiscordMessageEmbedAuthor(GetHabbo().Username),
                                description: "Ha ingresado al cliente del hotel por primera vez",
                                thumbnail: new DiscordMessageEmbedThumbnail(Webhooka_avatar + GetHabbo().Look),
                                footer: new DiscordMessageEmbedFooter("Creado por "+UserNameD, ProfilePicture)
        )
            }
            );
                    await client.SendToDiscord(message);
                    Console.WriteLine("login de nuevo usuario enviado a Discord ", ConsoleColor.DarkYellow);

                }
            }

        }

        private void parser_onNewPacket(ClientPacket Message)
        {
            try
            {
                AkiledEnvironment.GetGame().GetPacketManager().TryExecutePacket(this, Message);
            }
            catch (Exception ex)
            {
                Logging.LogPacketException(Message.ToString(), (ex).ToString());
            }
        }

        public ConnectionInformation GetConnection() => this.Connection;

        public Habbo GetHabbo() => this.Habbo;

        public void SendMessage(IServerPacket Message)
        {
            byte[] bytes = Message.GetBytes();

            if (Message == null)
                return;

            if (GetConnection() == null)
                return;

            GetConnection().SendData(bytes);
        }

        public void SendWhisper(string Message, int Colour = 0)
        {
            if (this == null || GetHabbo() == null || GetHabbo().CurrentRoom == null)
                return;

            RoomUser User = GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(GetHabbo().Username);
            if (User == null)
                return;

            SendPacket(new WhisperMessageComposer(User.VirtualId, Message, 0, (Colour == 0 ? User.LastBubble : Colour)));
        }


        public void StartConnection()
        {
            if (this.Connection == null)
                return;

            (this.Connection.parser as InitialPacketParser).SwitchParserRequest += new InitialPacketParser.NoParamDelegate(this.SwitchParserRequest);

            this.Connection.startPacketProcessing();
        }

        public bool Antipub(string Message, string type, int RoomId = 0)
        {
            if (GetHabbo() == null) return false;

            if (GetHabbo().HasFuse("word_filter_override")) return false;

            if (Message.Length <= 3) return false;

            Message = Encoding.GetEncoding("UTF-8").GetString(Encoding.GetEncoding("Windows-1252").GetBytes(Message));

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("INSERT INTO chatlogs (user_id,room_id,user_name,timestamp,message, type) VALUES ('" + GetHabbo().Id + "','" + RoomId + "',@pseudo,UNIX_TIMESTAMP(),@message,@type)");
                queryreactor.AddParameter("message", Message);
                queryreactor.AddParameter("type", type);
                queryreactor.AddParameter("pseudo", GetHabbo().Username);
                queryreactor.RunQuery();
            }

            if (!AkiledEnvironment.GetGame().GetChatManager().GetFilter().Ispub(Message))
            {
                if (AkiledEnvironment.GetGame().GetChatManager().GetFilter().CheckMessageWord(Message))
                {
                    using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        queryreactor.SetQuery("INSERT INTO chatlogs_pub (user_id,user_name,timestamp,message) VALUES ('" + GetHabbo().Id + "',@pseudo,UNIX_TIMESTAMP(),@message)");
                        queryreactor.AddParameter("message", "A vérifié: " + type + Message);
                        queryreactor.AddParameter("pseudo", GetHabbo().Username);
                        queryreactor.RunQuery();
                    }

                    foreach (GameClient Client in AkiledEnvironment.GetGame().GetClientManager().GetStaffUsers())
                    {
                        if (Client == null || Client.GetHabbo() == null)
                            continue;

                        Client.GetHabbo().SendWebPacket(new AddChatlogsComposer(this.Habbo.Id, this.Habbo.Username, type + Message));
                    }
                    return false;
                }

                return false;
            }

            int PubCount = GetHabbo().PubDectectCount++;

            if (type == "<CMD>") PubCount = 4;

            if (PubCount < 3 && PubCount > 0)
            {
                SendNotification(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.antipub.warn.1", this.Langue), PubCount));
                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryreactor.SetQuery("INSERT INTO chatlogs_pub (user_id,user_name,timestamp,message) VALUES ('" + GetHabbo().Id + "',@pseudo,UNIX_TIMESTAMP(),@message)");
                    queryreactor.AddParameter("message", "Pub numero " + PubCount + ": " + type + Message);
                    queryreactor.AddParameter("pseudo", GetHabbo().Username);
                    queryreactor.RunQuery();
                }
            }
            else if (PubCount == 3)
            {
                SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.antipub.warn.2", this.Langue));
                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryreactor.SetQuery("INSERT INTO chatlogs_pub (user_id,user_name,timestamp,message) VALUES ('" + GetHabbo().Id + "',@pseudo,UNIX_TIMESTAMP(),@message)");
                    queryreactor.AddParameter("message", "Pub numero " + PubCount + ": " + type + Message);
                    queryreactor.AddParameter("pseudo", GetHabbo().Username);
                    queryreactor.RunQuery();
                }
            }
            else if (PubCount == 4)
            {
                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryreactor.SetQuery("INSERT INTO chatlogs_pub (user_id,user_name,timestamp,message) VALUES ('" + GetHabbo().Id + "',@pseudo,UNIX_TIMESTAMP(),@message)");
                    queryreactor.AddParameter("message", "Pub numero " + PubCount + " bannisement: " + type + Message);
                    queryreactor.AddParameter("pseudo", GetHabbo().Username);
                    queryreactor.RunQuery();
                }

                Task task = AkiledEnvironment.GetGame().GetClientManager().BanUserAsync(this, "Robot", 86400, "Nuestro robot ha detectado la publicidad de la cuenta. " + GetHabbo().Username, true, false);
            }
            else
            {
                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryreactor.SetQuery("INSERT INTO chatlogs_pub (user_id,user_name,timestamp,message) VALUES ('" + GetHabbo().Id + "',@pseudo,UNIX_TIMESTAMP(),@message)");
                    queryreactor.AddParameter("message", "Pub numero " + PubCount + ": " + type + Message);
                    queryreactor.AddParameter("pseudo", GetHabbo().Username);
                    queryreactor.RunQuery();
                }
            }

            foreach (GameClient Client in AkiledEnvironment.GetGame().GetClientManager().GetStaffUsers())
            {
                if (Client == null || Client.GetHabbo() == null)
                    continue;

                Client.GetHabbo().SendWebPacket(new AddChatlogsComposer(this.Habbo.Id, this.Habbo.Username, type + Message));
            }

            return true;
        }

        public void SendNotification(string Message) => SendPacket(new BroadcastMessageAlertComposer(Message));

        public void SendHugeNotif(string Message)
        {
            ServerPacket Message1 = new ServerPacket(ServerPacketHeader.MOTDNotificationMessageComposer);
            Message1.WriteInteger(1);
            Message1.WriteString(Message);
            SendPacket(Message1);
        }

        public async Task Dispose()
        {
            if (GetHabbo() != null) await Habbo.OnDisconnectAsync().ConfigureAwait(true);

            this.Habbo = (Habbo)null;
            this.Connection = (ConnectionInformation)null;
            this.packetParser = null;
            this.RC4Client = null;
        }

        public void Disconnect()
        {
            if (this.Connection != null) this.Connection.Dispose();
        }

        public void SendPacket(IServerPacket Message)
        {
            if (Message == null || GetConnection() == null) return;

            GetConnection().SendData(Message.GetBytes());
        }
    }
}
