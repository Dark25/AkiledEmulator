
using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Roleplay.Player;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class MinaCommand : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if ((double)Session.GetHabbo().last_mina > (double)AkiledEnvironment.GetUnixTimestamp() - 30.0 && !Session.GetHabbo().HasFuse("override_limit_command"))
            {
                Session.SendWhisper("Debes esperar 30 segundos, para volver a usar el comando", 1);
            }
            else
            {
                RoomUser roomUser = (RoomUser)null;
                RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
                List<string> stringList = new List<string>();
                string room_idmina = AkiledEnvironment.GetConfig().data["room_idmina"];
                string str1 = AkiledEnvironment.GetConfig().data["baseitem_idmina"];
                string item_iddetector = AkiledEnvironment.GetConfig().data["item_iddetector"];
                string str2 = AkiledEnvironment.GetConfig().data["item_idpico"];
                string str3 = AkiledEnvironment.GetConfig().data["item_idexplosivo"];
                string name_monedaoficial = AkiledEnvironment.GetConfig().data["name_monedaoficial"];
                string icon_monedaoficial = AkiledEnvironment.GetConfig().data["icon_monedaoficial"];
                string name = AkiledEnvironment.GetConfig().data["minanamebot"];
                string str4 = AkiledEnvironment.GetConfig().data["minapremio1"];
                string minapremio1name = AkiledEnvironment.GetConfig().data["minapremio1name"];
                string minapremio2 = AkiledEnvironment.GetConfig().data["minapremio2"];
                string minapremio2name = AkiledEnvironment.GetConfig().data["minapremio2name"];
                string minapremio3 = AkiledEnvironment.GetConfig().data["minapremio3"];
                string minapremio3name = AkiledEnvironment.GetConfig().data["minapremio3name"];
                string minapremio4 = AkiledEnvironment.GetConfig().data["minapremio4"];
                string minapremio4name = AkiledEnvironment.GetConfig().data["minapremio4name"];
                string minapremio5 = AkiledEnvironment.GetConfig().data["minapremio5"];
                string minapremio5name = AkiledEnvironment.GetConfig().data["minapremio5name"];
                string minapremio6 = AkiledEnvironment.GetConfig().data["minapremio6"];
                string minapremio6name = AkiledEnvironment.GetConfig().data["minapremio6name"];
                string minapremio7 = AkiledEnvironment.GetConfig().data["minapremio7"];
                string minapremio7name = AkiledEnvironment.GetConfig().data["minapremio7name"];
                string minapremio8 = AkiledEnvironment.GetConfig().data["minapremio8"];
                string minapremio8name = AkiledEnvironment.GetConfig().data["minapremio8name"];
                string minapremio9 = AkiledEnvironment.GetConfig().data["minapremio9"];
                string minapremio9name = AkiledEnvironment.GetConfig().data["minapremio9name"];
                string minapremio10 = AkiledEnvironment.GetConfig().data["minapremio10"];
                string minapremio10name = AkiledEnvironment.GetConfig().data["minapremio10name"];
                string minapremio11 = AkiledEnvironment.GetConfig().data["minapremio11"];
                string minapremio11name = AkiledEnvironment.GetConfig().data["minapremio11name"];
                string minapremio12 = AkiledEnvironment.GetConfig().data["minapremio12"];
                string minapremio12name = AkiledEnvironment.GetConfig().data["minapremio12name"];
                string minapremio13 = AkiledEnvironment.GetConfig().data["minapremio13"];
                string minapremio13name = AkiledEnvironment.GetConfig().data["minapremio13name"];
                string minapremio14 = AkiledEnvironment.GetConfig().data["minapremio14"];
                string minapremio14name = AkiledEnvironment.GetConfig().data["minapremio14name"];
                string minapremio15 = AkiledEnvironment.GetConfig().data["minapremio15"];
                string minapremio15name = AkiledEnvironment.GetConfig().data["minapremio15name"];
                string minapremio16 = AkiledEnvironment.GetConfig().data["minapremio16"];
                string minapremio16name = AkiledEnvironment.GetConfig().data["minapremio16name"];
                roomUser = Room.GetRoomUserManager().GetBotOrPetByName(name);
                if (Session.GetHabbo().CurrentRoomId != Convert.ToInt32(room_idmina))
                {
                    if (!Session.GetHabbo().InRoom)
                        Session.SendMessage((IServerPacket)new RoomForwardComposer(Convert.ToInt32(room_idmina)));
                    else
                        Session.GetHabbo().PrepareRoom(Convert.ToInt32(room_idmina));
                }
                else
                {
                    foreach (Item obj1 in Room.GetGameMap().GetRoomItemForSquare(ThisUser.X, ThisUser.Y).ToList<Item>())
                    {
                        if (obj1.BaseItem == Convert.ToInt32(str1))
                        {
                            if (!Session.GetHabbo().hasdetector)
                            {
                                using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    queryReactor.SetQuery("SELECT * FROM `user_rpitems` WHERE `user_id` = " + Session.GetHabbo().Id.ToString() + " AND `item_id` = '" + Convert.ToInt32(item_iddetector).ToString() + "' LIMIT 1;");
                                    if (queryReactor.GetRow() != null)
                                        Session.GetHabbo().hasdetector = true;
                                }
                            }
                            if (!Session.GetHabbo().hasdetector)
                            {
                                Session.SendWhisper("¡Ops al parecer no tienes un detector de metal, debes Comprar tu detector de matal en la tienda o pregunta a un staff como minar :D", 34);
                                break;
                            }
                            if (!Session.GetHabbo().haspico)
                            {
                                using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    queryReactor.SetQuery("SELECT * FROM `user_rpitems` WHERE `user_id` = " + Session.GetHabbo().Id.ToString() + " AND `item_id` = '" + Convert.ToInt32(str2).ToString() + "' LIMIT 1;");
                                    if (queryReactor.GetRow() != null)
                                        Session.GetHabbo().haspico = true;
                                }
                            }
                            if (!Session.GetHabbo().haspico)
                            {
                                Session.SendWhisper("¡Ops al parecer no tienes un pico, debes encontrar tu propio pico, ¡Compra :D!", 34);
                                break;
                            }
                            if (Session.GetHabbo().explosivo <= -1 || Session.GetHabbo().explosivo == 0)
                            {
                                using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    queryReactor.SetQuery("SELECT * FROM `user_rpitems` WHERE `user_id` = " + Session.GetHabbo().Id.ToString() + " AND `item_id` = '" + Convert.ToInt32(str3).ToString() + "' LIMIT 1;");
                                    DataRow row = queryReactor.GetRow();
                                    Session.GetHabbo().explosivo = row == null ? 0 : (int)row["count"];
                                }
                            }
                            if (Session.GetHabbo().miningpass < AkiledEnvironment.GetIUnixTimestamp())
                                Session.SendWhisper("¡No tienes un pase de miner@! ¡Debes comprar :D!", 1);
                            else if (Session.GetHabbo().onfarm)
                                Session.SendWhisper("¡No puedes minar mientras fabricas explosivos!", 1);
                            else if (Session.GetHabbo().is_mining)
                            {
                                ThisUser.ApplyEffect(0);
                                Session.SendWhisper("Has dejado de Minar.", 1);
                                Session.GetHabbo().is_mining = false;
                                ThisUser.Freeze = false;
                            }
                            else if (Session.GetHabbo().explosivo <= 0)
                            {
                                Session.SendWhisper("No te quedan explosivos! necesitas fábricar los explosivos para ayudar a la excavación.", 34);
                                Session.GetHabbo().is_mining = false;
                                Session.SendWhisper("Has dejado de Minar.", 1);
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
                                Session.GetHabbo().last_mina = AkiledEnvironment.GetIUnixTimestamp();
                                Session.SendWhisper("Sabemos que es un trabajo fuerte, pero ya has comenzado la aventura en la mina!", 34);
                                Session.SendWhisper("te deseamos la mejor suerte en la busquedas de los minerales mas costosos del hotel.", 34);
                                RolePlayer roleplayer = ThisUser.Roleplayer;
                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@blue@* Has comenzado a minar*", 0, ThisUser.LastBubble));
                                Session.GetHabbo().is_mining = true;
                                ThisUser.Freeze = true;
                                ThisUser.ApplyEffect(712);
                                Task.Run((Func<Task>)(async () =>
                                {
                                    Random xx = new Random();
                                    int async_task = xx.Next(0, 100);
                                    Session.GetHabbo().async_angel_id = async_task;
                                    while (Session.GetHabbo().angelpass > AkiledEnvironment.GetIUnixTimestamp())
                                    {
                                        if (Session.GetHabbo() == null || !Session.GetHabbo().is_mining)
                                        {
                                            if (Session.GetHabbo() != null)
                                            {
                                                ThisUser.ApplyEffect(0);
                                                break;
                                            }
                                            break;
                                        }
                                        if (Session.GetHabbo().CurrentRoomId != Convert.ToInt32(room_idmina) || !Session.GetHabbo().InRoom)
                                        {
                                            ThisUser.ApplyEffect(0);
                                            Session.GetHabbo().is_mining = false;
                                            break;
                                        }
                                        Random x = new Random();
                                        float nextfish = (float)(x.Next(40, 180) / 3);
                                        float i;
                                        for (i = 0.0f; (double)i < (double)nextfish; ++i)
                                        {
                                            await Task.Delay(3000);
                                            if (Session.GetHabbo() == null || !Session.GetHabbo().is_mining)
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
                                                    Session.SendNotification("Ya no estás trabajando en la mina porque te cambiaste de sala.");
                                                    ThisUser.ApplyEffect(0);
                                                    Session.GetHabbo().is_mining = false;
                                                    ThisUser2 = (RoomUser)null;
                                                    ThisUser2 = (RoomUser)null;
                                                    break;
                                                }
                                            }
                                            catch
                                            {
                                                i = 9999f;
                                                Session.SendNotification("Ya no estás trabajando en la mina porque te cambiaste de sala.");
                                                ThisUser.ApplyEffect(0);
                                                Session.GetHabbo().is_mining = false;
                                                break;
                                            }
                                            if (Session.GetHabbo().CurrentRoomId != Convert.ToInt32(room_idmina) || !Session.GetHabbo().InRoom)
                                            {
                                                i = 9999f;
                                                Session.SendNotification("Ya no estás trabajando en la mina porque te cambiaste de sala.");
                                                ThisUser.ApplyEffect(0);
                                                Session.GetHabbo().is_mining = false;
                                                break;
                                            }
                                        }
                                        if ((double)i <= 9000.0 && Session.GetHabbo() != null && Session.GetHabbo().is_mining && Session.GetHabbo().async_angel_id == async_task)
                                        {
                                            if (Session.GetHabbo().CurrentRoomId != Convert.ToInt32(room_idmina) || !Session.GetHabbo().InRoom)
                                            {
                                                ThisUser.ApplyEffect(0);
                                                Session.GetHabbo().is_mining = false;
                                                break;
                                            }
                                            int fishid = x.Next(0, 1700);
                                            ThisUser = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
                                            if (Session.GetHabbo().explosivo <= 0)
                                            {
                                                Session.SendWhisper("No te quedan explosivos! debes fabricar algunos explosivos para seguir trabajando.", 34);
                                                ThisUser.ApplyEffect(0);
                                                ThisUser.Freeze = false;
                                                Session.GetHabbo().is_mining = false;
                                            }
                                            if (UserRoom.IsAsleep)
                                            {
                                                ThisUser.ApplyEffect(0);
                                                Session.SendWhisper("Has dejado de Minar por estar ausente.", 34);
                                                Session.GetHabbo().is_mining = false;
                                                ThisUser.Freeze = false;
                                            }
                                            else if (fishid % 4 == 0)
                                            {
                                                Session.SendWhisper("¡Tu detector de metales ha sufrido una imantacion muy fuerte y la ha dejado inservible!", 1);
                                                Session.SendWhisper("¡Tu pico tambien se ha dañado completamente, deberas usar otro!", 1);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@red@*Una magnetita ha descargado muy fuerte en tu detector*", 0, ThisUser.LastBubble));
                                                Session.GetHabbo().DestroyDetector(x.Next(0, 5));
                                                Session.GetHabbo().DestroyPico(x.Next(0, 5));
                                                DataRow GetPromotion4 = (DataRow)null;
                                                using (IQueryAdapter queryAdapter = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                                {
                                                    queryAdapter.SetQuery("SELECT * FROM `user_rpitems` WHERE `user_id` = " + Session.GetHabbo().Id.ToString() + " AND `item_id` = '" + Convert.ToInt32(item_iddetector).ToString() + "' LIMIT 1;");
                                                    GetPromotion4 = queryAdapter.GetRow();
                                                    if (GetPromotion4 != null)
                                                    {
                                                        Session.GetHabbo().haspico = true;
                                                        Session.GetHabbo().hasdetector = true;
                                                        ThisUser.Freeze = true;
                                                        ThisUser.ApplyEffect(712);
                                                        Session.GetHabbo().is_mining = true;
                                                    }
                                                    else
                                                    {
                                                        Session.SendWhisper("Dejaste de minar.", 1);
                                                        ThisUser.ApplyEffect(0);
                                                        ThisUser.Freeze = false;
                                                        Session.GetHabbo().haspico = false;
                                                        Session.GetHabbo().hasdetector = false;
                                                        Session.GetHabbo().is_mining = false;
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
                                                Session.GetHabbo().Leftexplosivo();
                                                Session.SendWhisper("Felicidades has encontrado un " + minapremio1name + "!", 1);
                                                roleplayer.Money += 25;
                                                roleplayer.Exp += 25;
                                                roleplayer.SendUpdate();
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@green@*Limpiar y guardar " + minapremio1name + " en mi mochila*", 0, ThisUser.LastBubble));
                                                Session.GetHabbo().SendPacketWeb((IServerPacket)new PlaySoundComposer("brillante", 2));
                                            }
                                            else if (fishid < 300)
                                            {
                                                Session.GetHabbo().Leftexplosivo();
                                                Session.SendWhisper("Felicidades has encontrado un " + minapremio2name + "!", 1);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@green@*Limpiar y guardar " + minapremio2name + " en mi mochila*", 0, ThisUser.LastBubble));
                                                Session.GetHabbo().SendPacketWeb((IServerPacket)new PlaySoundComposer("brillante", 2));
                                                roleplayer.Money += 25;
                                                roleplayer.Exp += 25;
                                                roleplayer.SendUpdate();
                                                ItemData ItemData = (ItemData)null;
                                                if (!AkiledEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(minapremio2), out ItemData))
                                                {
                                                    xx = (Random)null;
                                                    xx = (Random)null;
                                                    return;
                                                }
                                                List<Item> Items = ItemFactory.CreateMultipleItems(ItemData, Session.GetHabbo(), "", 1);
                                                foreach (Item obj2 in Items)
                                                {
                                                    Item obj = obj2;
                                                    Item PurchasedItem = obj;
                                                    if (Session.GetHabbo().GetInventoryComponent().TryAddItem(PurchasedItem))
                                                    {
                                                        Session.SendPacket((IServerPacket)new FurniListNotificationComposer(PurchasedItem.Id, 1));
                                                        Session.SendPacket((IServerPacket)RoomNotificationComposer.SendBubble("icons/" + ItemData.ItemName + "_icon", "Acabas de recibir un:\n\n " + minapremio2name + ".\n\n¡Corre, " + Session.GetHabbo().Username + ", revisa tu inventario, hay algo nuevo al parecer! \n\nClick Aquí", "inventory/open/furni"));
                                                    }
                                                    PurchasedItem = (Item)null;
                                                    PurchasedItem = (Item)null;
                                                    obj = (Item)null;
                                                }
                                                ItemData = (ItemData)null;
                                                Items = (List<Item>)null;
                                                ItemData = (ItemData)null;
                                                Items = (List<Item>)null;
                                            }
                                            else if (fishid < 400)
                                            {
                                                Session.GetHabbo().Leftexplosivo();
                                                Session.SendWhisper("Felicidades has encontrado un " + minapremio3name + "!", 1);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@green@*Limpiar y guardar " + minapremio3name + " en mi mochila*", 0, ThisUser.LastBubble));
                                                Session.GetHabbo().SendPacketWeb((IServerPacket)new PlaySoundComposer("brillante", 2));
                                                roleplayer.Money += 25;
                                                roleplayer.Exp += 25;
                                                roleplayer.SendUpdate();
                                                ItemData ItemData = (ItemData)null;
                                                if (!AkiledEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(minapremio3), out ItemData))
                                                {
                                                    xx = (Random)null;
                                                    xx = (Random)null;
                                                    return;
                                                }
                                                List<Item> Items = ItemFactory.CreateMultipleItems(ItemData, Session.GetHabbo(), "", 1);
                                                foreach (Item obj2 in Items)
                                                {
                                                    Item obj = obj2;
                                                    Item PurchasedItem = obj;
                                                    if (Session.GetHabbo().GetInventoryComponent().TryAddItem(PurchasedItem))
                                                    {
                                                        Session.SendPacket((IServerPacket)new FurniListNotificationComposer(PurchasedItem.Id, 1));
                                                        Session.SendPacket((IServerPacket)RoomNotificationComposer.SendBubble("icons/" + ItemData.ItemName + "_icon", "Acabas de recibir un:\n\n " + minapremio3name + ".\n\n¡Corre, " + Session.GetHabbo().Username + ", revisa tu inventario, hay algo nuevo al parecer! \n\nClick Aquí", "inventory/open/furni"));
                                                    }
                                                    PurchasedItem = (Item)null;
                                                    PurchasedItem = (Item)null;
                                                    obj = (Item)null;
                                                }
                                                ItemData = (ItemData)null;
                                                Items = (List<Item>)null;
                                                ItemData = (ItemData)null;
                                                Items = (List<Item>)null;
                                            }
                                            else if (fishid < 500)
                                            {
                                                Session.GetHabbo().Leftexplosivo();
                                                Session.SendWhisper("Felicidades has encontrado un " + minapremio4name + "!", 1);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@green@*Limpiar y guardar " + minapremio4name + " en mi mochila*", 0, ThisUser.LastBubble));
                                                GameClient gameClient = Session;
                                                string image = icon_monedaoficial;
                                                string[] strArray1 = new string[7];
                                                strArray1[0] = "Has recibido ";
                                                int int32 = Convert.ToInt32(minapremio4);
                                                strArray1[1] = int32.ToString();
                                                strArray1[2] = " ";
                                                strArray1[3] = "Esmeraldas";
                                                strArray1[4] = ", por encontrar un ";
                                                strArray1[5] = minapremio4name;
                                                strArray1[6] = ".";
                                                string message = string.Concat(strArray1);
                                                ServerPacket serverPacket = RoomNotificationComposer.SendBubble(image, message);
                                                gameClient.SendPacket((IServerPacket)serverPacket);
                                                Session.GetHabbo().Duckets += int32;
                                                Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, int32));
                                                roleplayer.Money += 25;
                                                roleplayer.Exp += 25;
                                                roleplayer.SendUpdate();
                                                gameClient = (GameClient)null;
                                                image = (string)null;
                                                strArray1 = (string[])null;
                                                message = (string)null;
                                                serverPacket = (ServerPacket)null;
                                            }
                                            else if (fishid < 600)
                                            {
                                                Session.GetHabbo().Leftexplosivo();
                                                Session.SendWhisper("Felicidades has encontrado un " + minapremio5name + "!", 1);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@green@*Limpiar y guardar " + minapremio5name + " en mi mochila*", 0, ThisUser.LastBubble));
                                                GameClient gameClient = Session;
                                                string image = icon_monedaoficial;
                                                string[] strArray1 = new string[7];
                                                strArray1[0] = "Has recibido ";
                                                int int32 = Convert.ToInt32(minapremio5);
                                                strArray1[1] = int32.ToString();
                                                strArray1[2] = " ";
                                                strArray1[3] = "Esmeraldas";
                                                strArray1[4] = ", por encontrar un ";
                                                strArray1[5] = minapremio4name;
                                                strArray1[6] = ".";
                                                string message = string.Concat(strArray1);
                                                ServerPacket serverPacket = RoomNotificationComposer.SendBubble(image, message);
                                                gameClient.SendPacket((IServerPacket)serverPacket);
                                                Session.GetHabbo().Duckets += int32;
                                                Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, int32));
                                                roleplayer.Money += 25;
                                                roleplayer.Exp += 25;
                                                roleplayer.SendUpdate();
                                                gameClient = (GameClient)null;
                                                image = (string)null;
                                                strArray1 = (string[])null;
                                                message = (string)null;
                                                serverPacket = (ServerPacket)null;
                                            }
                                            else if (fishid < 700)
                                            {
                                                Session.GetHabbo().Leftexplosivo();
                                                Session.SendWhisper("Felicidades has encontrado un " + minapremio6name + "!", 1);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@green@*Limpiar y guardar " + minapremio6name + " en mi mochila*", 0, ThisUser.LastBubble));
                                                Session.GetHabbo().SendPacketWeb((IServerPacket)new PlaySoundComposer("brillante", 2));
                                                roleplayer.Money += 25;
                                                roleplayer.Exp += 25;
                                                roleplayer.SendUpdate();
                                                ItemData ItemData = (ItemData)null;
                                                if (!AkiledEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(minapremio6), out ItemData))
                                                {
                                                    xx = (Random)null;
                                                    xx = (Random)null;
                                                    return;
                                                }
                                                List<Item> Items = ItemFactory.CreateMultipleItems(ItemData, Session.GetHabbo(), "", 1);
                                                foreach (Item obj2 in Items)
                                                {
                                                    Item obj = obj2;
                                                    Item PurchasedItem = obj;
                                                    if (Session.GetHabbo().GetInventoryComponent().TryAddItem(PurchasedItem))
                                                    {
                                                        Session.SendPacket((IServerPacket)new FurniListNotificationComposer(PurchasedItem.Id, 1));
                                                        Session.SendPacket((IServerPacket)RoomNotificationComposer.SendBubble("icons/" + ItemData.ItemName + "_icon", "Acabas de recibir un:\n\n " + minapremio6name + ".\n\n¡Corre, " + Session.GetHabbo().Username + ", revisa tu inventario, hay algo nuevo al parecer! \n\nClick Aquí", "inventory/open/furni"));
                                                    }
                                                    PurchasedItem = (Item)null;
                                                    PurchasedItem = (Item)null;
                                                    obj = (Item)null;
                                                }
                                                ItemData = (ItemData)null;
                                                Items = (List<Item>)null;
                                                ItemData = (ItemData)null;
                                                Items = (List<Item>)null;
                                            }
                                            else if (fishid < 800)
                                            {
                                                Session.GetHabbo().Leftexplosivo();
                                                Session.SendWhisper("Felicidades has encontrado un " + minapremio7name + "!", 1);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@green@*Limpiar y guardar " + minapremio7name + " en mi mochila*", 0, ThisUser.LastBubble));
                                                GameClient gameClient = Session;
                                                string image = icon_monedaoficial;
                                                string[] strArray1 = new string[7];
                                                strArray1[0] = "Has recibido ";
                                                int int32 = Convert.ToInt32(minapremio7);
                                                strArray1[1] = int32.ToString();
                                                strArray1[2] = " ";
                                                strArray1[3] = "Esmeraldas";
                                                strArray1[4] = ", por encontrar un ";
                                                strArray1[5] = minapremio7name;
                                                strArray1[6] = ".";
                                                string message = string.Concat(strArray1);
                                                ServerPacket serverPacket = RoomNotificationComposer.SendBubble(image, message);
                                                gameClient.SendPacket((IServerPacket)serverPacket);
                                                Session.GetHabbo().Duckets += int32;
                                                Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, int32));
                                                roleplayer.Money += 25;
                                                roleplayer.Exp += 25;
                                                roleplayer.SendUpdate();
                                                gameClient = (GameClient)null;
                                                image = (string)null;
                                                strArray1 = (string[])null;
                                                message = (string)null;
                                                serverPacket = (ServerPacket)null;
                                            }
                                            else if (fishid < 900)
                                            {
                                                Session.GetHabbo().Leftexplosivo();
                                                Session.SendWhisper("Felicidades has encontrado un " + minapremio8name + "!", 1);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@green@*Limpiar y guardar " + minapremio8name + " en mi mochila*", 0, ThisUser.LastBubble));
                                                Session.GetHabbo().SendPacketWeb((IServerPacket)new PlaySoundComposer("brillante", 2));
                                                roleplayer.Money += 25;
                                                roleplayer.Exp += 25;
                                                roleplayer.SendUpdate();
                                                ItemData ItemData = (ItemData)null;
                                                if (!AkiledEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(minapremio8), out ItemData))
                                                {
                                                    xx = (Random)null;
                                                    xx = (Random)null;
                                                    return;
                                                }
                                                List<Item> Items = ItemFactory.CreateMultipleItems(ItemData, Session.GetHabbo(), "", 1);
                                                foreach (Item obj2 in Items)
                                                {
                                                    Item obj = obj2;
                                                    Item PurchasedItem = obj;
                                                    if (Session.GetHabbo().GetInventoryComponent().TryAddItem(PurchasedItem))
                                                    {
                                                        Session.SendPacket((IServerPacket)new FurniListNotificationComposer(PurchasedItem.Id, 1));
                                                        Session.SendPacket((IServerPacket)RoomNotificationComposer.SendBubble("icons/" + ItemData.ItemName + "_icon", "Acabas de recibir un:\n\n " + minapremio8name + ".\n\n¡Corre, " + Session.GetHabbo().Username + ", revisa tu inventario, hay algo nuevo al parecer! \n\nClick Aquí", "inventory/open/furni"));
                                                    }
                                                    PurchasedItem = (Item)null;
                                                    PurchasedItem = (Item)null;
                                                    obj = (Item)null;
                                                }
                                                ItemData = (ItemData)null;
                                                Items = (List<Item>)null;
                                                ItemData = (ItemData)null;
                                                Items = (List<Item>)null;
                                            }
                                            else if (fishid < 1000)
                                            {
                                                Session.GetHabbo().Leftexplosivo();
                                                Session.SendWhisper("Felicidades has encontrado un " + minapremio9name + "!", 1);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@green@*Limpiar y guardar " + minapremio9name + " en mi mochila*", 0, ThisUser.LastBubble));
                                                Session.GetHabbo().SendPacketWeb((IServerPacket)new PlaySoundComposer("brillante", 2));
                                                roleplayer.Money += 25;
                                                roleplayer.Exp += 25;
                                                roleplayer.SendUpdate();
                                                ItemData ItemData = (ItemData)null;
                                                if (!AkiledEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(minapremio9), out ItemData))
                                                {
                                                    xx = (Random)null;
                                                    xx = (Random)null;
                                                    return;
                                                }
                                                List<Item> Items = ItemFactory.CreateMultipleItems(ItemData, Session.GetHabbo(), "", 1);
                                                foreach (Item obj2 in Items)
                                                {
                                                    Item obj = obj2;
                                                    Item PurchasedItem = obj;
                                                    if (Session.GetHabbo().GetInventoryComponent().TryAddItem(PurchasedItem))
                                                    {
                                                        Session.SendPacket((IServerPacket)new FurniListNotificationComposer(PurchasedItem.Id, 1));
                                                        Session.SendPacket((IServerPacket)RoomNotificationComposer.SendBubble("icons/" + ItemData.ItemName + "_icon", "Acabas de recibir un:\n\n " + minapremio9name + ".\n\n¡Corre, " + Session.GetHabbo().Username + ", revisa tu inventario, hay algo nuevo al parecer! \n\nClick Aquí", "inventory/open/furni"));
                                                    }
                                                    PurchasedItem = (Item)null;
                                                    PurchasedItem = (Item)null;
                                                    obj = (Item)null;
                                                }
                                                ItemData = (ItemData)null;
                                                Items = (List<Item>)null;
                                                ItemData = (ItemData)null;
                                                Items = (List<Item>)null;
                                            }
                                            else if (fishid < 1100)
                                            {
                                                Session.GetHabbo().Leftexplosivo();
                                                Session.SendWhisper("Felicidades has encontrado un " + minapremio10name + "!", 1);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@green@*Limpiar y guardar " + minapremio10name + " en mi mochila*", 0, ThisUser.LastBubble));
                                                Session.GetHabbo().SendPacketWeb((IServerPacket)new PlaySoundComposer("brillante", 2));
                                                roleplayer.Money += 25;
                                                roleplayer.Exp += 25;
                                                roleplayer.SendUpdate();
                                                ItemData ItemData = (ItemData)null;
                                                if (!AkiledEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(minapremio10), out ItemData))
                                                {
                                                    xx = (Random)null;
                                                    xx = (Random)null;
                                                    return;
                                                }
                                                List<Item> Items = ItemFactory.CreateMultipleItems(ItemData, Session.GetHabbo(), "", 1);
                                                foreach (Item obj2 in Items)
                                                {
                                                    Item obj = obj2;
                                                    Item PurchasedItem = obj;
                                                    if (Session.GetHabbo().GetInventoryComponent().TryAddItem(PurchasedItem))
                                                    {
                                                        Session.SendPacket((IServerPacket)new FurniListNotificationComposer(PurchasedItem.Id, 1));
                                                        Session.SendPacket((IServerPacket)RoomNotificationComposer.SendBubble("icons/" + ItemData.ItemName + "_icon", "Acabas de recibir un:\n\n " + minapremio10name + ".\n\n¡Corre, " + Session.GetHabbo().Username + ", revisa tu inventario, hay algo nuevo al parecer! \n\nClick Aquí", "inventory/open/furni"));
                                                    }
                                                    PurchasedItem = (Item)null;
                                                    PurchasedItem = (Item)null;
                                                    obj = (Item)null;
                                                }
                                                ItemData = (ItemData)null;
                                                Items = (List<Item>)null;
                                                ItemData = (ItemData)null;
                                                Items = (List<Item>)null;
                                            }
                                            else if (fishid < 1200)
                                            {
                                                Session.GetHabbo().Leftexplosivo();
                                                Session.SendWhisper("Felicidades has encontrado un " + minapremio11name + "!", 1);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@green@*Limpiar y guardar " + minapremio11name + " en mi mochila*", 0, ThisUser.LastBubble));
                                                Session.GetHabbo().SendPacketWeb((IServerPacket)new PlaySoundComposer("brillante", 2));
                                                roleplayer.Money += 25;
                                                roleplayer.Exp += 25;
                                                roleplayer.SendUpdate();
                                                ItemData ItemData = (ItemData)null;
                                                if (!AkiledEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(minapremio11), out ItemData))
                                                {
                                                    xx = (Random)null;
                                                    xx = (Random)null;
                                                    return;
                                                }
                                                List<Item> Items = ItemFactory.CreateMultipleItems(ItemData, Session.GetHabbo(), "", 1);
                                                foreach (Item obj2 in Items)
                                                {
                                                    Item obj = obj2;
                                                    Item PurchasedItem = obj;
                                                    if (Session.GetHabbo().GetInventoryComponent().TryAddItem(PurchasedItem))
                                                    {
                                                        Session.SendPacket((IServerPacket)new FurniListNotificationComposer(PurchasedItem.Id, 1));
                                                        Session.SendPacket((IServerPacket)RoomNotificationComposer.SendBubble("icons/" + ItemData.ItemName + "_icon", "Acabas de recibir un:\n\n " + minapremio11name + ".\n\n¡Corre, " + Session.GetHabbo().Username + ", revisa tu inventario, hay algo nuevo al parecer! \n\nClick Aquí", "inventory/open/furni"));
                                                    }
                                                    PurchasedItem = (Item)null;
                                                    PurchasedItem = (Item)null;
                                                    obj = (Item)null;
                                                }
                                                ItemData = (ItemData)null;
                                                Items = (List<Item>)null;
                                                ItemData = (ItemData)null;
                                                Items = (List<Item>)null;
                                            }
                                            else if (fishid < 1300)
                                            {
                                                Session.GetHabbo().Leftexplosivo();
                                                Session.SendWhisper("Felicidades has encontrado un " + minapremio12name + "!", 1);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@green@*Limpiar y guardar " + minapremio12name + " en mi mochila*", 0, ThisUser.LastBubble));
                                                Session.GetHabbo().SendPacketWeb((IServerPacket)new PlaySoundComposer("brillante", 2));
                                                roleplayer.Money += 25;
                                                roleplayer.Exp += 25;
                                                roleplayer.SendUpdate();
                                                ItemData ItemData = (ItemData)null;
                                                if (!AkiledEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(minapremio12), out ItemData))
                                                {
                                                    xx = (Random)null;
                                                    xx = (Random)null;
                                                    return;
                                                }
                                                List<Item> Items = ItemFactory.CreateMultipleItems(ItemData, Session.GetHabbo(), "", 1);
                                                foreach (Item obj2 in Items)
                                                {
                                                    Item obj = obj2;
                                                    Item PurchasedItem = obj;
                                                    if (Session.GetHabbo().GetInventoryComponent().TryAddItem(PurchasedItem))
                                                    {
                                                        Session.SendPacket((IServerPacket)new FurniListNotificationComposer(PurchasedItem.Id, 1));
                                                        Session.SendPacket((IServerPacket)RoomNotificationComposer.SendBubble("icons/" + ItemData.ItemName + "_icon", "Acabas de recibir un:\n\n " + minapremio12name + ".\n\n¡Corre, " + Session.GetHabbo().Username + ", revisa tu inventario, hay algo nuevo al parecer! \n\nClick Aquí", "inventory/open/furni"));
                                                    }
                                                    PurchasedItem = (Item)null;
                                                    PurchasedItem = (Item)null;
                                                    obj = (Item)null;
                                                }
                                                ItemData = (ItemData)null;
                                                Items = (List<Item>)null;
                                                ItemData = (ItemData)null;
                                                Items = (List<Item>)null;
                                            }
                                            else if (fishid < 1400)
                                            {
                                                Session.GetHabbo().Leftexplosivo();
                                                Session.SendWhisper("Felicidades has encontrado un " + minapremio13name + "!", 1);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@green@*Limpiar y guardar " + minapremio13name + " en mi mochila*", 0, ThisUser.LastBubble));
                                                Session.GetHabbo().SendPacketWeb((IServerPacket)new PlaySoundComposer("brillante", 2));
                                                roleplayer.Money += 25;
                                                roleplayer.Exp += 25;
                                                roleplayer.SendUpdate();
                                                ItemData ItemData = (ItemData)null;
                                                if (!AkiledEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(minapremio13), out ItemData))
                                                {
                                                    xx = (Random)null;
                                                    xx = (Random)null;
                                                    return;
                                                }
                                                List<Item> Items = ItemFactory.CreateMultipleItems(ItemData, Session.GetHabbo(), "", 1);
                                                foreach (Item obj2 in Items)
                                                {
                                                    Item obj = obj2;
                                                    Item PurchasedItem = obj;
                                                    if (Session.GetHabbo().GetInventoryComponent().TryAddItem(PurchasedItem))
                                                    {
                                                        Session.SendPacket((IServerPacket)new FurniListNotificationComposer(PurchasedItem.Id, 1));
                                                        Session.SendPacket((IServerPacket)RoomNotificationComposer.SendBubble("icons/" + ItemData.ItemName + "_icon", "Acabas de recibir un:\n\n " + minapremio13name + ".\n\n¡Corre, " + Session.GetHabbo().Username + ", revisa tu inventario, hay algo nuevo al parecer! \n\nClick Aquí", "inventory/open/furni"));
                                                    }
                                                    PurchasedItem = (Item)null;
                                                    PurchasedItem = (Item)null;
                                                    obj = (Item)null;
                                                }
                                                ItemData = (ItemData)null;
                                                Items = (List<Item>)null;
                                                ItemData = (ItemData)null;
                                                Items = (List<Item>)null;
                                            }
                                            else if (fishid < 1500)
                                            {
                                                Session.GetHabbo().Leftexplosivo();
                                                Session.SendWhisper("Felicidades has encontrado un " + minapremio14name + "!", 1);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@green@*Limpiar y guardar " + minapremio14name + " en mi mochila*", 0, ThisUser.LastBubble));
                                                Session.GetHabbo().SendPacketWeb((IServerPacket)new PlaySoundComposer("brillante", 2));
                                                roleplayer.Money += 25;
                                                roleplayer.Exp += 25;
                                                roleplayer.SendUpdate();
                                                ItemData ItemData = (ItemData)null;
                                                if (!AkiledEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(minapremio14), out ItemData))
                                                {
                                                    xx = (Random)null;
                                                    xx = (Random)null;
                                                    return;
                                                }
                                                List<Item> Items = ItemFactory.CreateMultipleItems(ItemData, Session.GetHabbo(), "", 1);
                                                foreach (Item obj2 in Items)
                                                {
                                                    Item obj = obj2;
                                                    Item PurchasedItem = obj;
                                                    if (Session.GetHabbo().GetInventoryComponent().TryAddItem(PurchasedItem))
                                                    {
                                                        Session.SendPacket((IServerPacket)new FurniListNotificationComposer(PurchasedItem.Id, 1));
                                                        Session.SendPacket((IServerPacket)RoomNotificationComposer.SendBubble("icons/" + ItemData.ItemName + "_icon", "Acabas de recibir un:\n\n " + minapremio14name + ".\n\n¡Corre, " + Session.GetHabbo().Username + ", revisa tu inventario, hay algo nuevo al parecer! \n\nClick Aquí", "inventory/open/furni"));
                                                    }
                                                    PurchasedItem = (Item)null;
                                                    PurchasedItem = (Item)null;
                                                    obj = (Item)null;
                                                }
                                                ItemData = (ItemData)null;
                                                Items = (List<Item>)null;
                                                ItemData = (ItemData)null;
                                                Items = (List<Item>)null;
                                            }
                                            else if (fishid < 1600)
                                            {
                                                Session.GetHabbo().Leftexplosivo();
                                                Session.SendWhisper("Felicidades has encontrado un " + minapremio15name + "!", 1);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@green@*Limpiar y guardar " + minapremio15name + " en mi mochila*", 0, ThisUser.LastBubble));
                                                GameClient gameClient = Session;
                                                string image = icon_monedaoficial;
                                                string[] strArray1 = new string[7];
                                                strArray1[0] = "Has recibido ";
                                                int int32 = Convert.ToInt32(minapremio15);
                                                strArray1[1] = int32.ToString();
                                                strArray1[2] = " ";
                                                strArray1[3] = "Esmeraldas";
                                                strArray1[4] = ", por encontrar un ";
                                                strArray1[5] = minapremio7name;
                                                strArray1[6] = ".";
                                                string message = string.Concat(strArray1);
                                                ServerPacket serverPacket = RoomNotificationComposer.SendBubble(image, message);
                                                gameClient.SendPacket((IServerPacket)serverPacket);
                                                Session.GetHabbo().Duckets += int32;
                                                Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, int32));
                                                roleplayer.Money += 25;
                                                roleplayer.Exp += 25;
                                                roleplayer.SendUpdate();
                                                gameClient = (GameClient)null;
                                                image = (string)null;
                                                strArray1 = (string[])null;
                                                message = (string)null;
                                                serverPacket = (ServerPacket)null;
                                            }
                                            else if (fishid < 1700)
                                            {
                                                Session.GetHabbo().Leftexplosivo();
                                                Session.SendWhisper("Felicidades has encontrado un " + minapremio16name + "!", 1);
                                                Room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@green@*Limpiar y guardar " + minapremio16name + " en mi mochila*", 0, ThisUser.LastBubble));
                                                Session.GetHabbo().SendPacketWeb((IServerPacket)new PlaySoundComposer("brillante", 2));
                                                roleplayer.Money += 25;
                                                roleplayer.Exp += 25;
                                                roleplayer.SendUpdate();
                                                ItemData ItemData = (ItemData)null;
                                                if (!AkiledEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(minapremio16), out ItemData))
                                                {
                                                    xx = (Random)null;
                                                    xx = (Random)null;
                                                    return;
                                                }
                                                List<Item> Items = ItemFactory.CreateMultipleItems(ItemData, Session.GetHabbo(), "", 1);
                                                foreach (Item obj2 in Items)
                                                {
                                                    Item obj = obj2;
                                                    Item PurchasedItem = obj;
                                                    if (Session.GetHabbo().GetInventoryComponent().TryAddItem(PurchasedItem))
                                                    {
                                                        Session.SendPacket((IServerPacket)new FurniListNotificationComposer(PurchasedItem.Id, 1));
                                                        Session.SendPacket((IServerPacket)RoomNotificationComposer.SendBubble("icons/" + ItemData.ItemName + "_icon", "Acabas de recibir un:\n\n " + minapremio16name + ".\n\n¡Corre, " + Session.GetHabbo().Username + ", revisa tu inventario, hay algo nuevo al parecer! \n\nClick Aquí", "inventory/open/furni"));
                                                    }
                                                    PurchasedItem = (Item)null;
                                                    PurchasedItem = (Item)null;
                                                    obj = (Item)null;
                                                }
                                                ItemData = (ItemData)null;
                                                Items = (List<Item>)null;
                                                ItemData = (ItemData)null;
                                                Items = (List<Item>)null;
                                            }
                                            else
                                            {
                                                Session.SendWhisper("No has tenido éxito en tu perforación, debido a que tu explosivo ha fallado en el intento.", 1);
                                                Session.GetHabbo().Leftexplosivo();
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
                                            Session.SendWhisper("Su pase de mineria ha expirado. Ya no puedes minar.", 1);
                                        Session.GetHabbo().is_mining = false;
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
