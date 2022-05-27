using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Quests;
using Akiled.HabboHotel.Roleplay.Player;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class FishCommand : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if ((double)Session.GetHabbo().last_fish > (double)AkiledEnvironment.GetUnixTimestamp() - 30.0 && !Session.GetHabbo().HasFuse("override_limit_command"))
            {
                Session.SendWhisper("Debes esperar 30 segundos, para volver a usar el comando", 34);
            }
            else
            {
                RoomUser Bot = (RoomUser)null;
                RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
                List<string> stringList = new List<string>();
                string room_idfish = AkiledEnvironment.GetConfig().data["room_idfish"];
                string str1 = AkiledEnvironment.GetConfig().data["baseitem_idfish"];
                string item_idfish = AkiledEnvironment.GetConfig().data["item_idfish"];
                string str2 = AkiledEnvironment.GetConfig().data["item_idgusano"];
                string name_monedaoficial = AkiledEnvironment.GetConfig().data["name_monedaoficial"];
                string icon_monedaoficial = AkiledEnvironment.GetConfig().data["icon_monedaoficial"];
                string name = AkiledEnvironment.GetConfig().data["namebot"];
                string str3 = AkiledEnvironment.GetConfig().data["premio1"];
                string str4 = AkiledEnvironment.GetConfig().data["premio2"];
                string str5 = AkiledEnvironment.GetConfig().data["premio3"];
                string str6 = AkiledEnvironment.GetConfig().data["premio4"];
                string premio5 = AkiledEnvironment.GetConfig().data["premio5"];
                string premio6 = AkiledEnvironment.GetConfig().data["premio6"];
                string premio7 = AkiledEnvironment.GetConfig().data["premio7"];
                string str7 = AkiledEnvironment.GetConfig().data["premio8"];
                string premio9 = AkiledEnvironment.GetConfig().data["premio9"];
                string str8 = AkiledEnvironment.GetConfig().data["premio10"];
                string premio11 = AkiledEnvironment.GetConfig().data["premio11"];
                string premio12 = AkiledEnvironment.GetConfig().data["premio12"];
                string premio13 = AkiledEnvironment.GetConfig().data["premio13"];
                string premio14 = AkiledEnvironment.GetConfig().data["premio14"];
                string premio15 = AkiledEnvironment.GetConfig().data["premio15"];
                string premio16 = AkiledEnvironment.GetConfig().data["premio16"];
                Bot = Room.GetRoomUserManager().GetBotOrPetByName(name);
                if (Session.GetHabbo().CurrentRoomId != Convert.ToInt32(room_idfish))
                {
                    if (!Session.GetHabbo().InRoom)
                        Session.SendMessage((IServerPacket)new RoomForwardComposer(Convert.ToInt32(room_idfish)));
                    else
                        Session.GetHabbo().PrepareRoom(Convert.ToInt32(room_idfish));
                }
                else
                {
                    foreach (Item obj in Room.GetGameMap().GetRoomItemForSquare(ThisUser.X, ThisUser.Y).ToList<Item>())
                    {
                        if (obj.BaseItem == Convert.ToInt32(str1))
                        {
                            if (!Session.GetHabbo().hasangel)
                            {
                                using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    queryReactor.SetQuery("SELECT * FROM `user_rpitems` WHERE `user_id` = " + Session.GetHabbo().Id.ToString() + " AND `item_id` = '" + Convert.ToInt32(item_idfish).ToString() + "' LIMIT 1;");
                                    if (queryReactor.GetRow() != null)
                                        Session.GetHabbo().hasangel = true;
                                }
                            }
                            if (!Session.GetHabbo().hasangel)
                            {
                                Session.SendWhisper("¡No tienes una caña de pescar! Párate frente a Susseth y compra una caña de pescar y gusanos.", 34);
                                break;
                            }
                            if (Session.GetHabbo().wurm == -1)
                            {
                                using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    queryReactor.SetQuery("SELECT * FROM `user_rpitems` WHERE `user_id` = " + Session.GetHabbo().Id.ToString() + " AND `item_id` = '" + Convert.ToInt32(str2).ToString() + "' LIMIT 1;");
                                    DataRow row = queryReactor.GetRow();
                                    Session.GetHabbo().wurm = row == null ? 0 : (int)row["count"];
                                }
                            }
                            if (Session.GetHabbo().onfarm)
                                Session.SendWhisper("¡No puedes pescar mientras cavas gusanos!", 34);
                            else if (Session.GetHabbo().is_angeln)
                            {
                                ThisUser.ApplyEffect(0);
                                Session.SendWhisper("Dejaste de pescar.", 34);
                                Session.GetHabbo().is_angeln = false;
                                ThisUser.Freeze = false;
                            }
                            else if (Session.GetHabbo().wurm <= 0 || Session.GetHabbo().wurm <= -1)
                            {
                                Session.SendWhisper("No te quedan gusanos! Compre algunos en la tienda de Susseth o desentierra algunos en el tronco.", 34);
                                Session.GetHabbo().is_angeln = false;
                                Session.SendWhisper("Dejaste de pescar.", 34);
                                ThisUser.Freeze = false;
                            }
                            else if (UserRoom.IsAsleep)
                            {
                                ThisUser.ApplyEffect(0);
                                Session.SendWhisper("Has dejado de Minar por estar ausente.", 34);
                                Session.GetHabbo().is_mining = false;
                                ThisUser.Freeze = false;
                            }
                            else
                            {
                                Session.GetHabbo().last_fish = AkiledEnvironment.GetIUnixTimestamp();
                                Session.SendWhisper("Woooo! La caña de pescar está en el agua. ¡Sé paciente ahora, en algún momento morderá un pez!", 34);
                                Session.SendWhisper("No podrás moverte hasta que no dejes de pescar", 34);
                                RolePlayer Rp = ThisUser.Roleplayer;
                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@blue@*Has comenzado a pescar*", 0, ThisUser.LastBubble));
                                Session.GetHabbo().is_angeln = true;
                                ThisUser.Freeze = true;
                                ThisUser.ApplyEffect(641);
                                Task.Run((Func<Task>)(async () =>
                                {
                                    Random xx = new Random();
                                    int async_task = xx.Next(0, 100);
                                    Session.GetHabbo().async_angel_id = async_task;
                                    while (Session.GetHabbo().angelpass > AkiledEnvironment.GetIUnixTimestamp())
                                    {
                                        if (Session.GetHabbo() == null || !Session.GetHabbo().is_angeln)
                                        {
                                            if (Session.GetHabbo() != null)
                                            {
                                                ThisUser.ApplyEffect(0);
                                                break;
                                            }
                                            break;
                                        }
                                        if (Session.GetHabbo().CurrentRoomId != Convert.ToInt32(room_idfish) || !Session.GetHabbo().InRoom)
                                        {
                                            ThisUser.ApplyEffect(0);
                                            Session.GetHabbo().is_angeln = false;
                                            break;
                                        }
                                        Random x = new Random();
                                        float nextfish = (float)(x.Next(40, 180) / 3);
                                        float i;
                                        for (i = 0.0f; (double)i < (double)nextfish; ++i)
                                        {
                                            await Task.Delay(3000);
                                            if (Session.GetHabbo() == null || !Session.GetHabbo().is_angeln)
                                            {
                                                i = 9999f;
                                                break;
                                            }
                                            if (Session.GetHabbo().async_angel_id != async_task)
                                            {
                                                i = 9999f;
                                                break;
                                            }
                                            try
                                            {
                                                RoomUser ThisUser2 = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
                                                if (ThisUser.VirtualId != ThisUser2.VirtualId)
                                                {
                                                    i = 9999f;
                                                    Session.SendNotification("Ya no estás pescando porque saliste del campamento o te cambiaste de sala.");
                                                    ThisUser.ApplyEffect(0);
                                                    Session.GetHabbo().is_angeln = false;
                                                    ThisUser2 = (RoomUser)null;
                                                    ThisUser2 = (RoomUser)null;
                                                    break;
                                                }
                                            }
                                            catch
                                            {
                                                i = 9999f;
                                                Session.SendNotification("Ya no estás pescando porque saliste del campamento o te cambiaste de sala.");
                                                ThisUser.ApplyEffect(0);
                                                Session.GetHabbo().is_angeln = false;
                                                break;
                                            }
                                            if (Session.GetHabbo().CurrentRoomId != Convert.ToInt32(room_idfish) || !Session.GetHabbo().InRoom)
                                            {
                                                i = 9999f;
                                                Session.SendNotification("Ya no estás pescando porque saliste del campamento o te cambiaste de sala.");
                                                ThisUser.ApplyEffect(0);
                                                Session.GetHabbo().is_angeln = false;
                                                break;
                                            }
                                        }
                                        if ((double)i <= 9000.0 && Session.GetHabbo() != null && Session.GetHabbo().is_angeln && Session.GetHabbo().async_angel_id == async_task)
                                        {
                                            if (Session.GetHabbo().CurrentRoomId != Convert.ToInt32(room_idfish) || !Session.GetHabbo().InRoom)
                                            {
                                                ThisUser.ApplyEffect(0);
                                                Session.GetHabbo().is_angeln = false;
                                                break;
                                            }
                                            int fishid = x.Next(0, 1001);
                                            ThisUser = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
                                            if (Session.GetHabbo().wurm <= 0)
                                            {
                                                Session.SendWhisper("No te quedan gusanos! Compre algunos en la tienda de Susseth o desentierra algunos.", 34);
                                                ThisUser.ApplyEffect(0);
                                                ThisUser.Freeze = false;
                                                Session.GetHabbo().is_angeln = false;
                                            }
                                            if (UserRoom.IsAsleep)
                                            {
                                                ThisUser.ApplyEffect(0);
                                                Session.SendWhisper("Has dejado de Pescar por estar ausente.", 34);
                                                Session.GetHabbo().is_angeln = false;
                                                ThisUser.Freeze = false;
                                            }
                                            else if (fishid % 4 == 0)
                                            {
                                                Session.SendWhisper("¡Tu caña de pescar sufrio daños por una piraña!", 34);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@red@*Tenía una piraña malvada en el anzuelo, te ha restado una caña de pescar*", 0, ThisUser.LastBubble));
                                                Session.GetHabbo().DestroyAngel(x.Next(0, 5));
                                                DataRow GetPromotion4 = (DataRow)null;
                                                using (IQueryAdapter queryAdapter = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                                {
                                                    queryAdapter.SetQuery("SELECT * FROM `user_rpitems` WHERE `user_id` = " + Session.GetHabbo().Id.ToString() + " AND `item_id` = '" + Convert.ToInt32(item_idfish).ToString() + "' LIMIT 1;");
                                                    GetPromotion4 = queryAdapter.GetRow();
                                                    if (GetPromotion4 != null)
                                                    {
                                                        Session.GetHabbo().hasangel = true;
                                                        ThisUser.Freeze = true;
                                                        ThisUser.ApplyEffect(641);
                                                        Session.GetHabbo().is_angeln = true;
                                                    }
                                                    else
                                                    {
                                                        Session.SendWhisper("Dejaste de pescar.", 34);
                                                        ThisUser.ApplyEffect(0);
                                                        ThisUser.Freeze = false;
                                                        Session.GetHabbo().hasangel = false;
                                                        Session.GetHabbo().is_angeln = false;
                                                        xx = (Random)null;
                                                        xx = (Random)null;
                                                        return;
                                                    }
                                                }
                                                GetPromotion4 = (DataRow)null;
                                                GetPromotion4 = (DataRow)null;
                                            }
                                            else if (fishid < 200)
                                            {
                                                Session.GetHabbo().EatWurm();
                                                Session.SendWhisper("Atrapaste una sardina!", 1);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@blue@*Jalar el Carrete con una sardina*", 0, ThisUser.LastBubble));
                                                ServerPacket Message = new ServerPacket(2704);
                                                Message.WriteInteger(Bot.VirtualId);
                                                Message.WriteString(Session.GetHabbo().Username + " no me gusta la sardina, entonces no te puedo pagar, lo siento sigue pescando.");
                                                Message.WriteInteger(0);
                                                Message.WriteInteger(2);
                                                Message.WriteInteger(0);
                                                Message.WriteInteger(-1);
                                                Session.SendPacket((IServerPacket)Message);
                                                Rp.Money += 25;
                                                Rp.Exp += 30;
                                                Rp.SendUpdate();
                                                Session.GetHabbo().SendPacketWeb((IServerPacket)new PlaySoundComposer("carrete", 2));
                                                Session.GetHabbo().AddFish(1);
                                                Message = (ServerPacket)null;
                                                Message = (ServerPacket)null;
                                            }
                                            else if (fishid < 300)
                                            {
                                                Session.GetHabbo().EatWurm();
                                                Session.SendWhisper("Atrapaste una pez Globo!", 1);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@blue@*Jalar el Carrete con un pez Globo*", 0, ThisUser.LastBubble));
                                                ServerPacket Message = new ServerPacket(2704);
                                                Message.WriteInteger(Bot.VirtualId);
                                                Message.WriteString(Session.GetHabbo().Username + ", Gordito cuando quiere, no es comestible, no puedo pagar por un pez Globo.");
                                                Message.WriteInteger(0);
                                                Message.WriteInteger(2);
                                                Message.WriteInteger(0);
                                                Message.WriteInteger(-1);
                                                Session.SendPacket((IServerPacket)Message);
                                                Rp.Money += 25;
                                                Rp.Exp += 30;
                                                Rp.SendUpdate();
                                                Session.GetHabbo().SendPacketWeb((IServerPacket)new PlaySoundComposer("carrete", 2));
                                                Session.GetHabbo().AddFish(2);
                                                Message = (ServerPacket)null;
                                                Message = (ServerPacket)null;
                                            }
                                            else if (fishid < 350)
                                            {
                                                Session.GetHabbo().EatWurm();
                                                Session.SendWhisper("Atrapaste a la suegra de LuisC!", 1);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@blue@*Jalar el Carrete con la Suegra*", 0, ThisUser.LastBubble));
                                                ServerPacket Message = new ServerPacket(2704);
                                                Message.WriteInteger(Bot.VirtualId);
                                                Message.WriteString(Session.GetHabbo().Username + " Pobre LuisC debe sufrir mucho, si la traes devuelta lo va aturdir, no te puedo pagar.");
                                                Message.WriteInteger(0);
                                                Message.WriteInteger(2);
                                                Message.WriteInteger(0);
                                                Message.WriteInteger(-1);
                                                Session.SendPacket((IServerPacket)Message);
                                                Rp.Money += 25;
                                                Rp.Exp += 30;
                                                Rp.SendUpdate();
                                                Session.GetHabbo().SendPacketWeb((IServerPacket)new PlaySoundComposer("carrete", 2));
                                                Session.GetHabbo().AddFish(3);
                                                Message = (ServerPacket)null;
                                                Message = (ServerPacket)null;
                                            }
                                            else if (fishid < 400)
                                            {
                                                Session.GetHabbo().EatWurm();
                                                Session.SendWhisper("Atrapaste una caracola!", 1);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@blue@*Jalar el Carrete con una caracola*", 0, ThisUser.LastBubble));
                                                ServerPacket Message = new ServerPacket(2704);
                                                Message.WriteInteger(Bot.VirtualId);
                                                Message.WriteString(Session.GetHabbo().Username + " No suelo comprar a menudo las caracolas, no puedo pagar por ellas ahora.");
                                                Message.WriteInteger(0);
                                                Message.WriteInteger(2);
                                                Message.WriteInteger(0);
                                                Message.WriteInteger(-1);
                                                Session.SendPacket((IServerPacket)Message);
                                                Rp.Money += 25;
                                                Rp.Exp += 30;
                                                Rp.SendUpdate();
                                                Session.GetHabbo().SendPacketWeb((IServerPacket)new PlaySoundComposer("carrete", 2));
                                                Session.GetHabbo().AddFish(4);
                                                Message = (ServerPacket)null;
                                                Message = (ServerPacket)null;
                                            }
                                            else if (fishid < 450)
                                            {
                                                Session.GetHabbo().EatWurm();
                                                Session.SendWhisper("Atrapaste una Anguila!", 1);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@blue@*Jalar el Carrete con una Anguila*", 0, ThisUser.LastBubble));
                                                ServerPacket Message = new ServerPacket(2704);
                                                Message.WriteInteger(Bot.VirtualId);
                                                Message.WriteString(Session.GetHabbo().Username + " La energía perfecta para el pueblo, trae más, a ver si montamos una discoteca.");
                                                Message.WriteInteger(0);
                                                Message.WriteInteger(2);
                                                Message.WriteInteger(0);
                                                Message.WriteInteger(-1);
                                                Session.SendPacket((IServerPacket)Message);
                                                Rp.Money += 25;
                                                Rp.Exp += 30;
                                                Rp.SendUpdate();
                                                Session.GetHabbo().AkiledPoints += Convert.ToInt32(premio5);
                                                Session.SendPacket((IServerPacket)new HabboActivityPointNotificationComposer(Session.GetHabbo().AkiledPoints, Convert.ToInt32(premio5), 105));
                                                GameClient gameClient = Session;
                                                string image = icon_monedaoficial;
                                                string[] strArray1 = new string[5]
                                                {
                          "Susseth: le ha enviado ",
                          null,
                          null,
                          null,
                          null
                                                };
                                                int int32 = Convert.ToInt32(premio5);
                                                strArray1[1] = int32.ToString();
                                                strArray1[2] = " ";
                                                strArray1[3] = name_monedaoficial;
                                                strArray1[4] = ", por la pesca de una Anguila.";
                                                string message = string.Concat(strArray1);
                                                ServerPacket serverPacket = RoomNotificationComposer.SendBubble(image, message);
                                                gameClient.SendPacket((IServerPacket)serverPacket);
                                                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                                {
                                                    IQueryAdapter queryAdapter = queryreactor;
                                                    string[] strArray2 = new string[5]
                                                    {
                            "UPDATE users SET vip_points = vip_points + ",
                            null,
                            null,
                            null,
                            null
                                                    };
                                                    int32 = Convert.ToInt32(premio5);
                                                    strArray2[1] = int32.ToString();
                                                    strArray2[2] = " WHERE id = ";
                                                    strArray2[3] = Session.GetHabbo().Id.ToString();
                                                    strArray2[4] = " LIMIT 1";
                                                    string query = string.Concat(strArray2);
                                                    queryAdapter.RunQuery(query);
                                                    queryAdapter = (IQueryAdapter)null;
                                                    strArray2 = (string[])null;
                                                    query = (string)null;
                                                }
                                                Session.GetHabbo().SendPacketWeb((IServerPacket)new PlaySoundComposer("carrete", 2));
                                                Session.GetHabbo().AddFish(5);
                                                Message = (ServerPacket)null;
                                                Message = (ServerPacket)null;
                                                gameClient = (GameClient)null;
                                                image = (string)null;
                                                strArray1 = (string[])null;
                                                message = (string)null;
                                                serverPacket = (ServerPacket)null;
                                            }
                                            else if (fishid < 500)
                                            {
                                                Session.GetHabbo().EatWurm();
                                                Session.SendWhisper("Atrapaste una Estrella de Mar!", 1);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@blue@*Jalar el Carrete con un Estrella de Mar*", 0, ThisUser.LastBubble));
                                                ServerPacket Message = new ServerPacket(2704);
                                                Message.WriteInteger(Bot.VirtualId);
                                                Message.WriteString(Session.GetHabbo().Username + " Pagaré por tu estrella de mar, ve y traele mejor mercancia la próxima vez.");
                                                Message.WriteInteger(0);
                                                Message.WriteInteger(2);
                                                Message.WriteInteger(0);
                                                Message.WriteInteger(-1);
                                                Session.SendPacket((IServerPacket)Message);
                                                Rp.Money += 25;
                                                Rp.Exp += 30;
                                                Rp.SendUpdate();
                                                Session.GetHabbo().AkiledPoints += Convert.ToInt32(premio6);
                                                Session.SendPacket((IServerPacket)new HabboActivityPointNotificationComposer(Session.GetHabbo().AkiledPoints, Convert.ToInt32(premio6), 105));
                                                GameClient gameClient = Session;
                                                string image = icon_monedaoficial;
                                                string[] strArray1 = new string[5]
                                                {
                          "Susseth: le ha enviado ",
                          null,
                          null,
                          null,
                          null
                                                };
                                                int int32 = Convert.ToInt32(premio6);
                                                strArray1[1] = int32.ToString();
                                                strArray1[2] = " ";
                                                strArray1[3] = name_monedaoficial;
                                                strArray1[4] = ", por la pesca de una estrella de mar.";
                                                string message = string.Concat(strArray1);
                                                ServerPacket serverPacket = RoomNotificationComposer.SendBubble(image, message);
                                                gameClient.SendPacket((IServerPacket)serverPacket);
                                                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                                {
                                                    IQueryAdapter queryAdapter = queryreactor;
                                                    string[] strArray2 = new string[5]
                                                    {
                            "UPDATE users SET vip_points = vip_points + ",
                            null,
                            null,
                            null,
                            null
                                                    };
                                                    int32 = Convert.ToInt32(premio6);
                                                    strArray2[1] = int32.ToString();
                                                    strArray2[2] = " WHERE id = ";
                                                    strArray2[3] = Session.GetHabbo().Id.ToString();
                                                    strArray2[4] = " LIMIT 1";
                                                    string query = string.Concat(strArray2);
                                                    queryAdapter.RunQuery(query);
                                                    queryAdapter = (IQueryAdapter)null;
                                                    strArray2 = (string[])null;
                                                    query = (string)null;
                                                }
                                                Session.GetHabbo().SendPacketWeb((IServerPacket)new PlaySoundComposer("carrete", 2));
                                                Session.GetHabbo().AddFish(6);
                                                Message = (ServerPacket)null;
                                                Message = (ServerPacket)null;
                                                gameClient = (GameClient)null;
                                                image = (string)null;
                                                strArray1 = (string[])null;
                                                message = (string)null;
                                                serverPacket = (ServerPacket)null;
                                            }
                                            else if (fishid < 600)
                                            {
                                                Session.GetHabbo().EatWurm();
                                                Session.SendWhisper("Atrapaste un Cangrejo!", 1);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@blue@*Jalar el Carrete con un Cangrejo*", 0, ThisUser.LastBubble));
                                                ServerPacket Message = new ServerPacket(2704);
                                                Message.WriteInteger(Bot.VirtualId);
                                                Message.WriteString(Session.GetHabbo().Username + " Muy rica la carne de Cangrejo, delociosaa uhmmmm, Susseth ha pagado por cangrejo.");
                                                Message.WriteInteger(0);
                                                Message.WriteInteger(2);
                                                Message.WriteInteger(0);
                                                Message.WriteInteger(-1);
                                                Session.SendPacket((IServerPacket)Message);
                                                Rp.Money += 25;
                                                Rp.Exp += 30;
                                                Rp.SendUpdate();
                                                Session.GetHabbo().AkiledPoints += Convert.ToInt32(premio7);
                                                Session.SendPacket((IServerPacket)new HabboActivityPointNotificationComposer(Session.GetHabbo().AkiledPoints, Convert.ToInt32(premio7), 105));
                                                GameClient gameClient = Session;
                                                string image = icon_monedaoficial;
                                                string[] strArray1 = new string[5]
                                                {
                          "Susseth: le ha enviado ",
                          null,
                          null,
                          null,
                          null
                                                };
                                                int int32 = Convert.ToInt32(premio6);
                                                strArray1[1] = int32.ToString();
                                                strArray1[2] = " ";
                                                strArray1[3] = name_monedaoficial;
                                                strArray1[4] = ", por la pesca de un Cangrejo.";
                                                string message = string.Concat(strArray1);
                                                ServerPacket serverPacket = RoomNotificationComposer.SendBubble(image, message);
                                                gameClient.SendPacket((IServerPacket)serverPacket);
                                                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                                {
                                                    IQueryAdapter queryAdapter = queryreactor;
                                                    string[] strArray2 = new string[5]
                                                    {
                            "UPDATE users SET vip_points = vip_points + ",
                            null,
                            null,
                            null,
                            null
                                                    };
                                                    int32 = Convert.ToInt32(premio7);
                                                    strArray2[1] = int32.ToString();
                                                    strArray2[2] = " WHERE id = ";
                                                    strArray2[3] = Session.GetHabbo().Id.ToString();
                                                    strArray2[4] = " LIMIT 1";
                                                    string query = string.Concat(strArray2);
                                                    queryAdapter.RunQuery(query);
                                                    queryAdapter = (IQueryAdapter)null;
                                                    strArray2 = (string[])null;
                                                    query = (string)null;
                                                }
                                                Session.GetHabbo().SendPacketWeb((IServerPacket)new PlaySoundComposer("carrete", 2));
                                                Session.GetHabbo().AddFish(7);
                                                Message = (ServerPacket)null;
                                                Message = (ServerPacket)null;
                                                gameClient = (GameClient)null;
                                                image = (string)null;
                                                strArray1 = (string[])null;
                                                message = (string)null;
                                                serverPacket = (ServerPacket)null;
                                            }
                                            else if (fishid < 650)
                                            {
                                                Session.GetHabbo().EatWurm();
                                                Session.SendWhisper("¡Atrapaste un tiburón! Su caña de pescar ha sudrido un gran dañó debido a su peso excesivo.", 1);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@blue@*Jalar el Carrete con el gran tiburón*", 0, ThisUser.LastBubble));
                                                Session.GetHabbo().SendPacketWeb((IServerPacket)new PlaySoundComposer("carrete", 2));
                                                Rp.Money += 25;
                                                Rp.Exp += 30;
                                                Rp.SendUpdate();
                                                Session.GetHabbo().AkiledPoints += Convert.ToInt32(premio7);
                                                Session.SendPacket((IServerPacket)new HabboActivityPointNotificationComposer(Session.GetHabbo().AkiledPoints, Convert.ToInt32(premio7), 105));
                                                Session.SendPacket((IServerPacket)RoomNotificationComposer.SendBubble(icon_monedaoficial, "Susseth: le ha enviado " + Convert.ToInt32(premio7).ToString() + " " + name_monedaoficial + ", por la pesca de un Tiburon."));
                                                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                                    queryreactor.RunQuery("UPDATE users SET vip_points = vip_points + " + Convert.ToInt32(premio7).ToString() + " WHERE id = " + Session.GetHabbo().Id.ToString() + " LIMIT 1");
                                                Session.GetHabbo().SendPacketWeb((IServerPacket)new PlaySoundComposer("akillegotiburon", 2));
                                                Session.GetHabbo().DestroyAngel(x.Next(0, 13));
                                                Session.GetHabbo().AddFish(8);
                                            }
                                            else if (fishid < 700)
                                            {
                                                Session.GetHabbo().EatWurm();
                                                Session.SendWhisper("Atrapaste un Atún!", 1);
                                                AkiledEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FISH_3_THUNFISH);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@blue@*Jalar el Carrete con un atún*", 0, ThisUser.LastBubble));
                                                ServerPacket Message = new ServerPacket(2704);
                                                Message.WriteInteger(Bot.VirtualId);
                                                Message.WriteString(Session.GetHabbo().Username + " Las personas del pueblo comen mucho Atún, estamos encantado con tu pesca gracias.");
                                                Message.WriteInteger(0);
                                                Message.WriteInteger(2);
                                                Message.WriteInteger(0);
                                                Message.WriteInteger(-1);
                                                Session.SendPacket((IServerPacket)Message);
                                                Rp.Money += 25;
                                                Rp.Exp += 30;
                                                Rp.SendUpdate();
                                                Session.GetHabbo().AkiledPoints += Convert.ToInt32(premio9);
                                                Session.SendPacket((IServerPacket)new HabboActivityPointNotificationComposer(Session.GetHabbo().AkiledPoints, Convert.ToInt32(premio9), 105));
                                                Session.SendPacket((IServerPacket)RoomNotificationComposer.SendBubble(icon_monedaoficial, "Susseth: le ha enviado " + Convert.ToInt32(premio9).ToString() + " " + name_monedaoficial + ", por la pesca de un Atún."));
                                                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                                    queryreactor.RunQuery("UPDATE users SET vip_points = vip_points + " + Convert.ToInt32(premio9).ToString() + " WHERE id = " + Session.GetHabbo().Id.ToString() + " LIMIT 1");
                                                Session.GetHabbo().SendPacketWeb((IServerPacket)new PlaySoundComposer("carrete", 2));
                                                Session.GetHabbo().AddFish(9);
                                                Message = (ServerPacket)null;
                                                Message = (ServerPacket)null;
                                            }
                                            else if (fishid < 750)
                                            {
                                                Session.GetHabbo().EatWurm();
                                                Session.SendWhisper("Atrapaste un Caballito de Mar!", 1);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@blue@*Jalar el Carrete con Caballito de Mar*", 0, ThisUser.LastBubble));
                                                ServerPacket Message = new ServerPacket(2704);
                                                Message.WriteInteger(Bot.VirtualId);
                                                Message.WriteString(Session.GetHabbo().Username + " Sinceramente no se para que pierdes el tiempo, lo siento, sigue pescando.");
                                                Message.WriteInteger(0);
                                                Message.WriteInteger(2);
                                                Message.WriteInteger(0);
                                                Message.WriteInteger(-1);
                                                Session.SendPacket((IServerPacket)Message);
                                                Rp.Money += 25;
                                                Rp.Exp += 30;
                                                Rp.SendUpdate();
                                                Session.GetHabbo().SendPacketWeb((IServerPacket)new PlaySoundComposer("carrete", 2));
                                                Session.GetHabbo().AddFish(10);
                                                Message = (ServerPacket)null;
                                                Message = (ServerPacket)null;
                                            }
                                            else if (fishid < 900)
                                            {
                                                Session.GetHabbo().EatWurm();
                                                Session.SendWhisper("Atrapaste un calzón de Ana!", 1);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@blue@*Jalar el Carrete con el calzón de Ana a tierra*", 0, ThisUser.LastBubble));
                                                ServerPacket Message = new ServerPacket(2704);
                                                Message.WriteInteger(Bot.VirtualId);
                                                Message.WriteString(Session.GetHabbo().Username + " Llévaselo a Zeref y te dara buena recompensa, yo te regalaré " + Convert.ToInt32(premio11).ToString() + " " + name_monedaoficial + "*");
                                                Message.WriteInteger(0);
                                                Message.WriteInteger(2);
                                                Message.WriteInteger(0);
                                                Message.WriteInteger(-1);
                                                Session.SendPacket((IServerPacket)Message);
                                                Rp.Money += 25;
                                                Rp.Exp += 30;
                                                Rp.SendUpdate();
                                                Session.GetHabbo().AkiledPoints += Convert.ToInt32(premio11);
                                                Session.SendPacket((IServerPacket)new HabboActivityPointNotificationComposer(Session.GetHabbo().AkiledPoints, Convert.ToInt32(premio11), 105));
                                                GameClient gameClient = Session;
                                                string image = icon_monedaoficial;
                                                int int32 = Convert.ToInt32(premio11);
                                                string message = "Susseth: le ha enviado " + int32.ToString() + " " + name_monedaoficial;
                                                ServerPacket serverPacket = RoomNotificationComposer.SendBubble(image, message);
                                                gameClient.SendPacket((IServerPacket)serverPacket);
                                                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                                {
                                                    IQueryAdapter queryAdapter = queryreactor;
                                                    string[] strArray = new string[5]
                                                    {
                            "UPDATE users SET vip_points = vip_points + ",
                            null,
                            null,
                            null,
                            null
                                                    };
                                                    int32 = Convert.ToInt32(premio11);
                                                    strArray[1] = int32.ToString();
                                                    strArray[2] = " WHERE id = ";
                                                    strArray[3] = Session.GetHabbo().Id.ToString();
                                                    strArray[4] = " LIMIT 1";
                                                    string query = string.Concat(strArray);
                                                    queryAdapter.RunQuery(query);
                                                    queryAdapter = (IQueryAdapter)null;
                                                    strArray = (string[])null;
                                                    query = (string)null;
                                                }
                                                Session.GetHabbo().SendPacketWeb((IServerPacket)new PlaySoundComposer("carrete", 2));
                                                Session.GetHabbo().AddFish(11);
                                                Message = (ServerPacket)null;
                                                Message = (ServerPacket)null;
                                                gameClient = (GameClient)null;
                                                image = (string)null;
                                                message = (string)null;
                                                serverPacket = (ServerPacket)null;
                                            }
                                            else if (fishid < 920)
                                            {
                                                Session.GetHabbo().EatWurm();
                                                Session.SendWhisper("Atrapaste una Calamar!", 1);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@blue@*Jalar el Carrete con el Calamar*", 0, ThisUser.LastBubble));
                                                ServerPacket Message = new ServerPacket(2704);
                                                Message.WriteInteger(Bot.VirtualId);
                                                Message.WriteString(Session.GetHabbo().Username + " Demasiado rica es la ensalada de Calamar, Sussetha buen chic@!");
                                                Message.WriteInteger(0);
                                                Message.WriteInteger(2);
                                                Message.WriteInteger(0);
                                                Message.WriteInteger(-1);
                                                Session.SendPacket((IServerPacket)Message);
                                                Rp.Money += 25;
                                                Rp.Exp += 30;
                                                Rp.SendUpdate();
                                                Session.GetHabbo().AkiledPoints += Convert.ToInt32(premio12);
                                                Session.SendPacket((IServerPacket)new HabboActivityPointNotificationComposer(Session.GetHabbo().AkiledPoints, Convert.ToInt32(premio12), 105));
                                                GameClient gameClient = Session;
                                                string image = icon_monedaoficial;
                                                string[] strArray1 = new string[5]
                                                {
                          "Susseth: le ha enviado ",
                          null,
                          null,
                          null,
                          null
                                                };
                                                int int32 = Convert.ToInt32(premio12);
                                                strArray1[1] = int32.ToString();
                                                strArray1[2] = " ";
                                                strArray1[3] = name_monedaoficial;
                                                strArray1[4] = ", por la pesca de un Calamar.";
                                                string message = string.Concat(strArray1);
                                                ServerPacket serverPacket = RoomNotificationComposer.SendBubble(image, message);
                                                gameClient.SendPacket((IServerPacket)serverPacket);
                                                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                                {
                                                    IQueryAdapter queryAdapter = queryreactor;
                                                    string[] strArray2 = new string[5]
                                                    {
                            "UPDATE users SET vip_points = vip_points + ",
                            null,
                            null,
                            null,
                            null
                                                    };
                                                    int32 = Convert.ToInt32(premio12);
                                                    strArray2[1] = int32.ToString();
                                                    strArray2[2] = " WHERE id = ";
                                                    strArray2[3] = Session.GetHabbo().Id.ToString();
                                                    strArray2[4] = " LIMIT 1";
                                                    string query = string.Concat(strArray2);
                                                    queryAdapter.RunQuery(query);
                                                    queryAdapter = (IQueryAdapter)null;
                                                    strArray2 = (string[])null;
                                                    query = (string)null;
                                                }
                                                Session.GetHabbo().SendPacketWeb((IServerPacket)new PlaySoundComposer("carrete", 2));
                                                Session.GetHabbo().AddFish(12);
                                                Message = (ServerPacket)null;
                                                Message = (ServerPacket)null;
                                                gameClient = (GameClient)null;
                                                image = (string)null;
                                                strArray1 = (string[])null;
                                                message = (string)null;
                                                serverPacket = (ServerPacket)null;
                                            }
                                            else if (fishid < 950)
                                            {
                                                Session.GetHabbo().EatWurm();
                                                Session.SendWhisper("Atrapaste una Medusa!", 1);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@blue@*Jalar el Carrete con la Medusa*", 0, ThisUser.LastBubble));
                                                ServerPacket Message = new ServerPacket(2704);
                                                Message.WriteInteger(Bot.VirtualId);
                                                Message.WriteString(Session.GetHabbo().Username + " No hay excusa Brr, gaste no se cuanto en la medusa Brr!");
                                                Message.WriteInteger(0);
                                                Message.WriteInteger(2);
                                                Message.WriteInteger(0);
                                                Message.WriteInteger(-1);
                                                Session.SendPacket((IServerPacket)Message);
                                                Rp.Money += 25;
                                                Rp.Exp += 30;
                                                Rp.SendUpdate();
                                                Session.GetHabbo().AkiledPoints += Convert.ToInt32(premio13);
                                                Session.SendPacket((IServerPacket)new HabboActivityPointNotificationComposer(Session.GetHabbo().AkiledPoints, Convert.ToInt32(premio13), 105));
                                                GameClient gameClient = Session;
                                                string image = icon_monedaoficial;
                                                string[] strArray1 = new string[5]
                                                {
                          "Susseth: le ha enviado ",
                          null,
                          null,
                          null,
                          null
                                                };
                                                int int32 = Convert.ToInt32(premio13);
                                                strArray1[1] = int32.ToString();
                                                strArray1[2] = " ";
                                                strArray1[3] = name_monedaoficial;
                                                strArray1[4] = ", por la pesca de una Medusa";
                                                string message = string.Concat(strArray1);
                                                ServerPacket serverPacket = RoomNotificationComposer.SendBubble(image, message);
                                                gameClient.SendPacket((IServerPacket)serverPacket);
                                                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                                {
                                                    IQueryAdapter queryAdapter = queryreactor;
                                                    string[] strArray2 = new string[5]
                                                    {
                            "UPDATE users SET vip_points = vip_points + ",
                            null,
                            null,
                            null,
                            null
                                                    };
                                                    int32 = Convert.ToInt32(premio13);
                                                    strArray2[1] = int32.ToString();
                                                    strArray2[2] = " WHERE id = ";
                                                    strArray2[3] = Session.GetHabbo().Id.ToString();
                                                    strArray2[4] = " LIMIT 1";
                                                    string query = string.Concat(strArray2);
                                                    queryAdapter.RunQuery(query);
                                                    queryAdapter = (IQueryAdapter)null;
                                                    strArray2 = (string[])null;
                                                    query = (string)null;
                                                }
                                                Session.GetHabbo().SendPacketWeb((IServerPacket)new PlaySoundComposer("anuelbrr", 2));
                                                Session.GetHabbo().AddFish(13);
                                                Message = (ServerPacket)null;
                                                Message = (ServerPacket)null;
                                                gameClient = (GameClient)null;
                                                image = (string)null;
                                                strArray1 = (string[])null;
                                                message = (string)null;
                                                serverPacket = (ServerPacket)null;
                                            }
                                            else if (fishid < 980)
                                            {
                                                Session.GetHabbo().EatWurm();
                                                Session.SendWhisper("Atrapaste un cofre con tesoro!", 1);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@blue@*Jalar el Carrete con el cofre con tesoro*", 0, ThisUser.LastBubble));
                                                ServerPacket Message = new ServerPacket(2704);
                                                Message.WriteInteger(Bot.VirtualId);
                                                Message.WriteString(Session.GetHabbo().Username + " Ohhh mi amig@ estos si son verdaderos negocios!");
                                                Message.WriteInteger(0);
                                                Message.WriteInteger(2);
                                                Message.WriteInteger(0);
                                                Message.WriteInteger(-1);
                                                Session.SendPacket((IServerPacket)Message);
                                                Rp.Money += 100;
                                                Rp.Exp += 100;
                                                Rp.SendUpdate();
                                                Session.GetHabbo().AkiledPoints += Convert.ToInt32(premio14);
                                                Session.SendPacket((IServerPacket)new HabboActivityPointNotificationComposer(Session.GetHabbo().AkiledPoints, Convert.ToInt32(premio14), 105));
                                                GameClient gameClient = Session;
                                                string image = icon_monedaoficial;
                                                int int32 = Convert.ToInt32(premio14);
                                                string message = "Susseth: le ha enviado " + int32.ToString() + " " + name_monedaoficial;
                                                ServerPacket serverPacket = RoomNotificationComposer.SendBubble(image, message);
                                                gameClient.SendPacket((IServerPacket)serverPacket);
                                                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                                {
                                                    IQueryAdapter queryAdapter = queryreactor;
                                                    string[] strArray = new string[5]
                                                    {
                            "UPDATE users SET vip_points = vip_points + ",
                            null,
                            null,
                            null,
                            null
                                                    };
                                                    int32 = Convert.ToInt32(premio14);
                                                    strArray[1] = int32.ToString();
                                                    strArray[2] = " WHERE id = ";
                                                    strArray[3] = Session.GetHabbo().Id.ToString();
                                                    strArray[4] = " LIMIT 1";
                                                    string query = string.Concat(strArray);
                                                    queryAdapter.RunQuery(query);
                                                    queryAdapter = (IQueryAdapter)null;
                                                    strArray = (string[])null;
                                                    query = (string)null;
                                                }
                                                Session.GetHabbo().SendPacketWeb((IServerPacket)new PlaySoundComposer("carrete", 2));
                                                Session.GetHabbo().AddFish(14);
                                                Message = (ServerPacket)null;
                                                Message = (ServerPacket)null;
                                                gameClient = (GameClient)null;
                                                image = (string)null;
                                                message = (string)null;
                                                serverPacket = (ServerPacket)null;
                                            }
                                            else if (fishid < 989)
                                            {
                                                Session.GetHabbo().EatWurm();
                                                Session.SendWhisper("Atrapaste un cofre con tesoro griego!", 1);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@blue@*Jalar el Carrete con el cofre con tesoro griego*", 0, ThisUser.LastBubble));
                                                ServerPacket Message = new ServerPacket(2704);
                                                Message.WriteInteger(Bot.VirtualId);
                                                Message.WriteString(Session.GetHabbo().Username + " Ohhh mi amig@ estos si son verdaderos negocios!");
                                                Message.WriteInteger(0);
                                                Message.WriteInteger(2);
                                                Message.WriteInteger(0);
                                                Message.WriteInteger(-1);
                                                Session.SendPacket((IServerPacket)Message);
                                                Rp.Money += 100;
                                                Rp.Exp += 30;
                                                Rp.SendUpdate();
                                                Session.GetHabbo().AkiledPoints += Convert.ToInt32(premio15);
                                                Session.SendPacket((IServerPacket)new HabboActivityPointNotificationComposer(Session.GetHabbo().AkiledPoints, Convert.ToInt32(premio15), 105));
                                                GameClient gameClient = Session;
                                                string image = icon_monedaoficial;
                                                int int32 = Convert.ToInt32(premio15);
                                                string message = "Susseth: le ha enviado " + int32.ToString() + " " + name_monedaoficial;
                                                ServerPacket serverPacket = RoomNotificationComposer.SendBubble(image, message);
                                                gameClient.SendPacket((IServerPacket)serverPacket);
                                                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                                {
                                                    IQueryAdapter queryAdapter = queryreactor;
                                                    string[] strArray = new string[5]
                                                    {
                            "UPDATE users SET vip_points = vip_points + ",
                            null,
                            null,
                            null,
                            null
                                                    };
                                                    int32 = Convert.ToInt32(premio15);
                                                    strArray[1] = int32.ToString();
                                                    strArray[2] = " WHERE id = ";
                                                    strArray[3] = Session.GetHabbo().Id.ToString();
                                                    strArray[4] = " LIMIT 1";
                                                    string query = string.Concat(strArray);
                                                    queryAdapter.RunQuery(query);
                                                    queryAdapter = (IQueryAdapter)null;
                                                    strArray = (string[])null;
                                                    query = (string)null;
                                                }
                                                Session.GetHabbo().SendPacketWeb((IServerPacket)new PlaySoundComposer("carrete", 2));
                                                Session.GetHabbo().AddFish(15);
                                                Message = (ServerPacket)null;
                                                Message = (ServerPacket)null;
                                                gameClient = (GameClient)null;
                                                image = (string)null;
                                                message = (string)null;
                                                serverPacket = (ServerPacket)null;
                                            }
                                            else if (fishid < 995)
                                            {
                                                Session.GetHabbo().EatWurm();
                                                Session.SendWhisper("Atrapaste un cofre con tesoro holandes!", 1);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@blue@*Jalar el Carrete con el cofre con tesoro holandes*", 0, ThisUser.LastBubble));
                                                ServerPacket Message = new ServerPacket(2704);
                                                Message.WriteInteger(Bot.VirtualId);
                                                Message.WriteString(Session.GetHabbo().Username + " Ohhh mi amig@ estos si son verdaderos negocios!");
                                                Message.WriteInteger(0);
                                                Message.WriteInteger(2);
                                                Message.WriteInteger(0);
                                                Message.WriteInteger(-1);
                                                Session.SendPacket((IServerPacket)Message);
                                                Rp.Money += 100;
                                                Rp.Exp += 30;
                                                Rp.SendUpdate();
                                                Session.GetHabbo().AkiledPoints += Convert.ToInt32(premio16);
                                                Session.SendPacket((IServerPacket)new HabboActivityPointNotificationComposer(Session.GetHabbo().AkiledPoints, Convert.ToInt32(premio16), 105));
                                                GameClient gameClient = Session;
                                                string image = icon_monedaoficial;
                                                int int32 = Convert.ToInt32(premio16);
                                                string message = "Susseth: le ha enviado " + int32.ToString() + " " + name_monedaoficial;
                                                ServerPacket serverPacket = RoomNotificationComposer.SendBubble(image, message);
                                                gameClient.SendPacket((IServerPacket)serverPacket);
                                                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                                {
                                                    IQueryAdapter queryAdapter = queryreactor;
                                                    string[] strArray = new string[5]
                                                    {
                            "UPDATE users SET vip_points = vip_points + ",
                            null,
                            null,
                            null,
                            null
                                                    };
                                                    int32 = Convert.ToInt32(premio16);
                                                    strArray[1] = int32.ToString();
                                                    strArray[2] = " WHERE id = ";
                                                    strArray[3] = Session.GetHabbo().Id.ToString();
                                                    strArray[4] = " LIMIT 1";
                                                    string query = string.Concat(strArray);
                                                    queryAdapter.RunQuery(query);
                                                    queryAdapter = (IQueryAdapter)null;
                                                    strArray = (string[])null;
                                                    query = (string)null;
                                                }
                                                Session.GetHabbo().SendPacketWeb((IServerPacket)new PlaySoundComposer("carrete", 2));
                                                Session.GetHabbo().AddFish(16);
                                                Message = (ServerPacket)null;
                                                Message = (ServerPacket)null;
                                                gameClient = (GameClient)null;
                                                image = (string)null;
                                                message = (string)null;
                                                serverPacket = (ServerPacket)null;
                                            }
                                            else
                                            {
                                                Session.SendWhisper("Maldición! Un pez muy pilas, se fue con tu carnada, no te quedes dormid@.", 34);
                                                Session.GetHabbo().EatWurm();
                                            }
                                            x = (Random)null;
                                            x = (Random)null;
                                        }
                                        else
                                            break;
                                    }
                                    if (Session.GetHabbo() == null)
                                    {
                                        xx = (Random)null;
                                        xx = (Random)null;
                                    }
                                    else
                                    {
                                        if (Session.GetHabbo().angelpass < AkiledEnvironment.GetIUnixTimestamp())
                                            Session.SendWhisper("Su pase de pesca ha expirado. Ya no puedes pescar.", 34);
                                        Session.GetHabbo().is_angeln = false;
                                        ThisUser.ApplyEffect(0);
                                        xx = (Random)null;
                                        xx = (Random)null;
                                    }
                                }));
                            }
                        }
                    }
                }
            }
        }
    }
}
