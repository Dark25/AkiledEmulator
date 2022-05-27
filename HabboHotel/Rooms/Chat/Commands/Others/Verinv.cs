using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
	class Verinv : IChatCommand
	{
		public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
		{
			if (Room == null)
			{
				return;
			}
			if (Params.Length == 2)
			{
				string Username = Params[1];
				GameClient Client = AkiledEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
				if (Client != null)
				{
					Session.SendWhisper("Der User muss offline sein, um in sein Inventar schauen zu können!", 1);
					return;
				}
				int UserId = AkiledEnvironment.GetGame().GetClientManager().GetUserIdByUsername(Username);
				if (UserId == 0)
				{
					Session.SendWhisper("Der User existiert nicht!", 1);
					return;
				}
				Session.GetHabbo().GetInventoryComponent().LoadUserInventory(UserId);
				Session.SendWhisper("In deinem Inventar siehst du nun alle Möbel von '" + Username + "'", 1);
			}
			else
			{
				Session.GetHabbo().GetInventoryComponent().LoadUserInventory(0);
				Session.SendWhisper("Dein Inventar ist nun wieder normal.", 1);
			}
			Session.SendWhisper("Gebe ':inventar' ein, um dein Inventar wiederherzustellen.", 1);
		}
	}
}
