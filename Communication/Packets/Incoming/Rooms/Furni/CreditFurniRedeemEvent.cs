using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class CreditFurniRedeemEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Room = null;
            if (!AkiledEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            if (!Room.CheckRights(Session, true))
                return;            Item Exchange = Room.GetRoomItemHandler().GetItem(Packet.PopInt());
            if (Exchange == null)
                return;

            if (Exchange.Data.InteractionType != InteractionType.EXCHANGE)
                return;

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())                queryreactor.RunQuery("DELETE items, items_limited FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) WHERE items.id = " + Exchange.Id);
            Room.GetRoomItemHandler().RemoveFurniture(null, Exchange.Id);            int Value = int.Parse(Exchange.GetBaseItem().ItemName.Split(new char[1] { '_' })[1]);            if (Value > 0)            {                if (Exchange.GetBaseItem().ItemName.StartsWith("CF_") || Exchange.GetBaseItem().ItemName.StartsWith("CFC_"))                {                    Session.GetHabbo().Credits += Value;                    Session.GetHabbo().UpdateCreditsBalance();                }

                else if (Exchange.GetBaseItem().ItemName.StartsWith("kk_") || Exchange.GetBaseItem().ItemName.StartsWith("kakas_") || Exchange.GetBaseItem().ItemName.StartsWith("kakacoins_") || Exchange.GetBaseItem().ItemName.StartsWith("TEMP_"))
                {
                    Session.GetHabbo().AkiledPoints += Value;
                    Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().AkiledPoints, Value, 105));
                    Session.GetHabbo().UpdateDiamondsBalance();

                    using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                        queryreactor.RunQuery("UPDATE users SET vip_points = vip_points + " + Value + " WHERE id = " + Session.GetHabbo().Id);
                }
                else if (Exchange.GetBaseItem().ItemName.StartsWith("DIA_") || Exchange.GetBaseItem().ItemName.StartsWith("DF_") || Exchange.GetBaseItem().ItemName.StartsWith("DI_"))
                {
                    Session.GetHabbo().Duckets += Value;
                    Session.GetHabbo().UpdateActivityPointsBalance();
                }                else if (Exchange.GetBaseItem().ItemName.StartsWith("WwnEx_"))                {                    using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())                    {                        queryreactor.RunQuery("UPDATE user_stats SET AchievementScore = AchievementScore + '" + Value + "' WHERE id = '" + Session.GetHabbo().Id + "'");                    }                    Session.GetHabbo().AchievementPoints += Value;                    Session.SendPacket(new AchievementScoreComposer(Session.GetHabbo().AchievementPoints));                    if (Room != null)
                    {
                        RoomUser roomUserByHabbo = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
                        if (roomUserByHabbo != null)
                        {
                            Session.SendPacket(new UserChangeComposer(roomUserByHabbo, true));
                            Room.SendPacket(new UserChangeComposer(roomUserByHabbo, false));
                        }
                    }                }            }
        }
    }
}
