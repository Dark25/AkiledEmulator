using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Akiled.HabboHotel.Animations
{
    public class AnimationManager
    {
        private const int MIN_USERS = 0;
        private const int START_TIME = 2;
        private const int NOTIF_TIME = 1;
        private const int CLOSE_TIME = 1;

        private List<int> _roomId;
        private bool _started;
        private bool _skipCycle;
        private int _timer;
        private int _roomIdGame;

        private bool _isActivate;
        private bool _notif;
        private bool _forceDisabled;
        private int _cycleId;

        public void OnUpdateUsersOnline(int UsersOnline)
        {
            if (this._isActivate && UsersOnline < MIN_USERS) this._isActivate = false;
            else if (!this._isActivate && UsersOnline >= MIN_USERS) this._isActivate = true;
        }

        public bool ToggleForceDisabled()
        {
            this._forceDisabled = !this._forceDisabled;

            return this._forceDisabled;
        }

        public AnimationManager()
        {
            this._roomId = new List<int>();
            this._started = false;
            this._timer = 0;
            this._roomIdGame = 0;
            this._isActivate = true;
            this._notif = false;
            this._skipCycle = false;
            this._forceDisabled = false;
            this._cycleId = 0;
        }

        public bool IsActivate()
        {
            return !this._forceDisabled && this._isActivate;
        }

        public bool AllowAnimation()
        {
            if (this._started) return false;

            if (this._timer >= this.GetMinutes(START_TIME - NOTIF_TIME)) return false;

            this._timer = 0;

            return true;
        }

        public string GetTime()
        {
            TimeSpan time = TimeSpan.FromSeconds(this.GetMinutes(START_TIME) - this._timer);

            return time.Minutes + " minutes et " + time.Seconds + " secondes";
        }

        public void Init()
        {
            this._roomId.Clear();

            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT id FROM rooms WHERE owner = 'AkiledGames'");

                DataTable table = dbClient.GetTable();
                if (table == null) return;

                foreach (DataRow dataRow in table.Rows)
                {
                    if (this._roomId.Contains(Convert.ToInt32(dataRow["id"]))) continue;

                    this._roomId.Add(Convert.ToInt32(dataRow["id"]));
                }
            }

            if (this._roomId.Count == 0) this._forceDisabled = true;
        }

        private void Cycle()
        {
            if (!this._isActivate && !this._started) return;

            if (this._forceDisabled && !this._started) return;

            if (this._skipCycle)
            {
                this._timer++;
                this._skipCycle = false;
            }
            else
                this._skipCycle = true;

            if (this._started)
            {
                if (this._timer >= this.GetMinutes(CLOSE_TIME))
                {
                    Room Rooom = AkiledEnvironment.GetGame().GetRoomManager().LoadRoom(this._roomIdGame);

                    this._started = false;

                    if (Rooom != null) Rooom.RoomData.State = 1;
                }
                return;
            }

            if (this._timer >= this.GetMinutes(START_TIME - NOTIF_TIME) && !this._notif)
            {
                this._notif = true;
                AkiledEnvironment.GetGame().GetClientWebManager().SendMessage(new NotifTopComposer("Jack & Daisy: La prochaine animation démarre dans 2 minutes !"), Core.Language.FRANCAIS);
            }

            if (this._timer >= this.GetMinutes(START_TIME))
            {
                if (this._cycleId >= this._roomId.Count)
                {
                    this._cycleId = 0;
                    this._roomId = this._roomId.OrderBy(a => Guid.NewGuid()).ToList();
                }

                int RoomId = this._roomId[this._cycleId]; //AkiledEnvironment.GetRandomNumber(0, this._roomId.Count - 1)
                this._cycleId++;

                Room room = AkiledEnvironment.GetGame().GetRoomManager().LoadRoom(RoomId);
                if (room == null) return;

                this._timer = 0;
                this._started = true;
                this._notif = false;
                this._roomIdGame = RoomId;

                room.RoomData.State = 0;
                room.CloseFullRoom = true;
                string event_alert = (AkiledEnvironment.GetConfig().data["event_alert"]);

                string AlertMessage = "<i>Beep beep, c'est l'heure d'une animation !</i>" +
                "\r\r" +
                "Rejoins-nous chez <b>AkiledGames</b> pour un jeu qui s'intitule <b>" + Encoding.UTF8.GetString(Encoding.GetEncoding("Windows-1252").GetBytes(room.RoomData.Name)) + "</b>" +
                "\r\r" +
                "Rends-toi dans l'appartement et tente de remporter un lot composé de <i>une ou plusieurs RareBox(s) et BadgeBox(s) ainsi qu'un point au TOP Gamer !</i>" +
                "\r\n" +
                "\r\n- Jack et Daisy\r\n";

                AkiledEnvironment.GetGame().GetModerationManager().LogStaffEntry(1953042, "AkiledGames", room.Id, string.Empty, "eventha", string.Format("JeuAuto EventHa: {0}", AlertMessage));
                AkiledEnvironment.GetGame().GetClientWebManager().SendMessage(new NotifAlertComposer(event_alert, "Message d'animation", AlertMessage, "Je veux y jouer !", room.Id, ""));
            }
        }

        public void OnCycle(Stopwatch moduleWatch)
        {
            this.Cycle();
            HandleFunctionReset(moduleWatch, "AnimationCycle");
        }

        private int GetMinutes(int minutes)
        {
            return (minutes * 60);
        }

        private void HandleFunctionReset(Stopwatch watch, string methodName)
        {
            try
            {
                if (watch.ElapsedMilliseconds > 500)
                    Console.WriteLine("High latency in {0}: {1}ms", methodName, watch.ElapsedMilliseconds);
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine("Canceled operation {0}", e);

            }
            watch.Restart();
        }

    }
}