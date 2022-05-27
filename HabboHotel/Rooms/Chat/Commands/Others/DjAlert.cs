using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using System.Text;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class DjAlert : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {

            if ((double)Session.GetHabbo().last_dj > AkiledEnvironment.GetUnixTimestamp() - 60.0 && !Session.GetHabbo().HasFuse("override_limit_command"))
            {
                Session.SendWhisper("Debes esperar 60 segundos, para volver a usar el comando", 1);
                return;
            }

            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor escribe el nombre del dj para enviar la alerta");
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);
            AkiledEnvironment.GetGame().GetClientManager().SendMessage(RoomNotificationComposer.SendBubble("LOCOSON", Encoding.GetEncoding("Windows-1252").GetString(Encoding.GetEncoding("UTF-8").GetBytes("¡DJ " + Message + " está emitiendo en vivo! Sintoniza RadioFM ahora mismo y disfruta al máximo.")) , "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));
            Session.GetHabbo().last_dj = AkiledEnvironment.GetIUnixTimestamp();
            return;
        }
    }
}
