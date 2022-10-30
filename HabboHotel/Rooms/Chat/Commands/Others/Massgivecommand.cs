using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using System.Linq;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class Massgivecommand : IChatCommand
    {

        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            string name_monedaoficial = (AkiledEnvironment.GetConfig().data["name_monedaoficial"]);
            if (Params.Length == 1)
            {
                Session.SendWhisper("Debes introducir el tipo de moneda: creditos, diamantes, " + name_monedaoficial + ".", 34);
                return;
            }

            string UpdateVal = Params[1];
            switch (UpdateVal.ToLower())
            {
                case "credits":
                    {
                        if (!Session.GetHabbo().HasFuse("give_command"))
                        {
                            Session.SendWhisper("Oops, usted no tiene los permisos necesarios para enviar monedas!");
                            break;
                        }
                        else
                        {
                            int Amount;
                            if (int.TryParse(Params[2], out Amount))
                            {
                                foreach (GameClient Target in AkiledEnvironment.GetGame().GetClientManager().GetClients.ToList())
                                {
                                    if (Target == null || Target.GetHabbo() == null || Target.GetHabbo().Username == Session.GetHabbo().Username)
                                        continue;

                                    Target.GetHabbo().Credits = Target.GetHabbo().Credits += Amount;
                                    Target.SendMessage(new CreditBalanceComposer(Target.GetHabbo().Credits));
                                    Target.SendMessage(RoomNotificationComposer.SendBubble("cred", "" + Session.GetHabbo().Username + " te acaba de enviar " + Amount + " créditos.", ""));

                                }

                                break;
                            }
                            else
                            {
                                Session.SendWhisper("Sólo puedes introducir cantidades numerales.", 34);
                                break;
                            }
                        }
                    }

                case "duckets":
                case "diamantes":
                case "Diamantes":
                case "dm":
                case "diamonds":
                    {
                        if (!Session.GetHabbo().HasFuse("give_command"))
                        {
                            Session.SendWhisper("Oops, usted no tiene los permisos necesarios para enviar monedas!");
                            break;
                        }
                        else
                        {
                            int Amount;
                            if (int.TryParse(Params[2], out Amount))
                            {
                                foreach (GameClient Target in AkiledEnvironment.GetGame().GetClientManager().GetClients.ToList())
                                {
                                    if (Target == null || Target.GetHabbo() == null || Target.GetHabbo().Username == Session.GetHabbo().Username)
                                        continue;

                                    Target.GetHabbo().Duckets += Amount;
                                    Target.SendMessage(new HabboActivityPointNotificationComposer(Target.GetHabbo().Duckets, Amount));
                                    Target.SendMessage(RoomNotificationComposer.SendBubble("senddiamonds", "" + Session.GetHabbo().Username + " te acaba de enviar " + Amount + " diamantes.", ""));
                                }

                                break;
                            }
                            else
                            {
                                Session.SendWhisper("Sólo puedes introducir cantidades numerales.", 34);
                                break;
                            }
                        }
                    }

                case "planetas":
                case "planeta":
                case "Pl":
                case "planets":
                    {
                        if (!Session.GetHabbo().HasFuse("give_command"))
                        {
                            Session.SendWhisper("Oops, usted no tiene los permisos necesarios para enviar monedas!");
                            break;
                        }
                        else
                        {
                            int Amount;
                            if (int.TryParse(Params[2], out Amount))
                            {

                                foreach (GameClient client in AkiledEnvironment.GetGame().GetClientManager().GetClients.ToList())
                                {
                                    client.GetHabbo().AkiledPoints += Amount;
                                    client.SendPacket(new HabboActivityPointNotificationComposer(client.GetHabbo().AkiledPoints, Amount, 105));

                                    using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                        queryreactor.RunQuery("UPDATE users SET vip_points = vip_points + " + Amount + " WHERE id = " + client.GetHabbo().Id + " LIMIT 1");

                                    if (client.GetHabbo().Id != Session.GetHabbo().Id)
                                        client.SendNotification(Session.GetHabbo().Username + " te ha enviado " + Amount.ToString() + " " + name_monedaoficial + "!");
                                }
                                Session.SendWhisper("Le enviaste " + Amount + " " + name_monedaoficial + " a todo el hotel online!");
                                break;
                            }
                            else
                            {
                                Session.SendWhisper("Sólo puedes introducir cantidades numerales.", 34);
                                break;
                            }
                        }
                    }

                default:
                    Session.SendWhisper("¡'" + UpdateVal + "' no es una moneda válida!", 34);
                    break;
            }
        }
    }
}
