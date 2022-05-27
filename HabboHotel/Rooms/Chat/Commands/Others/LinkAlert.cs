using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class LinkAlert : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {

            string Link = Params[1];
            string Message = CommandManager.MergeParams(Params, 2);
            var ThisUser = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (ThisUser == null)
                return;
            Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "<font color=\"#2E9AFE\"><a href='" + Link + "' target='_blank'><b>" + Message + "</b></a></font>", 0, ThisUser.LastBubble));
            Session.SendWhisper("Mensaje enviado correctamente en la sala.");
        }
    }
}
