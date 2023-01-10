using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class PremioUser : IChatCommand
    {
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, RoomUser UserRoom, string[] Params)
        {
            string name_monedaoficial = (AkiledEnvironment.GetConfig().data["name_monedaoficial"]);
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, introduzca el nombre.");
                return;
            }

            GameClient Target = AkiledEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (Target == null)
            {
                Session.SendWhisper("Oops, No se ha conseguido este usuario!");
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


            if (Session.GetHabbo().CurrentRoom == TargetClient.GetHabbo().CurrentRoom)
            {

                int Amount;

                if (int.TryParse(Params[2], out Amount))
                {

                    if (Amount < 0 || Amount > 4)
                    {
                        Session.SendWhisper("Por favor introduce un numero. (1-4)");
                        return;
                    }

                    Target.GetHabbo().Duckets += Amount;
                    Target.SendPacket(new HabboActivityPointNotificationComposer(Target.GetHabbo().Duckets, Amount));



                    if (Target.GetHabbo().Id != Session.GetHabbo().Id)
                        Target.SendNotification(Session.GetHabbo().Username + " te ha enviado " + Amount.ToString() + " Diamantes!");
                    Session.SendWhisper("Le enviaste " + Amount + " Esmeraldas a " + Target.GetHabbo().Username + "!");
                    AkiledEnvironment.GetGame().GetClientManager().SendMessage(RoomNotificationComposer.SendBubble("ganador", "" + Target.GetHabbo().Username + " acaba de ganar el evento. felicitaciones :)", ""));

                }
                else
                {
                    Session.SendWhisper("Oops, las cantidades solo en numeros..!");

                }


            }

        }
    }
}
