using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class roomalert : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Room == null)
                return;

            string s = CommandManager.MergeParams(Params, 1);
            if (Session.Antipub(s, "<CMD>", Room.Id))
                return;
            ServerPacket message = new ServerPacket(ServerPacketHeader.BroadcastMessageAlertMessageComposer);
            message.WriteString(s);
            Room.SendPacket(message);

        }
    }
}
