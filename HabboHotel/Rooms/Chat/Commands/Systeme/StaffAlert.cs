using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.GameClients;
using System.Collections.Generic;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class StaffAlert : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 2) return;

            string MessageTxt = CommandManager.MergeParams(Params, 1);

            if (string.IsNullOrEmpty(MessageTxt)) return;

            List<GameClient> Staffs = AkiledEnvironment.GetGame().GetClientManager().GetStaffUsers();

            if (Staffs == null) return;

            foreach (GameClient Staff in Staffs)
            {
                if (Staff == null) continue;
                if (Staff.GetHabbo() == null) continue;
                if (Staff.GetHabbo().CurrentRoom == null) continue;

                RoomUser User = Staff.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabboId(Staff.GetHabbo().Id);

                ServerPacket Message = new ServerPacket(ServerPacketHeader.WhisperMessageComposer);
                Message.WriteInteger(0);
                Message.WriteString("[STAFF ALERT] " + MessageTxt + " - " + UserRoom.GetUsername());
                Message.WriteInteger(0);
                Message.WriteInteger(23);
                Message.WriteInteger(0);
                Message.WriteInteger(-1);
                User.GetClient().SendPacket(Message);
            }
        }
    }
}
