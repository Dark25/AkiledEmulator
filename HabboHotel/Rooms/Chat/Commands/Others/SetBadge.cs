using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class SetBadge : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 3)
            {
                Session.SendWhisper("Introduce el nombre del usuario a quien deseas enviar una placa!");
                return;
            }

            GameClient TargetClient = AkiledEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient != null)
            {
                if (!TargetClient.GetHabbo().GetBadgeComponent().HasBadge(Params[2]))
                {
                    TargetClient.GetHabbo().GetBadgeComponent().GiveBadge(Params[2], 0, true);
                    if (TargetClient.GetHabbo().Id != Session.GetHabbo().Id)
                    {
                        TargetClient.SendMessage(new NewYearComposer(Params[2]));

                        Session.SendWhisper("Le diste la placa (" + Params[2] + ") exitosamente a " + Params[1] + ".");
                    }
                    else
                    {
                        Session.SendMessage(new NewYearComposer(Params[2]));
                        Session.SendWhisper("Le diste la placa (" + Params[2] + ") exitosamente a " + Params[1] + ".");
                    }
                }
                else
                    Session.SendWhisper("¡Huy!Este usuario ya tiene la placa.(" + Params[2] + ")!");
                return;
            }
            else
            {
                Session.SendWhisper("Oops! el usuario " + Params[1] + " no existe!");
                return;

            }
        }
    }
}