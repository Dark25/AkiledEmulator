using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using System.Text;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class RoomCommand : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room currentRoom = Session.GetHabbo().CurrentRoom;
            if (currentRoom == null)
                return;

            if (Params.Length == 1)
            {
                Session.SendWhisper("Oops, debes elegir una opcion para desactivar. ejecuta el comando ':room list'");
                return;
            }

            if (!Room.CheckRights(Session, true))
            {
                Session.SendWhisper("Oops, solo el dueño de la sala puede ejecutar este comando");
                return;
            }

            string Option = Params[1];
            switch (Option)
            {
                case "list":
                    {
                        StringBuilder List = new StringBuilder("");
                        List.AppendLine("Lista de comando en salas");
                        List.AppendLine("-------------------------");
                        List.AppendLine(":Room pets      : " + (currentRoom.RoomData.PetMorphsAllowed == true ? "Habilitado" : "Deshabilitado"));
                        List.AppendLine(":Room mascota   : " + (currentRoom.RoomData.PetMorphsAllowed == true ? "Habilitado" : "Deshabilitado"));
                        List.AppendLine(":Room pull      : " + (currentRoom.RoomData.PullEnabled == true ? "Habilitado" : "Deshabilitado"));
                        List.AppendLine(":Room push      : " + (currentRoom.RoomData.PushEnabled == true ? "Habilitado" : "Deshabilitado"));
                        List.AppendLine(":Room golpes    : " + (currentRoom.RoomData.GolpeEnabled == true ? "Habilitado" : "Deshabilitado"));
                        List.AppendLine(":Room spull     : " + (currentRoom.RoomData.SPullEnabled == true ? "Habilitado" : "Deshabilitado"));
                        List.AppendLine(":Room spush     : " + (currentRoom.RoomData.SPushEnabled == true ? "Habilitado" : "Deshabilitado"));
                        List.AppendLine(":Room respect   : " + (currentRoom.RoomData.RespectNotificationsEnabled == true ? "Habilitado" : "Deshabilitado"));
                        List.AppendLine(":Room enable    : " + (currentRoom.RoomData.EnablesEnabled == true ? "Habilitado" : "Deshabilitado"));
                        List.AppendLine(":Room robar     : " + (currentRoom.RoomData.RobarEnabled == true ? "Habilitado" : "Deshabilitado"));
                        List.AppendLine(":Room quemar    : " + (currentRoom.RoomData.BurnEnabled == true ? "Habilitado" : "Deshabilitado"));
                        List.AppendLine(":Room besos     : " + (currentRoom.RoomData.BesarEnabled == true ? "Habilitado" : "Deshabilitado"));
                        List.AppendLine(":Room muertes   : " + (currentRoom.RoomData.MatarEnabled == true ? "Habilitado" : "Deshabilitado"));
                        List.AppendLine(":Room sexo      : " + (currentRoom.RoomData.SexEnabled == true ? "Habilitado" : "Deshabilitado"));
                        List.AppendLine(":Room fumar     : " + (currentRoom.RoomData.CrispyEnabled == true ? "Habilitado" : "Deshabilitado"));



                        Session.SendNotification(List.ToString());
                        break;
                    }
                case "burn":
                case "quemar":
                    {
                        currentRoom.RoomData.BurnEnabled = !currentRoom.RoomData.BurnEnabled;
                        using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `burn_enabled` = @BurnEnabled WHERE `id` = '" + currentRoom.RoomData.Id + "' LIMIT 1");
                            dbClient.AddParameter("BurnEnabled", AkiledEnvironment.BoolToEnum(currentRoom.RoomData.BurnEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("El asado humano, en esta sala esta " + (currentRoom.RoomData.BurnEnabled == true ? "Habilitado!" : "Deshabilitado!"));
                        break;
                    }
                case "sexo":
                case "sex":
                    {
                        currentRoom.RoomData.SexEnabled = !currentRoom.RoomData.SexEnabled;
                        using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `sex_enabled` = @SexEnabled WHERE `id` = '" + currentRoom.RoomData.Id + "' LIMIT 1");
                            dbClient.AddParameter("SexEnabled", AkiledEnvironment.BoolToEnum(currentRoom.RoomData.SexEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("El sexo, en esta sala esta " + (currentRoom.RoomData.SexEnabled == true ? "Habilitado!" : "Deshabilitado!"));
                        break;
                    }
                case "atracos":
                case "robar":
                    {
                        currentRoom.RoomData.RobarEnabled = !currentRoom.RoomData.RobarEnabled;
                        using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `robar_enabled` = @RobarEnabled WHERE `id` = '" + currentRoom.RoomData.Id + "' LIMIT 1");
                            dbClient.AddParameter("RobarEnabled", AkiledEnvironment.BoolToEnum(currentRoom.RoomData.RobarEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Los Atracos (robos), en esta sala estan " + (currentRoom.RoomData.RobarEnabled == true ? "Habilitado!" : "Deshabilitado!"));
                        break;
                    }
                case "besos":
                case "besar":
                    {
                        currentRoom.RoomData.BesarEnabled = !currentRoom.RoomData.BesarEnabled;
                        using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `besar_enabled` = @BesarEnabled WHERE `id` = '" + currentRoom.RoomData.Id + "' LIMIT 1");
                            dbClient.AddParameter("BesarEnabled", AkiledEnvironment.BoolToEnum(currentRoom.RoomData.BesarEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Los Besos (pikitos), en esta sala estan " + (currentRoom.RoomData.BesarEnabled == true ? "Habilitado!" : "Deshabilitado!"));
                        break;
                    }
                case "muertes":
                case "matar":
                    {
                        currentRoom.RoomData.MatarEnabled = !currentRoom.RoomData.MatarEnabled;
                        using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `matar_enabled` = @MatarEnabled WHERE `id` = '" + currentRoom.RoomData.Id + "' LIMIT 1");
                            dbClient.AddParameter("MatarEnabled", AkiledEnvironment.BoolToEnum(currentRoom.RoomData.MatarEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Las Muertes, en esta sala estan " + (currentRoom.RoomData.MatarEnabled == true ? "Habilitado!" : "Deshabilitado!"));
                        break;
                    }
                case "golpe":
                case "golpes":
                    {
                        currentRoom.RoomData.GolpeEnabled = !currentRoom.RoomData.GolpeEnabled;
                        using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `golpe_enabled` = @GolpeEnabled WHERE `id` = '" + currentRoom.RoomData.Id + "' LIMIT 1");
                            dbClient.AddParameter("GolpeEnabled", AkiledEnvironment.BoolToEnum(currentRoom.RoomData.GolpeEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Los golpes en esta sala estan " + (currentRoom.RoomData.GolpeEnabled == true ? "Habilitado!" : "Deshabilitado!"));
                        break;
                    }
                case "crispy":
                case "criperos":
                case "fumar":
                case "smokee":
                    {
                        currentRoom.RoomData.CrispyEnabled = !currentRoom.RoomData.CrispyEnabled;
                        using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `crispy_enabled` = @CrispyEnabled WHERE `id` = '" + currentRoom.RoomData.Id + "' LIMIT 1");
                            dbClient.AddParameter("CrispyEnabled", AkiledEnvironment.BoolToEnum(currentRoom.RoomData.CrispyEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Los criperos (fuma crispy) en esta sala estan " + (currentRoom.RoomData.CrispyEnabled == true ? "Habilitado!" : "Deshabilitado!"));
                        break;
                    }


                case "push":
                    {
                        currentRoom.RoomData.PushEnabled = !currentRoom.RoomData.PushEnabled;
                        using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `push_enabled` = @PushEnabled WHERE `id` = '" + currentRoom.RoomData.Id + "' LIMIT 1");
                            dbClient.AddParameter("PushEnabled", AkiledEnvironment.BoolToEnum(currentRoom.RoomData.PushEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Modo Push ahora esta " + (currentRoom.RoomData.PushEnabled == true ? "Habilitado!" : "Deshabilitado!"));
                        break;
                    }

                case "spush":
                    {
                        currentRoom.RoomData.SPushEnabled = !currentRoom.RoomData.SPushEnabled;
                        using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `spush_enabled` = @PushEnabled WHERE `id` = '" + currentRoom.RoomData.Id + "' LIMIT 1");
                            dbClient.AddParameter("PushEnabled", AkiledEnvironment.BoolToEnum(currentRoom.RoomData.SPushEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Modo Super Push ahora esta " + (currentRoom.RoomData.SPushEnabled == true ? "Habilitado!" : "Deshabilitado!"));
                        break;
                    }

                case "spull":
                    {
                        currentRoom.RoomData.SPullEnabled = !currentRoom.RoomData.SPullEnabled;
                        using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `spull_enabled` = @PullEnabled WHERE `id` = '" + currentRoom.RoomData.Id + "' LIMIT 1");
                            dbClient.AddParameter("PullEnabled", AkiledEnvironment.BoolToEnum(currentRoom.RoomData.SPullEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Modo Super Pull ahora esta  " + (currentRoom.RoomData.SPullEnabled == true ? "Habilitado!" : "Deshabilitado!"));
                        break;
                    }

                case "pull":
                    {
                        currentRoom.RoomData.PullEnabled = !currentRoom.RoomData.PullEnabled;
                        using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `pull_enabled` = @PullEnabled WHERE `id` = '" + currentRoom.RoomData.Id + "' LIMIT 1");
                            dbClient.AddParameter("PullEnabled", AkiledEnvironment.BoolToEnum(currentRoom.RoomData.PullEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Modo Pull ahora esta " + (currentRoom.RoomData.PullEnabled == true ? "Habilitado!" : "Deshabilitado!"));
                        break;
                    }

                case "enable":
                case "enables":
                    {
                        currentRoom.RoomData.EnablesEnabled = !currentRoom.RoomData.EnablesEnabled;
                        using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `enables_enabled` = @EnablesEnabled WHERE `id` = '" + currentRoom.RoomData.Id + "' LIMIT 1");
                            dbClient.AddParameter("EnablesEnabled", AkiledEnvironment.BoolToEnum(currentRoom.RoomData.EnablesEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Los efectos en esta sala estan " + (currentRoom.RoomData.EnablesEnabled == true ? "Habilitados!" : "Deshabilitados!"));
                        break;
                    }

                case "respect":
                case "respetos":
                    {
                        currentRoom.RoomData.RespectNotificationsEnabled = !currentRoom.RoomData.RespectNotificationsEnabled;
                        using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `respect_notifications_enabled` = @RespectNotificationsEnabled WHERE `id` = '" + currentRoom.RoomData.Id + "' LIMIT 1");
                            dbClient.AddParameter("RespectNotificationsEnabled", AkiledEnvironment.BoolToEnum(currentRoom.RoomData.RespectNotificationsEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Notificaciones de Respeto estan " + (currentRoom.RoomData.RespectNotificationsEnabled == true ? "Habilitado!" : "Deshabilitado!"));
                        break;
                    }
                case "pets":
                case "morphs":
                case "mascota":
                    {
                        Room.PetMorphsAllowed = !Room.PetMorphsAllowed;
                        using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `pet_morphs_allowed` = @PetMorphsAllowed WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("PetMorphsAllowed", AkiledEnvironment.BoolToEnum(Room.PetMorphsAllowed));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Que se convierta en mascotas esta " + (Room.PetMorphsAllowed == true ? "Habilitado!" : "Deshabilitado!"));

                        if (!Room.PetMorphsAllowed)
                        {
                            foreach (RoomUser User in Room.GetRoomUserManager().GetRoomUsers())
                            {
                                if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                                    continue;

                                User.GetClient().SendWhisper("El propietario de la sala ha deshabilitado la opcion de convertirse en mascota.");
                                if (User.GetClient().GetHabbo().PetId > 0)
                                {
                                    //Tell the user what is going on.
                                    User.GetClient().SendWhisper("Oops, el dueño de la sala solo permite Usuarios normales, no mascotas..");

                                    //Change the users Pet Id.
                                    User.GetClient().GetHabbo().PetId = 0;

                                    //Quickly remove the old user instance.
                                    //Room.SendMessage(new UsersComposer(User.VirtualId));

                                    //Add the new one, they won't even notice a thing!!11 8-)
                                    //Room.SendMessage(new UsersComposer(User));
                                }
                            }
                        }
                        break;
                    }
            }
        }
    }
}
