using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class Commands : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {

            if (Session.GetHabbo().Rank <= 2)
            {
                ServerPacket notif = new ServerPacket(ServerPacketHeader.NuxAlertComposer);
                notif.WriteString("habbopages/commands.txt");
                Session.SendPacket(notif);
            }
            else if (Session.GetHabbo().Rank > 15)
            {
                ServerPacket notif = new ServerPacket(ServerPacketHeader.NuxAlertComposer);
                notif.WriteString("habbopages/commandsowner.txt");
                Session.SendPacket(notif);
            }
            else
            {
                Session.SendHugeNotif(AkiledEnvironment.GetGame().GetChatManager().GetCommands().GetCommandList(Session));
            }
        }
    }
}
