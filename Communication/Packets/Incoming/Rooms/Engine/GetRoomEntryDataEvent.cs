using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using System;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class GetRoomEntryDataEvent : IPacketEvent
    {


        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().LoadingRoomId == 0)
                return;


            Room Room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().LoadingRoomId);
            if (Room == null)
                return;

            if (Room.RoomData.State == 1)
            {
                if (!Session.GetHabbo().AllowDoorBell)
                    return;
                Session.GetHabbo().AllowDoorBell = false;
            }

            if (Session.GetHabbo().InRoom)
            {
                if (!AkiledEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room oldRoom))
                    return;

                if (oldRoom.GetRoomUserManager() != null)
                    oldRoom.GetRoomUserManager().RemoveUserFromRoom(Session, false, false);
            }

            if (!Room.GetRoomUserManager().AddAvatarToRoom(Session))
            {
                Room.GetRoomUserManager().RemoveUserFromRoom(Session, false, false);
                return;
            }

            Room.SendObjects(Session);

            if (Room.HideWired && Room.CheckRights(Session, true))
                Session.SendMessage(new RoomNotificationComposer("furni_placement_error", "message", "Wireds ocultos en la sala"));

            Session.SendPacket(new RoomEntryInfoComposer(Room.Id, Room.CheckRights(Session, true)));
            Session.SendPacket(new RoomVisualizationSettingsComposer(Room.RoomData.WallThickness, Room.RoomData.FloorThickness, Room.RoomData.Hidewall));

            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);

            if (ThisUser != null)
                Room.SendPacket(new UserChangeComposer(ThisUser, false));

            if (!ThisUser.IsSpectator)
                Room.GetRoomUserManager().UserEnter(ThisUser);

            if (Room.RoomData.HideWired)
                Room.SendMessage(Room.HideWiredMessages(Room.RoomData.HideWired));

            if (Session.GetHabbo().Nuxenable)
            {
                ServerPacket nuxStatus = new ServerPacket(ServerPacketHeader.NuxAlertComposer);
                nuxStatus.WriteInteger(2);
                Session.SendPacket(nuxStatus);

                Session.GetHabbo().PassedNuxCount++;
                Session.SendPacket(new NuxAlertComposer("nux/lobbyoffer/hide"));
                Session.SendPacket(new NuxAlertComposer("helpBubble/add/BOTTOM_BAR_NAVIGATOR/nux.bot.info.navigator.1"));
            }

            if (Session.GetHabbo().spamEnable)
            {
                TimeSpan timeSpan = DateTime.Now - Session.GetHabbo().spamFloodTime;
                if (timeSpan.TotalSeconds < (double)Session.GetHabbo().spamProtectionTime)
                {
                    Session.SendPacket(new FloodControlComposer(Session.GetHabbo().spamProtectionTime - timeSpan.Seconds));
                }
            }

            if (Room.RoomData.OwnerId != Session.GetHabbo().Id)
                AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_RoomEntry", 1);

            if (!Session.GetHabbo().Username.Contains(">") && !Session.GetHabbo().Username.Contains("<") && !Session.GetHabbo().Username.Contains("="))
                return;
        }
    }
}

