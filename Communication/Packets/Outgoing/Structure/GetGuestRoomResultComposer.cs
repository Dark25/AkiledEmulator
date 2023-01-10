using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Navigators;
using Akiled.HabboHotel.Rooms;
using System;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class GetGuestRoomResultComposer : ServerPacket
    {
        public GetGuestRoomResultComposer(GameClient Session, RoomData Data, Boolean isLoading, Boolean checkEntry)
            : base(ServerPacketHeader.GetGuestRoomResultMessageComposer)
        {
            WriteBoolean(isLoading);
            WriteInteger(Data.Id);
            WriteString(Data.Name);
            WriteInteger(Data.OwnerId);
            WriteString(Data.OwnerName);
            WriteInteger((Session.GetHabbo().IsTeleporting) ? 0 : Data.State);
            WriteInteger(Data.UsersNow);
            WriteInteger(Data.UsersMax);
            WriteString(Data.Description);
            WriteInteger(Data.TrocStatus);
            WriteInteger(Data.Score);
            WriteInteger(0);//Top rated room rank.
            WriteInteger(Data.Category);

            WriteInteger(Data.Tags.Count);
            foreach (string Tag in Data.Tags)
            {
                WriteString(Tag);
            }


            if (Data.Group != null)
            {
                WriteInteger(58);//What?
                WriteInteger(Data.Group == null ? 0 : Data.Group.Id);
                WriteString(Data.Group == null ? "" : Data.Group.Name);
                WriteString(Data.Group == null ? "" : Data.Group.Badge);
            }
            else
            {
                WriteInteger(56);//What?
            }


            this.WriteBoolean(checkEntry);
            StaffPick room = (StaffPick)null;
            if (!AkiledEnvironment.GetGame().GetNavigator().TryGetStaffPickedRoom(Data.Id, out room))
                this.WriteBoolean(false);
            else
                this.WriteBoolean(true);
            WriteBoolean(false);
            WriteBoolean(false);

            WriteInteger(Data.MuteFuse); // who can mute
            WriteInteger(Data.WhoCanKick); // who can kick
            WriteInteger(Data.BanFuse); // who can ban

            WriteBoolean((Session != null) ? Data.OwnerName.ToLower() != Session.GetHabbo().Username.ToLower() : false);
            WriteInteger(Data.ChatType);  //ChatMode, ChatSize, ChatSpeed, HearingDistance, ExtraFlood is the order.
            WriteInteger(Data.ChatBalloon);
            WriteInteger(Data.ChatSpeed);
            WriteInteger(Data.ChatMaxDistance);
            WriteInteger(Data.ChatFloodProtection);
        }
    }
}
