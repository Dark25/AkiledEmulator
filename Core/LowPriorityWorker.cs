using Akiled.Database.Interfaces;
using AkiledEmulator.HabboHotel.Hotel.CollectorPark;
using AkiledEmulator.HabboHotel.Hotel.Giveaway;
using System;
using System.Diagnostics;

namespace Akiled.Core
{
    public class LowPriorityWorker
    {
        private static int UserPeak;
        private static bool isExecuted;

        private static string mColdTitle;

        public static void Init()
        {
            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT userpeak FROM server_status");
                UserPeak = dbClient.GetInteger();
            }
            mColdTitle = string.Empty;

            lowPriorityProcessWatch = new Stopwatch();
            lowPriorityProcessWatch.Start();
        }

        private static Stopwatch lowPriorityProcessWatch;
        
        public static void Process()
        {
            if (lowPriorityProcessWatch.ElapsedMilliseconds >= 1000)
            {
                try
                {
                    if (GiveAwayConfigs.enabled)
                    {
                        if (GiveAwayConfigs.timestamp < AkiledEnvironment.GetUnixTimestamp())
                        {
                            try
                            {
                                GiveAwayConfigs.Stop();
                                
                            }
                            catch (Exception ex)
                            {
                                Logging.LogException(ex.ToString());
                            }
                        }
                    }

                    if (CollectorParkConfigs.enabled)
                    {
                        try
                        {
                            CollectorParkConfigs.load();
                        }
                        catch (Exception ex)
                        {
                            Logging.LogException(ex.ToString());
                        }
                    }
                }
                catch
                {

                }
            }

            if (lowPriorityProcessWatch.ElapsedMilliseconds >= 60000 || !isExecuted)
            {
                isExecuted = true;
                lowPriorityProcessWatch.Restart();
                try
                {
                    int UsersOnline = AkiledEnvironment.GetGame().GetClientManager().Count;

                    if (UsersOnline > UserPeak)
                        UserPeak = UsersOnline;

                    int RoomsLoaded = AkiledEnvironment.GetGame().GetRoomManager().Count;

                    TimeSpan Uptime = DateTime.Now - AkiledEnvironment.ServerStarted;

                    mColdTitle = "AkiledEmu | Server ON: " + Uptime.Days + " D " + Uptime.Hours + " H " + Uptime.Minutes + " M | " +
                         "Usuarios ON: " + UsersOnline + " | Salas Cargadas: " + RoomsLoaded;

                    Console.Title = mColdTitle;

                    using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE server_status SET users_online = " + UsersOnline + ", rooms_loaded = " + RoomsLoaded + ", userpeak = " + UserPeak + ", stamp = UNIX_TIMESTAMP();INSERT INTO cms_connecter (connecter, date, appart) VALUES (" + UsersOnline + ", UNIX_TIMESTAMP(), " + RoomsLoaded + ");");
                    }
                }
                catch (Exception e) { Logging.LogThreadException(e.ToString(), "Server status update task"); }
            }
        }
    }
}