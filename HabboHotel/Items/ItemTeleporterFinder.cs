using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Rooms;
using System;
using System.Data;

namespace Akiled.HabboHotel.Items
{
    public static class ItemTeleporterFinder
    {
        public static int GetLinkedTele(int TeleId)
        {
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("SELECT tele_two_id FROM tele_links WHERE tele_one_id = " + TeleId);
                DataRow row = queryreactor.GetRow();
                if (row == null)
                    return 0;
                else
                    return Convert.ToInt32(row[0]);
            }
        }

        public static int GetTeleRoomId(int TeleId, Room pRoom)
        {
            if (pRoom == null)
                return 0;
            if (pRoom.GetRoomItemHandler() == null)
                return 0;
            if (pRoom.GetRoomItemHandler().GetItem(TeleId) != null)
                return pRoom.Id;

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("SELECT room_id FROM items WHERE id = " + TeleId + " LIMIT 1");
                DataRow row = queryreactor.GetRow();
                if (row == null)
                    return 0;
                else
                    return Convert.ToInt32(row[0]);
            }
        }

        public static bool IsTeleLinked(int TeleId, Room pRoom)
        {
            int linkedTele = GetLinkedTele(TeleId);
            if (linkedTele == 0)
                return false;
            Item roomItem = pRoom.GetRoomItemHandler().GetItem(linkedTele);
            return roomItem != null && (roomItem.GetBaseItem().InteractionType == InteractionType.TELEPORT || roomItem.GetBaseItem().InteractionType == InteractionType.ARROW) || GetTeleRoomId(linkedTele, pRoom) != 0;
        }
    }
}
