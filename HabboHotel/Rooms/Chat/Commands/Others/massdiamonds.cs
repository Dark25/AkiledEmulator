using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using System.Linq;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class massdiamonds : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            string name_monedaoficial = (AkiledEnvironment.GetConfig().data["name_monedaoficial"]);
            int Amount;
            if (int.TryParse(Params[1], out Amount))
            {
                if (Amount < 0 || Amount > 1000)
                {
                    Session.SendWhisper("Por favor introduce un numero. (1-1000)");
                    return;
                }

                foreach (GameClient client in AkiledEnvironment.GetGame().GetClientManager().GetClients.ToList())
                {
                    if (client.GetHabbo() != null)
                    {
                        client.GetHabbo().AkiledPoints += Amount;
                        client.SendPacket(new HabboActivityPointNotificationComposer(client.GetHabbo().AkiledPoints, Amount, 105));

                        using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                            queryreactor.RunQuery("UPDATE users SET vip_points = vip_points + " + Amount + " WHERE id = " + client.GetHabbo().Id + " LIMIT 1");

                        if (client.GetHabbo().Id != Session.GetHabbo().Id)
                            client.SendNotification(Session.GetHabbo().Username + " te ha enviado " + Amount.ToString() + " " + name_monedaoficial + "!");
                    }
                }
                Session.SendWhisper("Le enviaste " + Amount + " " + name_monedaoficial + " a todo el hotel online!");
                return;
            }
            else
            {
                Session.SendWhisper("Sólo puedes introducir cantidades numerales.", 34);
                return;
            }
        }
    }
}