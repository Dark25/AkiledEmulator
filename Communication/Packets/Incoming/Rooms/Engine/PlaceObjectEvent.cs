namespace Akiled.Communication.Packets.Incoming.Structure
{
    using Akiled.Communication.Packets.Outgoing;
    using Akiled.Communication.Packets.Outgoing.Structure;
    using Akiled.Database.Interfaces;
    using Akiled.HabboHotel.GameClients;
    using Akiled.HabboHotel.Items;
    using Akiled.HabboHotel.Quests;
    using Akiled.HabboHotel.Rooms;
    using Akiled.HabboHotel.Rooms.Wired;
    using Akiled.HabboHotel.Users;
    using System;
    using System.Linq;

    internal class PlaceObjectEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
                return;
            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null || !room.CheckRights(Session))
            {
                if (Session.GetConnection().IsWebSocket)
                    Session.SendNotification("Disculpe, pero no puede colocar este furni aqui!");
                if (Session.GetConnection().IsWebSocket)
                    return;
                Session.SendPacket((IServerPacket)new RoomNotificationComposer("furni_placement_error", "message", "${room.error.cant_set_not_owner}"));
            }
            else if (room.RoomData.SellPrice > 0)
            {
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("roomsell.error.7", Session.Langue));
            }
            else
            {
                string[] strArray = Packet.PopString().Split(' ');
                int result1 = 0;
                if (!int.TryParse(strArray[0], out result1) || result1 <= 0)
                    return;
                Item obj1 = Session.GetHabbo().GetInventoryComponent().GetItem(result1);
                if (obj1 == null)
                    return;
                if (obj1.GetBaseItem().InteractionType == InteractionType.actionshowmessage || obj1.GetBaseItem().InteractionType == InteractionType.wf_act_bot_talk || obj1.GetBaseItem().InteractionType == InteractionType.wf_act_bot_talk_to_avatar || obj1.GetBaseItem().InteractionType == InteractionType.superwired)
                {
                    if (!Session.GetHabbo().HasFuse("override_limit_wired_ms") && room.GetRoomItemHandler().GetFloor.Where<Item>((Func<Item, bool>)(i => i.GetBaseItem().InteractionType == InteractionType.actionshowmessage)).ToList<Item>().Count > 10)
                    {
                        Session.SendNotification("No puedes usar mas de 10 wireds de este tipo, comunícate con algún staff si necesitas usar más de esta cantidad!");
                        return;
                    }
                    if (!Session.GetHabbo().HasFuse("override_limit_wired_ms") && room.GetRoomItemHandler().GetFloor.Where<Item>((Func<Item, bool>)(i => i.GetBaseItem().InteractionType == InteractionType.wf_act_bot_talk)).ToList<Item>().Count > 10)
                    {
                        Session.SendNotification("No puedes usar mas de 10 wireds de este tipo, comunícate con algún staff si necesitas usar más de esta cantidad!");
                        return;
                    }
                    if (!Session.GetHabbo().HasFuse("override_limit_wired_ms") && room.GetRoomItemHandler().GetFloor.Where<Item>((Func<Item, bool>)(i => i.GetBaseItem().InteractionType == InteractionType.wf_act_bot_talk_to_avatar)).ToList<Item>().Count > 10)
                    {
                        Session.SendNotification("No puedes usar mas de 10 wireds de este tipo, comunícate con algún staff si necesitas usar más de esta cantidad!");
                        return;
                    }
                    if (!Session.GetHabbo().HasFuse("override_limit_wired_ms") && room.GetRoomItemHandler().GetFloor.Where<Item>((Func<Item, bool>)(i => i.GetBaseItem().InteractionType == InteractionType.superwired)).ToList<Item>().Count > 10)
                    {
                        Session.SendNotification("No puedes usar mas de 10 wireds de este tipo, comunícate con algún staff si necesitas usar más de esta cantidad!");
                        return;
                    }
                }
                if (obj1.GetBaseItem().InteractionType == InteractionType.MOODLIGHT)
                {
                    MoodlightData moodlightData = room.MoodlightData;
                    if (moodlightData != null && room.GetRoomItemHandler().GetItem(moodlightData.ItemId) != null)
                    {
                        Session.SendNotification("No se pueden tener mas de (1) regulador(es) en la sala!");
                        return;
                    }
                }
                if (obj1.GetBaseItem().InteractionType == InteractionType.BADGE_TROC)
                {
                    if (Session.GetHabbo().GetBadgeComponent().HasBadge(obj1.ExtraData))
                    {
                        Session.SendNotification("Ya tienes esta placa!");
                    }
                    else
                    {
                        using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                            queryReactor.RunQuery("DELETE FROM items WHERE id = " + result1.ToString());
                        Session.GetHabbo().GetInventoryComponent().RemoveItem(result1);
                        Session.GetHabbo().GetBadgeComponent().GiveBadge(obj1.ExtraData, 0, true);
                        Session.SendPacket(new ReceiveBadgeComposer(obj1.ExtraData));
                        Session.SendNotification("Ya has recibido esta placa: " + obj1.ExtraData + " !");
                    }
                }

                else if (!obj1.IsWallItem)
                {
                    int result2;
                    int result3;
                    int result4;
                    if (strArray.Length < 4 || !int.TryParse(strArray[1], out result2) || !int.TryParse(strArray[2], out result3) || !int.TryParse(strArray[3], out result4))
                        return;
                    if (Session.GetHabbo().forceRot > -1)
                        result4 = Session.GetHabbo().forceRot;
                    Item obj2 = new Item(obj1.Id, obj1.OwnerId, room.Id, obj1.BaseItem, obj1.ExtraData, obj1.LimitedNo, obj1.LimitedTot, result2, result3, 0.0, result4, "", room);
                    if (room.GetRoomItemHandler().SetFloorItem(Session, obj2, result2, result3, result4, true, false, true))
                    {
                        using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                            queryReactor.RunQuery("UPDATE items SET room_id = " + room.Id.ToString() + ", user_id = " + obj2.OwnerId.ToString() + " WHERE id = " + result1.ToString());
                        Session.GetHabbo().GetInventoryComponent().RemoveItem(result1);
                        if (WiredUtillity.TypeIsWired(obj1.GetBaseItem().InteractionType))
                            WiredSaver.HandleDefaultSave(obj1.Id, room, obj2);
                        if (Session.GetHabbo().forceUse > -1)
                            obj2.Interactor.OnTrigger(Session, obj2, 0, true);
                        if (Session.GetHabbo().forceOpenGift)
                        {
                            if (obj2.GetBaseItem().InteractionType == InteractionType.EXTRABOX)
                                ItemExtrabox.OpenExtrabox(Session, obj2, room);
                            else if (obj2.GetBaseItem().InteractionType == InteractionType.DELUXEBOX)
                                ItemExtrabox.OpenDeluxeBox(Session, obj2, room);
                            else if (obj2.GetBaseItem().InteractionType == InteractionType.LEGENDBOX)
                                ItemExtrabox.OpenLegendBox(Session, obj2, room);
                            else if (obj2.GetBaseItem().InteractionType == InteractionType.BADGEBOX)
                                ItemExtrabox.OpenBadgeBox(Session, obj2, room);
                        }
                        AkiledEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_PLACE);
                    }
                    else
                        Session.SendPacket((IServerPacket)new RoomNotificationComposer("furni_placement_error", "message", "${room.error.cant_set_item}"));
                }
                else
                {
                    if (!obj1.IsWallItem)
                        return;
                    string[] data = new string[strArray.Length - 1];
                    for (int index = 1; index < strArray.Length; ++index)
                        data[index - 1] = strArray[index];
                    string position = string.Empty;
                    if (PlaceObjectEvent.TrySetWallItem2(Session.GetHabbo(), obj1, data, out position))
                    {
                        try
                        {
                            Item obj3 = new Item(obj1.Id, obj1.OwnerId, room.Id, obj1.BaseItem, obj1.ExtraData, obj1.LimitedNo, obj1.LimitedTot, 0, 0, 0.0, 0, position, room);
                            if (room.GetRoomItemHandler().SetWallItem(Session, obj3))
                            {
                                using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                    queryReactor.RunQuery("UPDATE items SET room_id = " + room.Id.ToString() + ", user_id = " + obj3.OwnerId.ToString() + " WHERE id = " + result1.ToString());
                                Session.GetHabbo().GetInventoryComponent().RemoveItem(result1);
                            }
                        }
                        catch
                        {
                            Session.SendPacket((IServerPacket)new RoomNotificationComposer("furni_placement_error", "message", "${room.error.cant_set_not_owner}"));
                        }
                    }
                    else
                        Session.SendPacket((IServerPacket)new RoomNotificationComposer("furni_placement_error", "message", "${room.error.cant_set_not_owner}"));
                }
            }
        }

        private static bool TrySetWallItem(string[] data, out string position)
        {
            if (data.Length != 3 || !data[0].StartsWith(":w=") || !data[1].StartsWith("l=") || data[2] != "r" && data[2] != "l")
            {
                position = (string)null;
                return false;
            }
            string str1 = data[0].Substring(3, data[0].Length - 3);
            string str2 = data[1].Substring(2, data[1].Length - 2);
            if (!str1.Contains(",") || !str2.Contains(","))
            {
                position = (string)null;
                return false;
            }
            int result1 = 0;
            int result2 = 0;
            int result3 = 0;
            int result4 = 0;
            int.TryParse(str1.Split(',')[0], out result1);
            int.TryParse(str1.Split(',')[1], out result2);
            int.TryParse(str2.Split(',')[0], out result3);
            int.TryParse(str2.Split(',')[1], out result4);
            string wallPosition = ":w=" + result1.ToString() + "," + result2.ToString() + " l=" + result3.ToString() + "," + result4.ToString() + " " + data[2];
            position = PlaceObjectEvent.WallPositionCheck(wallPosition);
            return position != null;
        }

        private static bool TrySetWallItem2(
          Habbo Habbo,
          Item item,
          string[] data,
          out string position)
        {
            string str1 = data[0].Substring(3, data[0].Length - 3);
            string str2 = data[1].Substring(2, data[1].Length - 2);
            if (!str1.Contains(",") || !str2.Contains(","))
            {
                position = (string)null;
                return false;
            }
            int result1 = 0;
            int result2 = 0;
            int result3 = 0;
            int result4 = 0;
            int.TryParse(str1.Split(',')[0], out result1);
            int.TryParse(str1.Split(',')[1], out result2);
            int.TryParse(str2.Split(',')[0], out result3);
            int.TryParse(str2.Split(',')[1], out result4);
            string wallPosition = ":w=" + result1.ToString() + "," + result2.ToString() + " l=" + result3.ToString() + "," + result4.ToString() + " " + data[2];
            position = PlaceObjectEvent.WallPositionCheck(wallPosition);
            return position != null;
        }

        public static string WallPositionCheck(string wallPosition)
        {
            try
            {
                if (wallPosition.Contains<char>(Convert.ToChar(13)) || wallPosition.Contains<char>(Convert.ToChar(9)))
                    return (string)null;
                string[] strArray1 = wallPosition.Split(' ');
                if (strArray1[2] != "l" && strArray1[2] != "r")
                    return (string)null;
                string[] strArray2 = strArray1[0].Substring(3).Split(',');
                int num1 = int.Parse(strArray2[0]);
                int num2 = int.Parse(strArray2[1]);
                if (num1 < -1000 || num2 < -1 || num1 > 700 || num2 > 700)
                    return (string)null;
                string[] strArray3 = strArray1[1].Substring(2).Split(',');
                int num3 = int.Parse(strArray3[0]);
                int num4 = int.Parse(strArray3[1]);
                if (num3 < -1 || num4 < -1000 || num3 > 700 || num4 > 700)
                    return (string)null;
                return ":w=" + num1.ToString() + "," + num2.ToString() + " l=" + num3.ToString() + "," + num4.ToString() + " " + strArray1[2];
            }
            catch
            {
                return (string)null;
            }
        }
    }
}
