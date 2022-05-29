using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using System.Text;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class LTDmensual : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            string Message = CommandManager.MergeParams(Params, 1);
            string name_hotel = (AkiledEnvironment.GetConfig().data["namehotel_text"]);
            string ltdmensual_alert = (AkiledEnvironment.GetConfig().data["ltdmensual_alert"]);
            AkiledEnvironment.GetGame().GetClientManager().SendMessage(new RoomNotificationComposer("¡LTD MENSUAL DISPONIBLE!",
              "¡Estimados Usuairios De <font color=\"#d100fe\"><b>© " + name_hotel + "</b></font> Acaba De Ser Actualizado <b>El Catálogo!</b> Esta Vez Se Trata De Un Nuevo <font color=\"#fecb00\"><b>Rare(s) LTD MENSUAL </b></font> Sólo Debes Hacer Click En El Botón De Abajo Para Visualizarlo.<br>", ltdmensual_alert, Encoding.GetEncoding("Windows-1252").GetString(Encoding.GetEncoding("UTF-8").GetBytes("Ir a La Página LTD")), "event:catalog/open/ltdmensual" + Message));

            Session.SendWhisper("Alerta De Nuevo LTD Mensual Enviado Correctamente.", 34);
        }
    }
}
