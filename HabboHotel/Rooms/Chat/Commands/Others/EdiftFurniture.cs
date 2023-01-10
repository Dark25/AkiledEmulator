using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class EdiftFurniture : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            string name_monedaoficial = (AkiledEnvironment.GetConfig().data["name_monedaoficial"]);
            RoomUser RUser = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            List<Item> Items = Room.GetGameMap().GetRoomItemForSquare(RUser.X, RUser.Y);
            if (Params.Length == 1 || Params[1] == "cmd")
            {
                Session.SendPacket(new NuxAlertComposer("habbopages/itemutilidad.txt"));
                return;
            }
            String Type = Params[1].ToLower();
            int numeroint = 0, FurnitureID = 0;
            String FurnitureName = "";
            String inhe = "";
            DataRow Item = null;
            String opcion = "";
            switch (Type)
            {
                case "newprice":
                    {
                        try
                        {
                            numeroint = Convert.ToInt32(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.GetRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("INSERT catalog_rares_history VALUES ('','230230','" + IItem.BaseItem + "','" + FurnitureName + "','" + FurnitureName + "','0','0','" + numeroint + "','0','0','0','0','0','0','0',CURRENT_TIMESTAMP,'','" + Session.GetHabbo().Username + "','','','','0','" + numeroint + "');");
                                    dbClient.RunQuery("UPDATE `catalog_rares_history` SET `last_diamonds` = '" + numeroint + "' WHERE `item_id` = '" + FurnitureID + "' and rarecat_principal=1 LIMIT 1");
                                }
                                UserRoom.SendWhisperChat("Nuevo precio del rare en la web del Item: " + FurnitureID + " editada con éxito (Cantidad Ingresada: " + numeroint.ToString() + ")");
                            }
                            AkiledEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ha ocurrido un error (Ingrese números válidos)");
                        }
                    }
                    break;
                case "setrare":
                    {
                        try
                        {
                            numeroint = Convert.ToInt32(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.GetRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("INSERT catalog_rares_history VALUES ('','260260','" + IItem.BaseItem + "','" + FurnitureName + "','" + FurnitureName + "','0','0','" + numeroint + "','0','0','0','0','0','0','0',CURRENT_TIMESTAMP,'','" + Session.GetHabbo().Username + "','','','','0','" + numeroint + "');");
                                    //dbClient.RunQuery("UPDATE `catalog_rares_history` SET `last_diamonds` = '" + numeroint + "' WHERE `item_id` = '" + FurnitureID + "' and rarecat_principal=1 LIMIT 1");
                                }
                                UserRoom.SendWhisperChat("Nuevo precio del rare en la web del Item: " + FurnitureID + " editada con éxito (Cantidad Ingresada: " + numeroint.ToString() + ")");
                            }
                            AkiledEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ha ocurrido un error (Ingrese números válidos)");
                        }
                    }
                    break;

                case "pkakas":
                    {
                        try
                        {
                            numeroint = Convert.ToInt32(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.GetRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `catalog_items` SET `cost_diamonds` = '" + numeroint + "' WHERE `item_id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                UserRoom.SendWhisperChat("Has cambiado el precio en " + name_monedaoficial + " del Item: " + FurnitureID + " editada con éxito (Cantidad Ingresada: " + numeroint.ToString() + ")");
                            }
                            AkiledEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ha ocurrido un error (Ingrese números válidos)");
                        }
                    }

                    break;
                case "pdiamonds":
                    {
                        try
                        {
                            numeroint = Convert.ToInt32(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.GetRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `catalog_items` SET `cost_pixels` = '" + numeroint + "' WHERE `item_id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                UserRoom.SendWhisperChat("Has cambiado el precio en diamantes del Item: " + FurnitureID + " editada con éxito (Cantidad Ingresada: " + numeroint.ToString() + ")");
                            }
                            AkiledEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ha ocurrido un error (Ingrese números válidos)");
                        }
                    }

                    break;
                case "pcredits":
                    {
                        try
                        {
                            numeroint = Convert.ToInt32(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.GetRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `catalog_items` SET `cost_credits` = '" + numeroint + "' WHERE `item_id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                UserRoom.SendWhisperChat("Has cambiado el precio en creditos del Item: " + FurnitureID + " editada con éxito (Cantidad Ingresada: " + numeroint.ToString() + ")");
                            }
                            AkiledEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ha ocurrido un error (Ingrese números válidos)");
                        }
                    }
                    break;
                case "preciokk":
                    {
                        try
                        {
                            numeroint = Convert.ToInt32(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.GetRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `catalog_rares_history` SET `cost_diamonds` = '" + numeroint + "' WHERE `item_id` = '" + FurnitureID + "' and `principal`=1 LIMIT 1");
                                }
                                UserRoom.SendWhisperChat("Has cambiado el precio del rare en la web del Item: " + FurnitureID + " editada con éxito (Cantidad Ingresada: " + numeroint.ToString() + ")");
                            }
                            AkiledEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ha ocurrido un error (Ingrese números válidos)");
                        }
                    }
                    break;
                case "preciokkltd":
                    {
                        try
                        {
                            numeroint = Convert.ToInt32(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.GetRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `catalog_rares_history` SET `old_cost_diamonds` = '" + numeroint + "' WHERE `item_id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                UserRoom.SendWhisperChat("Has cambiado el precio del rare en la web del Item: " + FurnitureID + " editada con éxito (Cantidad Ingresada: " + numeroint.ToString() + ")");
                            }
                            AkiledEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ha ocurrido un error (Ingrese números válidos)");
                        }
                    }
                    break;
                case "setlastmonth":
                    {
                        try
                        {
                            numeroint = Convert.ToInt32(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.GetRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `catalog_rares_history` SET `last_month` = '" + numeroint + "' WHERE `item_id` = '" + FurnitureID + "' and `principal`=1 LIMIT 1");
                                }
                                UserRoom.SendWhisperChat("Has puesto este rare como principal del item: " + FurnitureID + " editada con éxito (Cantidad Ingresada: " + numeroint.ToString() + ")");
                            }
                            AkiledEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ha ocurrido un error (Ingrese números válidos)");
                        }
                    }
                    break;
                case "setcat":
                    {
                        try
                        {
                            numeroint = Convert.ToInt32(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.GetRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `catalog_rares_history` SET `categoria` = '" + numeroint + "' WHERE `item_id` = '" + FurnitureID + "' and `principal`=1 LIMIT 1");
                                }
                                UserRoom.SendWhisperChat("Has cambiado la categoria del rare en la web del Item: " + FurnitureID + " editada con éxito (Cantidad Ingresada: " + numeroint.ToString() + ")");
                            }
                            AkiledEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ha ocurrido un error (Ingrese números válidos)");
                        }
                    }
                    break;
                case "ltdstack":
                    {
                        try
                        {
                            numeroint = Convert.ToInt32(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.GetRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `catalog_items` SET `limited_stack` = '" + numeroint + "' WHERE `item_id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                UserRoom.SendWhisperChat("Ha cambiado la cantidad de LTD disponibles para vender del Item: " + FurnitureID + " editada con éxito (Cantidad Ingresada: " + numeroint.ToString() + ")");
                            }
                            AkiledEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ha ocurrido un error (Ingrese números válidos)");
                        }
                    }
                    break;
                case "ltdsell":
                    {
                        try
                        {
                            numeroint = Convert.ToInt32(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.GetRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `catalog_items` SET `limited_sells` = '" + numeroint + "' WHERE `item_id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                UserRoom.SendWhisperChat("Ha cambiado la cantidad de LTD Vendidos del Item: " + FurnitureID + " editada con éxito (Cantidad Ingresada: " + numeroint.ToString() + ")");
                            }
                            AkiledEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ha ocurrido un error (Ingrese números válidos)");
                        }
                    }
                    break;
                case "setpage":
                    {
                        try
                        {
                            numeroint = Convert.ToInt32(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.GetRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `catalog_items` SET `page_id` = '" + numeroint + "' WHERE `item_id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                UserRoom.SendWhisperChat("Cambio de pagina del Item: " + FurnitureID + " editada con éxito (Id de pagina ingresado: " + numeroint.ToString() + ")");
                            }
                            AkiledEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ha ocurrido un error (Ingrese números válidos)");
                        }
                    }
                    break;

                case "setreceta":
                    {
                        try
                        {
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT item_name FROM furniture WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.GetRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureName = Convert.ToString(Item[0]);
                                    dbClient.RunQuery("INSERT INTO crafting_items VALUES ('" + FurnitureName + "');");
                                }
                                UserRoom.SendWhisperChat("El item se agrego a las recetas crafting");
                            }
                            AkiledEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ha ocurrido un error.");
                        }
                    }
                    break;
                case "countltd":
                    {
                        foreach (Item IItem in Items.ToList())
                        {
                            if (IItem == null)
                                continue;
                            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("SELECT COUNT(*) FROM items_limited WHERE base_item = '" + IItem.BaseItem + "' LIMIT 1");
                                int unidades = dbClient.GetInteger();
                                FurnitureID = unidades;
                            }
                            UserRoom.SendWhisperChat("En el hotel hay: " + FurnitureID + " , unidades de este furni.");
                        }
                    }
                    break;
                case "count":
                    {
                        foreach (Item IItem in Items.ToList())
                        {
                            if (IItem == null)
                                continue;
                            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("SELECT COUNT(*) FROM items WHERE base_item = '" + IItem.BaseItem + "' LIMIT 1");
                                int unidades = dbClient.GetInteger();
                                FurnitureID = unidades;
                            }
                            UserRoom.SendWhisperChat("En el hotel hay: " + FurnitureID + " , unidades de este furni.");
                        }
                    }
                    break;
                case "id":
                    {
                        foreach (Item IItem in Items.ToList())
                        {
                            if (IItem == null)
                                continue;
                            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                Item = dbClient.GetRow();
                                FurnitureID = Convert.ToInt32(Item[0]);
                            }
                            UserRoom.SendWhisperChat("El ID del item es: (" + FurnitureID + ")");
                        }
                    }
                    break;
                case "width":
                    {
                        try
                        {
                            numeroint = Convert.ToInt32(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.GetRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `width` = '" + numeroint + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                UserRoom.SendWhisperChat("Anchura del Item: " + FurnitureID + " editada con éxito (Valor de anchura ingresado: " + numeroint.ToString() + ")");
                            }
                            AkiledEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ha ocurrido un error (Ingrese números válidos)");
                        }
                    }
                    break;
                case "length":
                    {
                        try
                        {
                            numeroint = Convert.ToInt32(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.GetRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `length` = '" + numeroint + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                UserRoom.SendWhisperChat("Longitud del Item: " + FurnitureID + " editada con éxito (Valor de longitud ingresado: " + numeroint.ToString() + ")");
                            }
                            AkiledEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ha ocurrido un error (Ingrese números válidos)");
                        }
                    }
                    break;
                case "effect":
                    {
                        try
                        {
                            numeroint = Convert.ToInt32(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.GetRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `effect_id` = '" + numeroint + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                UserRoom.SendWhisperChat("Efecto del Item: " + FurnitureID + " editada con éxito (Valor de Efecto ingresado: " + numeroint.ToString() + ")");
                            }
                            AkiledEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ha ocurrido un error (Ingrese números válidos)");
                        }
                    }
                    break;
                case "wired":
                    {
                        try
                        {
                            numeroint = Convert.ToInt32(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.GetRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `wired_id` = '" + numeroint + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                UserRoom.SendWhisperChat("Wired ID del Item: " + FurnitureID + " editada con éxito (Valor de Wired ID ingresado: " + numeroint.ToString() + ")");
                            }
                            AkiledEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ha ocurrido un error (Ingrese números válidos)");
                        }
                    }
                    break;
                case "height":
                    {
                        try
                        {
                            inhe = Convert.ToString(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.GetRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `stack_height` = '" + inhe + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                UserRoom.SendWhisperChat("Altura del Item: " + FurnitureID + " editada con éxito (Valor de altura ingresado: " + inhe.ToString() + ")");
                            }
                            AkiledEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ha ocurrido un error (Ingrese números válidos)");
                        }
                    }
                    break;


                case "heightajust":
                    {
                        try
                        {
                            inhe = Convert.ToString(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.GetRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `height_adjustable` = '" + inhe + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                UserRoom.SendWhisperChat("Altura ajustable del Item: " + FurnitureID + " editada con éxito (Valor de Altura ajustable ingresada: " + inhe.ToString() + ")");
                            }
                            AkiledEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ha ocurrido un error (Ingrese números válidos)");
                        }
                    }
                    break;
                case "interactioncount":
                    {
                        try
                        {
                            numeroint = Convert.ToInt32(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.GetRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `interaction_modes_count` = '" + numeroint + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                UserRoom.SendWhisperChat("Numero de interacciones del Item: " + FurnitureID + " editado con éxito (Valor ingresado: " + numeroint.ToString() + ")");
                            }
                            AkiledEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ha ocurrido un error (Ingrese números válidos)");
                        }
                    }
                    break;
                case "vendingid":
                    {
                        try
                        {
                            inhe = Convert.ToString(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.GetRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `vending_ids` = '" + inhe.ToString() + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                UserRoom.SendWhisperChat("Numero de interacciones del Item: " + FurnitureID + " editado con éxito (Valor ingresado: " + inhe.ToString() + ")");
                            }
                            AkiledEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ha ocurrido un error (Ingrese números válidos)");
                        }
                    }
                    break;
                case "cansit":
                    {
                        try
                        {
                            opcion = Params[2].ToLower();
                            if (!opcion.Equals("si") && !opcion.Equals("no"))
                            {
                                UserRoom.SendWhisperChat("Ingresa una opción valida (si/no)");
                                return;
                            }
                            if (opcion.Equals("si"))
                                opcion = "1";
                            else if (opcion.Equals("no"))
                                opcion = "0";
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.GetRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `can_sit` = '" + opcion + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                UserRoom.SendWhisperChat("can_sit del Item: " + FurnitureID + " editado con éxito");
                            }
                            AkiledEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ha ocurrido un error.");
                        }
                    }
                    break;
                case "canstack":
                    {
                        try
                        {
                            opcion = Params[2].ToLower();
                            if (!opcion.Equals("si") && !opcion.Equals("no"))
                            {
                                UserRoom.SendWhisperChat("Ingresa una opción valida (si/no)");
                                return;
                            }
                            if (opcion.Equals("si"))
                                opcion = "1";
                            else if (opcion.Equals("no"))
                                opcion = "0";
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.GetRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `can_stack` = '" + opcion + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                UserRoom.SendWhisperChat("can_stack del Item: " + FurnitureID + " editado con éxito");
                            }
                            AkiledEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ha ocurrido un error.");
                        }
                    }
                    break;
                case "canwalk":
                    {
                        try
                        {
                            opcion = Params[2].ToLower();
                            if (!opcion.Equals("si") && !opcion.Equals("no"))
                            {
                                UserRoom.SendWhisperChat("Ingresa una opción valida (si/no)");
                                return;
                            }
                            if (opcion.Equals("si"))
                                opcion = "1";
                            else if (opcion.Equals("no"))
                                opcion = "0";
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.GetRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `is_walkable` = '" + opcion + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                UserRoom.SendWhisperChat("can_walk del Item: " + FurnitureID + " editado con éxito");
                            }
                            AkiledEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ha ocurrido un error.");
                        }
                    }
                    break;

                case "cantrade":
                    {
                        try
                        {
                            opcion = Params[2].ToLower();
                            if (!opcion.Equals("si") && !opcion.Equals("no"))
                            {
                                UserRoom.SendWhisperChat("Ingresa una opción valida (si/no)");
                                return;
                            }
                            if (opcion.Equals("si"))
                                opcion = "1";
                            else if (opcion.Equals("no"))
                                opcion = "0";
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.GetRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `allow_trade` = '" + opcion + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                UserRoom.SendWhisperChat("tradeo del Item: " + FurnitureID + " editado con éxito");
                            }
                            AkiledEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ha ocurrido un error.");
                        }
                    }
                    break;
                case "mercadillo":
                    {
                        try
                        {
                            opcion = Params[2].ToLower();
                            if (!opcion.Equals("si") && !opcion.Equals("no"))
                            {
                                UserRoom.SendWhisperChat("Ingresa una opción valida (si/no)");
                                return;
                            }
                            if (opcion.Equals("si"))
                                opcion = "1";
                            else if (opcion.Equals("no"))
                                opcion = "0";
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.GetRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `is_rare` = '" + opcion + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                UserRoom.SendWhisperChat("Opción de venta en el mercadillo del Item: " + FurnitureID + " editado con éxito");
                            }
                            AkiledEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ha ocurrido un error.");
                        }
                    }
                    break;
                case "interaction":
                    {
                        try
                        {
                            opcion = Params[2].ToLower();
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.GetRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `interaction_type` = '" + opcion + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                UserRoom.SendWhisperChat("Interacción del Item: " + FurnitureID + " editada con éxito. (Valor ingresado: " + opcion + ")");
                            }
                            AkiledEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ha ocurrido un error.");
                        }
                    }
                    break;
                case "furniname":
                    {
                        try
                        {
                            inhe = Convert.ToString(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.GetRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `public_name` = '" + inhe.ToString() + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                UserRoom.SendWhisperChat("Nombre del Item: " + FurnitureID + " editada con éxito. (Valor ingresado: " + inhe.ToString() + ")");
                            }
                            AkiledEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ha ocurrido un error.");
                        }
                    }
                    break;
                case "cataname":
                    {
                        try
                        {
                            inhe = Convert.ToString(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.GetRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `catalog_items` SET `catalog_name` = '" + inhe.ToString() + "' WHERE `item_id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                UserRoom.SendWhisperChat("Nombre del Item en el cata: " + FurnitureID + " editada con éxito. (Valor ingresado: " + inhe.ToString() + ")");
                            }
                            AkiledEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ha ocurrido un error.");
                        }
                    }
                    break;
                case "catanameweb":
                    {
                        try
                        {
                            inhe = Convert.ToString(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.GetRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `catalog_rares_history` SET `catalog_nameweb` = '" + inhe.ToString() + "' WHERE `item_id` = '" + FurnitureID + "'  LIMIT 1");
                                }
                                UserRoom.SendWhisperChat("Nombre del Item en el cataweb: " + FurnitureID + " editada con éxito. (Valor ingresado: " + inhe.ToString() + ")");
                            }
                            AkiledEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ha ocurrido un error.");
                        }
                    }
                    break;
                case "catanameitem":
                    {
                        try
                        {
                            inhe = Convert.ToString(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.GetRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `catalog_rares_history` SET `catalog_name` = '" + inhe.ToString() + "' WHERE `item_id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                UserRoom.SendWhisperChat("Nombre del Item en el cataweb: " + FurnitureID + " editada con éxito. (Valor ingresado: " + inhe.ToString() + ")");
                            }
                            AkiledEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Ha ocurrido un error.");
                        }
                    }
                    break;
                default:
                    {
                        UserRoom.SendWhisperChat("La opción ingresada no existe, para saber las opciones decir :item cmd");
                        return;
                    }

            }

        }
    }
}
