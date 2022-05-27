using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
	class HALCommand : IChatCommand
	{
		public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
		{

			if ((double)Session.GetHabbo().last_hal > AkiledEnvironment.GetUnixTimestamp() - 60.0 && !Session.GetHabbo().HasFuse("override_limit_command"))
			{
				Session.SendWhisper("Debes esperar 60 segundos, para volver a usar el comando", 1);
				return;
			}


			if (Params.Length == 1)

			{
				Session.SendWhisper("Por favor escribe el link del enlace y luego el mensaje.");
				return;
			}
			if (Params.Length == 2)

			{
				Session.SendWhisper("Por favor escribe el mensaje que vas enviar despues del link.");
				return;
			}
			string URL = Params[1];
			string Message = CommandManager.MergeParams(Params, 2);
			AkiledEnvironment.GetGame().GetClientManager().SendMessage(new RoomNotificationComposer("Mensaje del Equipo Administrativo", "<font color = '#000000'><font>" + Message + " </font></font>\r\n <font color = '#008000'><font><b>" + Session.GetHabbo().Username, "</b></font></font>", URL, URL));
			Session.GetHabbo().last_hal = AkiledEnvironment.GetIUnixTimestamp();
		}
	}
}
