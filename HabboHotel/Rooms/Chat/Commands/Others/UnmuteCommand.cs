using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Users;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
	class UnmuteCommand : IChatCommand
	{
		public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
		{
			if (Params.Length == 1)
			{
				Session.SendWhisper("Escribe el nombre de usuari@!", 1);
				return;
			}
			GameClient TargetClient = AkiledEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
			if (TargetClient == null || TargetClient.GetHabbo() == null)
			{
				Session.SendWhisper("Der Spieler konnte nicht gefunden werden.", 1);
				return;
			}
			using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
			{
				dbClient.RunQuery("UPDATE `users` SET `time_muted` = '0' WHERE `id` = '" + TargetClient.GetHabbo().Id + "' LIMIT 1");
			}
			TargetClient.GetHabbo().TimeMuted = 0.0;
			TargetClient.SendWhisper("Usted ha sido desmutead@ por " + Session.GetHabbo().Username + " agradécele!", 1);
			Session.SendWhisper("Has desmuteado al usuario " + TargetClient.GetHabbo().Username + "!", 1);
		}
	}
}
