using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using System;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class paycommand : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Oops, debes ingresar un nombre de usuario!");
                return;
            }

            if ((double)Session.GetHabbo().last_pay > AkiledEnvironment.GetUnixTimestamp() - 30.0 && !Session.GetHabbo().HasFuse("override_limit_command"))
            {
                Session.SendWhisper("Debes esperar 30 segundos, para volver a usar el comando", 1);
                return;
            }

            string name_monedaoficial = (AkiledEnvironment.GetConfig().data["name_monedaoficial"]);
            string icon_monedaoficial = (AkiledEnvironment.GetConfig().data["icon_monedaoficial"]);
            if (Params.Length == 2)
            {
                Session.SendWhisper("Oops, ingresa el tipo de moneda que deseas transferir. (creditos/diamantes/" + name_monedaoficial + ")");
                return;
            }

            if (Params.Length == 3)
            {
                Session.SendWhisper("Oops, introduce la cantidad que deseas enviar.");
                return;
            }
            else if (Params[3].Contains("-"))
            {
                Session.SendWhisper("Oops, Vuelve a usarlo y te quedaras sin cuenta! (E:3)");
                return;
            }

            GameClient TargetClient = null;
            TargetClient = AkiledEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);

            if (TargetClient == null)
            {
                Session.SendWhisper("Oops, el usuario no se encuentra conectado!");
                return;
            }

            if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("Oops, no puedes usar este comando con tu misma cuenta!");
                return;
            }

            int Amount = Convert.ToInt32(Params[3]);

            var Options = Params[2];
            switch (Options.ToLower())
            {
                case "credits":
                case "creditos":
                case "c":
                    {
                        if (Session.GetHabbo().Credits < Amount)
                        {
                            Session.SendWhisper("Oops, no tienes suficientes créditos! (E:1)");
                            return;
                        }

                        if (Amount > Session.GetHabbo().Credits)
                        {
                            Session.SendWhisper("Oops, no tienes suficientes créditos! (E:2)");
                            return;
                        }

                        Session.GetHabbo().Credits -= Amount;
                        Session.SendPacket(new CreditBalanceComposer(Session.GetHabbo().Credits));

                        TargetClient.GetHabbo().Credits += Amount;
                        TargetClient.SendPacket(new CreditBalanceComposer(TargetClient.GetHabbo().Credits));

                        Session.SendWhisper("Enviaste con éxito " + Amount + " créditos de su cuenta a la cuenta de " + TargetClient.GetHabbo().Username + " !");
                        TargetClient.SendWhisper("Has recibido del usuario " + Session.GetHabbo().Username + " " + Amount + " créditos!");
                    
                        Session.GetHabbo().last_pay = AkiledEnvironment.GetIUnixTimestamp();
                        break;
                    }
                case "diamantes":
                case "Diamantes":
                case "dm":
                case "diamonds":
                    {
                        if (Session.GetHabbo().Duckets < Amount)
                        {
                            Session.SendWhisper("Oops, no tienes suficientes diamantes! (E:1)", 34);
                            return;
                        }

                        if (Amount > Session.GetHabbo().Duckets)
                        {
                            Session.SendWhisper("Oops, no tienes suficientes diamantes! (E:2)", 34);
                            return;
                        }

                        Session.GetHabbo().Duckets -= Amount;
                        Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Amount));

                        TargetClient.GetHabbo().Duckets += Amount;
                        TargetClient.SendPacket(new HabboActivityPointNotificationComposer(TargetClient.GetHabbo().Duckets, Amount));

                        Session.SendWhisper("Enviaste con éxito " + Amount + " diamantes de su cuenta a la cuenta de " + TargetClient.GetHabbo().Username + " !", 34);
                        TargetClient.SendWhisper("Has recibido del usuario " + Session.GetHabbo().Username + " " + Amount + " diamantes!", 34);
                        AkiledEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("senddiamonds", "El usuario: " + Session.GetHabbo().Username + " acaba de enviar la cantidad de : " + Amount + " Diamantes al usuario: " + TargetClient.GetHabbo().Username + ", hay que estar atentos a los timos o estafas."));

                        foreach (GameClient Client in AkiledEnvironment.GetGame().GetClientManager().GetStaffUsers())
                        {
                            if (Client == null || Client.GetHabbo() == null)
                                continue;

                            string type = "<PAY>";
                            Client.GetHabbo().SendWebPacket(new AddChatlogsComposer(Session.GetHabbo().Id, Session.GetHabbo().Username, type + " acaba de enviar la cantidad de : " + Amount + " Diamantes al usuario: " + TargetClient.GetHabbo().Username + ", hay que estar atentos a los timos o estafas."));
                        }

                        Session.GetHabbo().last_pay = AkiledEnvironment.GetIUnixTimestamp();
                        break;
                    }
                case "planetas":
                case "planeta":
                case "Pl":
                case "planets":

                    {
                        if (Session.GetHabbo().AkiledPoints < Amount)
                        {
                            Session.SendWhisper("Oops, ¡No tienes suficientes " + name_monedaoficial + "! (E:1)");
                            return;
                        }

                        if (Amount > Session.GetHabbo().AkiledPoints)
                        {
                            Session.SendWhisper("Oops, ¡No tienes suficientes " + name_monedaoficial + "! (E:2)");
                            return;
                        }

                        Session.GetHabbo().AkiledPoints -= Amount;
                        Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().AkiledPoints, 0, 105));

                        using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                            queryreactor.RunQuery("UPDATE users SET vip_points = vip_points - " + Amount + " WHERE id = " + Session.GetHabbo().Id + " LIMIT 1");

                        TargetClient.GetHabbo().AkiledPoints += Amount;
                        TargetClient.SendPacket(new HabboActivityPointNotificationComposer(TargetClient.GetHabbo().AkiledPoints, Amount, 105));

                        using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                            queryreactor.RunQuery("UPDATE users SET vip_points = vip_points + " + Amount + " WHERE id = " + TargetClient.GetHabbo().Id + " LIMIT 1");

                        Session.SendWhisper("Enviaste con éxito " + Amount + " " + name_monedaoficial + " de su cuenta a la cuenta de " + TargetClient.GetHabbo().Username + " !");
                        TargetClient.SendWhisper("Has recibido del usuario " + Session.GetHabbo().Username + " " + Amount + " " + name_monedaoficial + "!");
                        AkiledEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble(icon_monedaoficial, "El usuario: " + Session.GetHabbo().Username + " acaba de enviar la cantidad de : " + Amount + " " + name_monedaoficial + " al usuario: " + TargetClient.GetHabbo().Username + ", hay que estar atentos a los timos o estafas."));

                        foreach (GameClient Client in AkiledEnvironment.GetGame().GetClientManager().GetStaffUsers())
                        {
                            if (Client == null || Client.GetHabbo() == null)
                                continue;

                            string type = "<PAY>";
                            Client.GetHabbo().SendWebPacket(new AddChatlogsComposer(Session.GetHabbo().Id, Session.GetHabbo().Username, type + " acaba de enviar la cantidad de : " + Amount + " " + name_monedaoficial + " al usuario: " + TargetClient.GetHabbo().Username + ", hay que estar atentos a los timos o estafas."));
                        }

                        Session.GetHabbo().last_pay = AkiledEnvironment.GetIUnixTimestamp();
                        break;

                    }
                default:
                    {
                        Session.SendWhisper("Oops, ¡Esta opción no existe! Solo puedes enviar: (creditos/diamantes/" + name_monedaoficial + ")");
                        break;
                    }
            }
        }
    }
}