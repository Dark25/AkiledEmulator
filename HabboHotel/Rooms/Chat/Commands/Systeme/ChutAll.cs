using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.GameClients;
using System.Linq;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class ChutAll : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            string Message = CommandManager.MergeParams(Params, 1);
            foreach (GameClient client in AkiledEnvironment.GetGame().GetClientManager().GetClients.ToList())
            {
                client.SendWhisper(Message, 33);
            }
            Session.SendWhisper("Mensaje Enviado Con Éxito!", 33);
        }
    }
}