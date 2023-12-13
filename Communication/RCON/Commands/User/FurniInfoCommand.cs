using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Akiled.Communication.RCON.Commands.User
{
    internal class FurniInfoCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            if (!int.TryParse(parameters[0], out int Userid))
                return false;

            if (Userid == 0)
                return false;

            GameClient Session = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Userid);

            if (Session == null)
                return false;

            Room Room = Session.GetHabbo().CurrentRoom;

            if (Room == null || !Room.CheckRights(Session, true))
                return false;


            if (parameters.Length == 1) 
            {
                var itemid = "";
                RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
                List<Item> Items = Room.GetGameMap().GetRoomItemForSquare(User.X, User.Y);

                if (Items.Count == 0)
                {
                    Session.SendWhisper("Debes colocarte encima del mobiliario (furni) para poder ver su información");
                    return false;
                }

                foreach (Item _item in Items)
                {
                    DataRow ItemData = null;
                    DataRow FurniData = null;
                    DataRow UserData = null;
                    DataRow RoomData = null;
                    int Mobi = _item.Id;

                    using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("SELECT `id`,`user_id`,`room_id`,`base_item` FROM items WHERE `id` = @id LIMIT 1");
                        dbClient.AddParameter("id", Mobi);
                        ItemData = dbClient.GetRow();
                    }
                    if (ItemData == null)
                    {
                        Session.SendNotification("¡Vaya, no hay ningún furni con esa identificación (" + Mobi + ")!");
                        return false;
                    }

                    using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("SELECT * FROM `furniture` WHERE `id` = '" + Convert.ToInt32(ItemData["base_item"]) + "' LIMIT 1");
                        FurniData = dbClient.GetRow();
                        if (FurniData == null)
                        {
                            Session.SendWhisper("Desafortunadamente, no se ha encontrado ningún registro asociado a este mobiliario (furni)");
                            return false;
                        }
                    }
                    using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("SELECT * FROM `users` WHERE `id` = '" + Convert.ToInt32(ItemData["user_id"]) + "' LIMIT 1");
                        UserData = dbClient.GetRow();
                        if (UserData == null)
                        {
                            Session.SendWhisper("El Dueño de furni no encontrado en la base de datos.");
                            return false;
                        }
                    }
                    using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        if (Convert.ToInt32(ItemData["room_id"]) != 0 || Convert.ToInt32(ItemData["room_id"]) != null)
                        {
                            dbClient.SetQuery("SELECT * FROM `rooms` WHERE `id` = '" + Convert.ToInt32(ItemData["room_id"]) + "' LIMIT 1");
                            RoomData = dbClient.GetRow();
                            if (RoomData == null)
                            {
                                Session.SendWhisper("El Dueño de furni no encontrado en la base de datos.");
                                return false;
                            }
                        }
                    }

                    StringBuilder HabboInfo = new StringBuilder();
                    HabboInfo.Append("<b>Información de mobiliario:</b>\r");
                    HabboInfo.Append("<b>ID del mobiliario: </b>" + Convert.ToInt32(Mobi) + "\r");
                    HabboInfo.Append("<b>Dueño: </b>" + UserData["username"] + "  (" + Convert.ToInt32(UserData["id"]) + ")" + "\r");
                    HabboInfo.Append("\r");
                    HabboInfo.Append("<b>Información en Furniture:</b>\r");
                    HabboInfo.Append("<b>ID del mobiliario:</b> " + Convert.ToInt32(FurniData["id"]) + "\r");
                    HabboInfo.Append("<b>Classname:</b> " + FurniData["item_name"] + "\r");
                    HabboInfo.Append("<b>Nombre Público: </b>" + FurniData["public_name"] + "\r\r");

                    HabboInfo.Append("<b>Informacion de la Sala:</b>\r");
                    if (Convert.ToInt32(ItemData["room_id"]) > 0)
                    {
                        HabboInfo.Append("<b>Nombre de la sala:</b> " + RoomData["caption"] + "\r");
                        HabboInfo.Append("<b>ID de la sala:</b> " + RoomData["id"] + "\r");
                        DataRow DonoQuarto = null;
                        using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT `id`,`username` FROM users WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("id", (string)RoomData["owner"]);
                            DonoQuarto = dbClient.GetRow();
                        }

                        HabboInfo.Append("<b>Usuarios en la sala: </b>" + RoomData["users_now"] + "\r");
                    }
                    else
                    {
                        HabboInfo.Append("<b>El furni está en inventario..</b>");
                    }

                    Session.SendNotification(HabboInfo.ToString());
                }
            }
            return true;
        }

    }
}

