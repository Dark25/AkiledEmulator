using Akiled.Core;
using Akiled.HabboHotel.Users;
using Akiled.HabboHotel.Users.UserData;
using Akiled.Communication.Packets.Outgoing;
using Akiled.Net;
using ConnectionManager;
using SharedPacketLib;
using System;
using Akiled.Database.Interfaces;
using Akiled.Communication.Packets.Incoming;
using Akiled.Communication.Encryption.Crypto.Prng;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.HabboHotel.Items;
using System.Text;
using Akiled.HabboHotel.Rooms;
using System.Collections.Generic;
using Akiled.Communication.Packets.Outgoing.Inventory.AvatarEffects;

using JNogueira.Discord.Webhook.Client;
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
                string ip = this.GetConnection().getIp();
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
                    await this.IsNewUserAsync();
                    this.SendPacket(new AuthenticationOKComposer());
                    this.SendPacket(new FavouritesComposer(this.Habbo.FavoriteRooms));
                    this.SendPacket(new FigureSetIdsComposer(this.Habbo.GetClothing().GetClothingParts));
                   
                    if (this.Habbo.HasFuse("fuse_mod"))
                    {
                        this.SendPacket(new UserRightsComposer(this.Habbo.Rank < 2 ? 2 : this.GetHabbo().Rank));
                    }
                    else
                    {
                        this.SendPacket(new UserRightsComposer(this.Habbo.Rank < 2 ? 2 : 2));
                    }
                    this.SendPacket(new AvailabilityStatusComposer());
                    this.SendPacket(new AchievementScoreComposer(this.Habbo.AchievementPoints));
                    this.SendPacket(new BuildersClubMembershipComposer());
                    this.SendPacket(new ActivityPointsComposer(Habbo.Duckets, Habbo.AkiledPoints));
                    this.SendPacket(new CfhTopicsInitComposer(AkiledEnvironment.GetGame().GetModerationManager().UserActionPresets));
                    this.SendPacket(new SoundSettingsComposer(this.Habbo.ClientVolume, false, false, false, 1));
                    this.SendPacket(new AvatarEffectsComposer(AkiledEnvironment.GetGame().GetEffectsInventoryManager().GetEffects()));
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
                        this.SendPacket(AkiledEnvironment.GetGame().GetModerationManager().SerializeTool());
                    }

                    if (!this.GetHabbo().Nuxenable)
                    {
                    

                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        Console.WriteLine("Usuario Conectado: " + this.GetHabbo().Username + " al Hotel, con la IP :" + this.GetHabbo().IP + " ",
                        ConsoleColor.DarkGreen);
                        
                        string Webhook = AkiledEnvironment.GetConfig().data["Webhook"];
                        string Webhook_login_logout_ProfilePicture = AkiledEnvironment.GetConfig().data["Webhook_on_off_Image"];
                        string Webhook_login_logout_UserNameD = AkiledEnvironment.GetConfig().data["Webhook_on_off_Username"];
                        string Webhook_login_logout_WebHookurl = AkiledEnvironment.GetConfig().data["Webhook_on_off_URL"];

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
                                author: new DiscordMessageEmbedAuthor(this.GetHabbo().Username),
                                description: "Ha ingresado al cliente del hotel",
                                thumbnail: new DiscordMessageEmbedThumbnail("https://hrecu.site/habbo-imaging/avatar/" + this.GetHabbo().Look),
                                footer: new DiscordMessageEmbedFooter("Creado por "+Webhook_login_logout_UserNameD, Webhook_login_logout_ProfilePicture)
        )
                    }
                    );
                            await client.SendToDiscord(message);
                            Console.WriteLine("login enviado a Discord ", ConsoleColor.DarkCyan);

                        }

                    }
                    
                    

                    if (this.GetHabbo().AccountCreated > AkiledEnvironment.GetUnixTimestamp() - 259200.0 && this.GetHabbo().Nuxenable2 && this.GetHabbo().AchievementPoints > 20000)
                    {
                        this.SendNotification("<font color = '#B40404'><font><b>Enhorabuena!</b></font></font>\n\nQueremos felicitarte por ser un amig@ fiel, y por haber completado 20 horas de conexión y  recompensa > 20000 en el hotel, te hemos obsequiado un Trono Clásico..\r");


                        using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                        {

                            dbClient.RunQuery("UPDATE users SET nux_enable2 = '0' WHERE id = " + this.GetHabbo().Id);


                        }

                        ItemData ItemData = null;
                        if (!AkiledEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(premiox20horasid), out ItemData))
                            return;

                        List<Item> Items = ItemFactory.CreateMultipleItems(ItemData, this.GetHabbo(), "", 1);
                        foreach (Item PurchasedItem in Items)
                        {
                            if (this.GetHabbo().GetInventoryComponent().TryAddItem(PurchasedItem))
                            {
                                this.SendPacket(new FurniListNotificationComposer(PurchasedItem.Id, 1));
                                this.SendPacket(RoomNotificationComposer.SendBubble("rewards", "Acabas de recibir un:\n\n " + premiox20horasname + ".\n\n¡Corre, " + this.GetHabbo().Username + ", revisa tu inventario, hay algo nuevo al parecer! \n\nClick Aquí", "inventory/open/furni"));
                                Console.ForegroundColor = ConsoleColor.DarkGreen;
                                Console.WriteLine("El Usuario: " + this.GetHabbo().Username + " ha recibido su premio x estar conectado 20 horas. ", "Akiled.Users",
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


            if (GetHabbo().NewUser && this.GetHabbo().Gender.ToUpper() != "M")
            {
                GetHabbo().NewUser = false;
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("Nuevo Usuario Conectado: " + this.GetHabbo().Username + " al Hotel, con la IP :" + this.GetHabbo().IP + " ", "Akiled.Users",
                ConsoleColor.DarkGreen);

                int RoomId = 0;
               

                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("INSERT INTO rooms (caption,description,owner,model_name,category,state, icon_bg, icon_fg, icon_items, wallpaper, floor, landscape, allow_hidewall, wallthick, floorthick) SELECT @caption, @desc, @username, @model, category, state, icon_bg, icon_fg, icon_items, wallpaper, floor, landscape, allow_hidewall, wallthick, floorthick FROM rooms WHERE id = '" + sala_girl + "'");
                    dbClient.AddParameter("caption", this.GetHabbo().Username + " - Tú Sala de Bienvenida");
                    dbClient.AddParameter("desc", AkiledEnvironment.GetLanguageManager().TryGetValue("room.welcome.desc", this.Langue));
                    dbClient.AddParameter("username", this.GetHabbo().Username);
                    dbClient.AddParameter("model", modelsala_girl);
                    RoomId = (int)dbClient.InsertQuery();
                    if (RoomId == 0)
                        return;

                    dbClient.RunQuery("INSERT INTO items (user_id, room_id, base_item, extra_data, x, y, z, rot) SELECT '" + this.GetHabbo().Id + "', '" + RoomId + "', base_item, extra_data, x, y, z, rot FROM items WHERE room_id = '" + sala_girl + "'");

                    dbClient.RunQuery("UPDATE users SET nux_enable = '0', home_room = '" + RoomId + "' WHERE id = " + this.GetHabbo().Id);


                }

                this.GetHabbo().UsersRooms.Add(AkiledEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId));
                this.GetHabbo().HomeRoom = RoomId;

                ServerPacket nuxStatus = new ServerPacket(ServerPacketHeader.NuxAlertComposer);
                nuxStatus.WriteInteger(2);
                this.SendPacket(nuxStatus);

                this.SendPacket(new NuxAlertComposer("nux/lobbyoffer/hide"));
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
                                author: new DiscordMessageEmbedAuthor(this.GetHabbo().Username),
                                description: "Ha ingresado al cliente del hotel por primera vez",
                                thumbnail: new DiscordMessageEmbedThumbnail("https://hrecu.site/habbo-imaging/avatar/" + this.GetHabbo().Look),
                                footer: new DiscordMessageEmbedFooter("Creado por "+UserNameD, ProfilePicture)
        )
            }
            );
                    await client.SendToDiscord(message);
                    Console.WriteLine("login de nuevo usuario enviado a Discord ", ConsoleColor.DarkYellow);

                }
            }

            if (GetHabbo().NewUser && this.GetHabbo().Gender.ToUpper() != "F")
            {
                GetHabbo().NewUser = false;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Nuevo Usuario Conectado: " + this.GetHabbo().Username + " al Hotel, con la IP :" + this.GetHabbo().IP + " ", "Akiled.Users",
                ConsoleColor.DarkGreen);

                int RoomId = 0;

                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("INSERT INTO rooms (caption,description,owner,model_name,category,state, icon_bg, icon_fg, icon_items, wallpaper, floor, landscape, allow_hidewall, wallthick, floorthick) SELECT @caption, @desc, @username, @model, category, state, icon_bg, icon_fg, icon_items, wallpaper, floor, landscape, allow_hidewall, wallthick, floorthick FROM rooms WHERE id = '" + sala_boy + "'");
                    dbClient.AddParameter("caption", this.GetHabbo().Username + " - Tú Sala de Bienvenida");
                    dbClient.AddParameter("desc", AkiledEnvironment.GetLanguageManager().TryGetValue("room.welcome.desc", this.Langue));
                    dbClient.AddParameter("username", this.GetHabbo().Username);
                    dbClient.AddParameter("model", modelsala_boy);
                    RoomId = (int)dbClient.InsertQuery();
                    if (RoomId == 0)
                        return;

                    dbClient.RunQuery("INSERT INTO items (user_id, room_id, base_item, extra_data, x, y, z, rot) SELECT '" + this.GetHabbo().Id + "', '" + RoomId + "', base_item, extra_data, x, y, z, rot FROM items WHERE room_id = '" + sala_boy + "'");

                    dbClient.RunQuery("UPDATE users SET nux_enable = '0' WHERE id = " + this.GetHabbo().Id);


                }

                this.GetHabbo().UsersRooms.Add(AkiledEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId));
                this.GetHabbo().HomeRoom = RoomId;

                ServerPacket nuxStatus = new ServerPacket(ServerPacketHeader.NuxAlertComposer);
                nuxStatus.WriteInteger(2);
                this.SendPacket(nuxStatus);

                this.SendPacket(new NuxAlertComposer("nux/lobbyoffer/hide"));
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
                                author: new DiscordMessageEmbedAuthor(this.GetHabbo().Username),
                                description: "Ha ingresado al cliente del hotel por primera vez",
                                thumbnail: new DiscordMessageEmbedThumbnail("https://hrecu.site/habbo-imaging/avatar/" + this.GetHabbo().Look),
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
            if (this.GetHabbo() == null) return false;

            if (this.GetHabbo().HasFuse("word_filter_override")) return false;

            if (Message.Length <= 3) return false;

            Message = Encoding.GetEncoding("UTF-8").GetString(Encoding.GetEncoding("Windows-1252").GetBytes(Message));

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("INSERT INTO chatlogs (user_id,room_id,user_name,timestamp,message, type) VALUES ('" + this.GetHabbo().Id + "','" + RoomId + "',@pseudo,UNIX_TIMESTAMP(),@message,@type)");
                queryreactor.AddParameter("message", Message);
                queryreactor.AddParameter("type", type);
                queryreactor.AddParameter("pseudo", this.GetHabbo().Username);
                queryreactor.RunQuery();
            }

            if (!AkiledEnvironment.GetGame().GetChatManager().GetFilter().Ispub(Message))
            {
                if (AkiledEnvironment.GetGame().GetChatManager().GetFilter().CheckMessageWord(Message))
                {
                    using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        queryreactor.SetQuery("INSERT INTO chatlogs_pub (user_id,user_name,timestamp,message) VALUES ('" + this.GetHabbo().Id + "',@pseudo,UNIX_TIMESTAMP(),@message)");
                        queryreactor.AddParameter("message", "A vérifié: " + type + Message);
                        queryreactor.AddParameter("pseudo", this.GetHabbo().Username);
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

            int PubCount = this.GetHabbo().PubDectectCount++;

            if (type == "<CMD>") PubCount = 4;

            if (PubCount < 3 && PubCount > 0)
            {
                this.SendNotification(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.antipub.warn.1", this.Langue), PubCount));
                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryreactor.SetQuery("INSERT INTO chatlogs_pub (user_id,user_name,timestamp,message) VALUES ('" + this.GetHabbo().Id + "',@pseudo,UNIX_TIMESTAMP(),@message)");
                    queryreactor.AddParameter("message", "Pub numero " + PubCount + ": " + type + Message);
                    queryreactor.AddParameter("pseudo", this.GetHabbo().Username);
                    queryreactor.RunQuery();
                }
            }
            else if (PubCount == 3)
            {
                this.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.antipub.warn.2", this.Langue));
                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryreactor.SetQuery("INSERT INTO chatlogs_pub (user_id,user_name,timestamp,message) VALUES ('" + this.GetHabbo().Id + "',@pseudo,UNIX_TIMESTAMP(),@message)");
                    queryreactor.AddParameter("message", "Pub numero " + PubCount + ": " + type + Message);
                    queryreactor.AddParameter("pseudo", this.GetHabbo().Username);
                    queryreactor.RunQuery();
                }
            }
            else if (PubCount == 4)
            {
                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryreactor.SetQuery("INSERT INTO chatlogs_pub (user_id,user_name,timestamp,message) VALUES ('" + this.GetHabbo().Id + "',@pseudo,UNIX_TIMESTAMP(),@message)");
                    queryreactor.AddParameter("message", "Pub numero " + PubCount + " bannisement: " + type + Message);
                    queryreactor.AddParameter("pseudo", this.GetHabbo().Username);
                    queryreactor.RunQuery();
                }

                Task task = AkiledEnvironment.GetGame().GetClientManager().BanUserAsync(this, "Robot", 86400, "Nuestro robot ha detectado la publicidad de la cuenta. " + this.GetHabbo().Username, true, false);
            }
            else
            {
                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryreactor.SetQuery("INSERT INTO chatlogs_pub (user_id,user_name,timestamp,message) VALUES ('" + this.GetHabbo().Id + "',@pseudo,UNIX_TIMESTAMP(),@message)");
                    queryreactor.AddParameter("message", "Pub numero " + PubCount + ": " + type + Message);
                    queryreactor.AddParameter("pseudo", this.GetHabbo().Username);
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
            this.SendPacket(Message1);
        }

        public async Task Dispose()
        {
            if (this.GetHabbo() != null) await this.Habbo.OnDisconnectAsync();

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
            if (Message == null || this.GetConnection() == null) return;
            
            this.GetConnection().SendData(Message.GetBytes());
        }
    }
}
