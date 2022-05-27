using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.HabboHotel.GameClients;
using System.Linq;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
	class RoomBadgeCommand : IChatCommand
	{
		public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
		{
			if (Params.Length == 1)
			{
				Session.SendWhisper("Oops, debes ingresar un código de placa!");
				return;
			}
			foreach (RoomUser User in Room.GetRoomUserManager().GetUserList().ToList())
			{
				if (User != null && User.GetClient() != null && User.GetClient().GetHabbo() != null)
				{
					if (!User.GetClient().GetHabbo().GetBadgeComponent().HasBadge(Params[1]))
					{
						User.GetClient().GetHabbo().GetBadgeComponent().GiveBadge(Params[1], true);
						User.GetClient().SendNotification("<font color = '#008000'><font size= '16'><b>Has Recibido una Nueva Placa!</b></font></font>\n\nAsí es, Los staff estan muy contento por contar con tu presencia, por lo tanto, te han obsequiado una nueva placa, revisa tu inventario y hazla lucir.");
					}
					else
					{
						User.GetClient().SendWhisper(Session.GetHabbo().Username + ", Ya posees esta placa en tu inventario, pero ánimos, ya vienen nuevas para que las difrutes.");
					}
				}
			}
			Session.SendWhisper("Usted ha enviado la placa " + Params[1] + " a toda la sala!");
		}
	}
}
