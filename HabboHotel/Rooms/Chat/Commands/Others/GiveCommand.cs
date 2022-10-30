using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class GiveCommand : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            string name_monedaoficial = (AkiledEnvironment.GetConfig().data["name_monedaoficial"]);
            string name_monedaoficial2 = (AkiledEnvironment.GetConfig().data["name_monedaoficial2"]);
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor introduce ! (creditos, Esmeraldas, Planetas)");
                return;
            }

            GameClient Target = AkiledEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (Target == null)
            {
                Session.SendWhisper("Oops, No se ha conseguido este usuario!");
                return;
            }

            string UpdateVal = Params[2];
            switch (UpdateVal.ToLower())
            {
                case "creditos":
                case "credito":
                    {
                        if (!Session.GetHabbo().HasFuse("give_command"))
                        {
                            Session.SendWhisper("Oops, usted no tiene los permisos necesarios para usar este comando!");
                            break;
                        }
                        else
                        {
                            int Amount;
                            if (int.TryParse(Params[3], out Amount))
                            {
                                Target.GetHabbo().Credits = Target.GetHabbo().Credits += Amount;
                                Target.SendPacket(new CreditBalanceComposer(Target.GetHabbo().Credits));

                                if (Target.GetHabbo().Id != Session.GetHabbo().Id)
                                    Target.SendNotification(Session.GetHabbo().Username + " te ha enviado " + Amount.ToString() + " Credito(s)!");
                                Session.SendWhisper("Le enviaste " + Amount + " Credito(s) a " + Target.GetHabbo().Username + "!");
                                break;
                            }
                            else
                            {
                                Session.SendWhisper("Oops, las cantidades solo en numeros..");
                                break;
                            }
                        }
                    }

                case "esmeralda":
                case "esmeraldas":
                case "esme":

                    {
                        if (!Session.GetHabbo().HasFuse("give_command"))
                        {
                            Session.SendWhisper("Oops, usted no tiene los permisos necesarios para enviar Esmeraldas!");
                            break;
                        }
                        else
                        {
                            int Amount;
                            if (int.TryParse(Params[3], out Amount))
                            {
                                Target.GetHabbo().Duckets += Amount;
                                Target.SendPacket(new HabboActivityPointNotificationComposer(Target.GetHabbo().Duckets, Amount));

                                if (Target.GetHabbo().Id != Session.GetHabbo().Id)
                                    Target.SendNotification(Session.GetHabbo().Username + " te ha enviado " + Amount.ToString() + " " + name_monedaoficial2 + "!");
                                Session.SendWhisper("Le enviaste " + Amount + " " + name_monedaoficial2 + " a " + Target.GetHabbo().Username + "!");
                                break;
                            }
                            else
                            {
                                Session.SendWhisper("Oops, las cantidades solo en numeros..");
                                break;
                            }
                        }
                    }

                case "planetas":
                case "Planeta":

                    {
                        if (!Session.GetHabbo().HasFuse("give_command"))
                        {
                            Session.SendWhisper("Oops, No tiene los permisos necesarios para usar este comando!");
                            break;
                        }
                        else
                        {
                            int Amount;
                            if (int.TryParse(Params[3], out Amount))
                            {

                                Target.GetHabbo().AkiledPoints += Amount;
                                Target.SendPacket(new HabboActivityPointNotificationComposer(Target.GetHabbo().AkiledPoints, Amount, 105));

                                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                    queryreactor.RunQuery("UPDATE users SET vip_points = vip_points + " + Amount + " WHERE id = " + Target.GetHabbo().Id + " LIMIT 1");

                                if (Target.GetHabbo().Id != Session.GetHabbo().Id)
                                    Target.SendNotification(Session.GetHabbo().Username + " te ha enviado " + Amount.ToString() + " " + name_monedaoficial + "!");
                                Session.SendWhisper("Le enviaste " + Amount + " " + name_monedaoficial + " a " + Target.GetHabbo().Username + "!");
                                break;
                            }
                            else
                            {
                                Session.SendWhisper("Oops, las cantidades solo en numeros..!");
                                break;
                            }
                        }
                    }

                default:
                    Session.SendWhisper("'" + UpdateVal + "' no es una moneda válida.");
                    break;
            }
        }
    }
}
