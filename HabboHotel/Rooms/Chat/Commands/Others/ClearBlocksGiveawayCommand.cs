using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class ClearBlocksGiveawayCommand : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("TRUNCATE `give_away_blocks`");
                AkiledEnvironment.GetGame().GetGiveAwayBlocks().Init(dbClient);
            }

            Session.SendWhisper("¡Bloques de sorteos vaciado!", 34);
        }
    }
}
