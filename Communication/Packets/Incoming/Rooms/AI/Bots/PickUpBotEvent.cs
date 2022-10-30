using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.Users.Inventory.Bots;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class PickUpBotEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            int BotId = Packet.PopInt();
            if (BotId <= 0)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null || !Room.CheckRights(Session, true))
                return;

            RoomUser BotUser = null;
            if (!Room.GetRoomUserManager().TryGetBot(BotId, out BotUser))
            {
                RoomUser TargetUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabboId(BotId);
                if (TargetUser == null)
                    return;

                //Check some values first, please!
                if (TargetUser.GetClient() == null || TargetUser.GetClient().GetHabbo() == null)
                    return;

                TargetUser.transfbot = false;

                //Quickly remove the old user instance.
                Room.SendPacket(new UserRemoveComposer(TargetUser.VirtualId));

                //Add the new one, they won't even notice a thing!!11 8-)
                Room.SendPacket(new UsersComposer(TargetUser));
                return;
            }

            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `bots` SET `room_id` = '0' WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("id", BotId);
                dbClient.RunQuery();
            }

            Session.GetHabbo().GetInventoryComponent().TryAddBot(new Bot(BotUser.BotData.Id, BotUser.BotData.OwnerId, BotUser.BotData.Name, BotUser.BotData.Motto, BotUser.BotData.Look, BotUser.BotData.Gender, BotUser.BotData.WalkingEnabled, BotUser.BotData.AutomaticChat, BotUser.BotData.ChatText, BotUser.BotData.SpeakingInterval, BotUser.BotData.IsDancing, BotUser.BotData.Enable, BotUser.BotData.Handitem, BotUser.BotData.Status));
            Session.SendPacket(new BotInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetBots()));
            Room.GetRoomUserManager().RemoveBot(BotUser.VirtualId, false);

        }
    }
}
