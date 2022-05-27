using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms.Games;
using System;
using System.Data;


namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class RewardCommand : IChatCommand
    {
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, RoomUser UserRoom, string[] Params)
        {
            Room TargetRoom;
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, introduzca el nombre.");
                return;
            }

            GameClient TargetClient = AkiledEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);

            if (TargetClient == null)
            {
                Session.SendWhisper("Este usuario no se encuentra en la sala o está desconectad@.");
                return;
            }

            if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("Lo sentimos, no puede darse premios, " + Session.GetHabbo().Username + ".");
                return;
            }

            if (TargetClient.GetHabbo().CurrentRoom == Session.GetHabbo().CurrentRoom)
            {
             int RewardDiamonds = 0;
            int Rewardcredits = 0;
            int RewardDuckets = 0;
            using (IQueryAdapter dbQuery = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbQuery.SetQuery("SELECT * FROM `game_rewardscash` LIMIT 1");
                DataTable gUsersTable = dbQuery.GetTable();

                foreach (DataRow Row in gUsersTable.Rows)
                {
                    Rewardcredits = Convert.ToInt32(Row["credits"]);
                    RewardDiamonds = Convert.ToInt32(Row["Akiledcoins"]);
                    RewardDuckets = Convert.ToInt32(Row["diamantes"]);
                }
            }
            TargetClient.GetHabbo().Credits += Rewardcredits;
            TargetClient.SendPacket(new CreditBalanceComposer(TargetClient.GetHabbo().Credits));
            TargetClient.GetHabbo().Duckets += RewardDuckets;
            TargetClient.SendPacket(new HabboActivityPointNotificationComposer(TargetClient.GetHabbo().Duckets, RewardDuckets));
            TargetClient.GetHabbo().AkiledPoints += RewardDiamonds;
            TargetClient.SendPacket(new HabboActivityPointNotificationComposer(TargetClient.GetHabbo().AkiledPoints, RewardDiamonds, 105));
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                queryreactor.RunQuery("UPDATE users SET vip_points = vip_points + " + RewardDiamonds + " WHERE id = " + TargetClient.GetHabbo().Id + " LIMIT 1");
            if (!AkiledEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out TargetRoom))
                return;
            //Session.SendPacket(new RoomForwardComposer(Session.GetHabbo().CurrentRoomId));
            TargetClient.SendPacket(RoomNotificationComposer.SendBubble("ganador", "Has recibido " + Rewardcredits + " Créditos, " + RewardDuckets + " Esmeraldas, " + RewardDiamonds + " Planeta's por haber ganado el juego o evento.", ""));
            AkiledEnvironment.GetGame().GetClientManager().SendMessage(RoomNotificationComposer.SendBubble("ganador", "" + TargetClient.GetHabbo().Username + " acaba de ganar el evento. felicitaciones :)", ""));
        }
        }
    }
}