using Akiled.HabboHotel.GameClients;
using AkiledEmulator.HabboHotel.Hotel.Giveaway;
using System;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class GiveawayCommand : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length > 1 && Session.GetHabbo().HasFuse("giveaway_manager_command"))
            {
                string option = Params[1];

                if (option.Equals("stop") || option.Equals("off"))
                {
                    GiveAwayConfigs.Stop();
                    return;
                }

                if (GiveAwayConfigs.enabled)
                {
                    Session.SendWhisper("¡Ya habilitado! Si desea deshabilitar use la opción stop", 3);
                    return;
                }

                if (option.Equals("start") || option.Equals("on"))
                {
                    if (!int.TryParse(Params[2], out int time))
                        return;

                    if (time < 5 || time > 3600)
                    {
                        Session.SendWhisper("Tiempo en segundos. Entre 5 y 1h.", 1);
                        return;
                    }

                    if (Params.Length == 2)
                    {
                        Session.SendWhisper("Tiempo en segundos. Entre 5 y 1h.", 1);
                        return;
                    }

                    if (!int.TryParse(Params[3], out int maxWinners))
                        return;

                    if (maxWinners <= 0 || maxWinners > 100)
                    {
                        Session.SendWhisper("Ganadores máximos. Entre 1 y 100.", 1);
                        return;
                    }

                    if (Params.Length == 3)
                    {
                        Session.SendWhisper("Ganadores máximos. Entre 1 y 100.", 1);
                        return;
                    }

                    string description = CommandManager.MergeParams(Params, 4);

                    //Updates
                    GiveAwayConfigs.enabled = !GiveAwayConfigs.enabled;
                    GiveAwayConfigs.timestamp = AkiledEnvironment.GetUnixTimestamp() + time;
                    GiveAwayConfigs.maxWinners = maxWinners;
                    GiveAwayConfigs.description = description;
                    GiveAwayConfigs.startedByUsername = Session.GetHabbo().Username;
                    GiveAwayConfigs.Start();
                }
                else
                    Session.SendWhisper("¡Opción inválida!", 3);
            }
            else
            {
                //Check if is enabled
                if (!GiveAwayConfigs.enabled)
                {
                    Session.SendWhisper("¡No hay ningún sorteo en marcha en este momento!", 3);
                    return;
                }

                //Check if the user can participate
                if (AkiledEnvironment.GetGame().GetGiveAwayBlocks().TryGet(Session.GetHabbo().Id, out GiveAwayBlocks giveAwayBlocks))
                    if (AkiledEnvironment.GetGame().GetGiveAwayBlocks().CheckTimeExpire(Session.GetHabbo().Id, giveAwayBlocks))
                    {
                        DateTime timeMissing = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Convert.ToDouble(giveAwayBlocks.timestamp - AkiledEnvironment.GetUnixTimestamp()));

                        Session.SendWhisper("Puedes intentarlo de nuevo en " + (timeMissing.Minute > 0 ? timeMissing.Minute + " minutos " + timeMissing.Second + "segundos!" : timeMissing.Second + " segundo!", 3));
                        return;
                    }

                DateTime data = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(GiveAwayConfigs.timestamp - AkiledEnvironment.GetUnixTimestamp());

                if (!GiveAwayConfigs.participants.Contains(Session.GetHabbo().Id))
                    GiveAwayConfigs.participants.Add(Session.GetHabbo().Id);
                else
                {
                    Session.SendWhisper("Puedes intentarlo de nuevo en " + (data.Minute > 0 ? data.Minute + " minutos " + data.Second + "segundos!" : data.Second + " segundo!", 3));
                    return;
                }

                Session.SendWhisper("¡Estás participando! Necesitas esperar " + (data.Minute > 0 ? data.Minute + " minutos " + data.Second + "segundos!" : data.Second + " segundo!", 3));
            }
        }
    }
}
