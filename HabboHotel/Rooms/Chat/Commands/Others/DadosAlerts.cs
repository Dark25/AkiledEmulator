using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using System.Text;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class DadosAlerts : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if ((double)Session.GetHabbo().last_dadosalert > AkiledEnvironment.GetUnixTimestamp() - 60.0 && !Session.GetHabbo().HasFuse("override_limit_command"))
            {
                Session.SendWhisper("Debes esperar 1 minuto, para volver a usar el comando", 1);
                return;
            }
            AkiledEnvironment.GetGame().GetClientManager().SendMessage(RoomNotificationComposer.SendBubble("DADOON", Encoding.GetEncoding("Windows-1252").GetString(Encoding.GetEncoding("UTF-8").GetBytes("¡Los Dados Han Sido Abiertos Por " + Session.GetHabbo().Username + " Ven a Probar Tu Suerte Apostando Tus Rares o Lingotes.")), "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));
            Session.GetHabbo().last_dadosalert = AkiledEnvironment.GetIUnixTimestamp();
            return;

        }
    }
}
