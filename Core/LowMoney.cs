
using Akiled.HabboHotel.Users;
using System;
using System.Threading;

namespace Akiled.Core
{
    public class LowMoney
    {
        private string segundospremio = AkiledEnvironment.GetConfig().data[nameof(segundospremio)];
        private string segundospremiorp = AkiledEnvironment.GetConfig().data[nameof(segundospremiorp)];
        private Habbo _player = (Habbo)null;
        private Timer _timer = (Timer)null;
        private Timer _timer2 = (Timer)null;
        private bool _timerRunning = false;
        private bool _timerRunningrp = false;
        private bool _timerLagging = false;
        private bool _timerLaggingrp = false;
        private bool _disabled = false;
        private bool _disabledrp = false;
        private AutoResetEvent _resetEvent = new AutoResetEvent(true);
        private AutoResetEvent _resetEventrp = new AutoResetEvent(true);
        private static int _runtimeInSec = 60;
        private static int _runrptimeInSec = 40;

        public bool Init(Habbo Player)
        {
            if (Player == null || this._player != null)
                return false;
            this._player = Player;
            this._timer = new Timer(new TimerCallback(this.Run), (object)null, LowMoney._runtimeInSec * Convert.ToInt32(this.segundospremio), LowMoney._runtimeInSec * Convert.ToInt32(this.segundospremio));
            this._timer2 = new Timer(new TimerCallback(this.Runrp), (object)null, LowMoney._runrptimeInSec * Convert.ToInt32(this.segundospremiorp), LowMoney._runrptimeInSec * Convert.ToInt32(this.segundospremiorp));
            return true;
        }

        public void Run(object State)
        {
            try
            {
                if (this._disabled)
                    return;
                if (this._timerRunning)
                {
                    this._timerLagging = true;
                }
                else
                {
                    this._resetEvent.Reset();
                    if (this._player.TimeMuted > 0.0)
                        this._player.TimeMuted -= 60.0;
                    this._player.CheckCreditsTimer();
                    this._timerRunning = false;
                    this._timerLagging = false;
                    this._resetEvent.Set();
                }
            }
            catch
            {
            }
        }

        public void Runrp(object State)
        {
            try
            {
                if (this._disabledrp)
                    return;
                if (this._timerRunningrp)
                {
                    this._timerLaggingrp = true;
                }
                else
                {
                    this._resetEventrp.Reset();
                    if (this._player.TimeMuted > 0.0)
                        this._player.TimeMuted -= 60.0;
                    this._player.CheckCreditsTimerRP();
                    this._timerRunningrp = false;
                    this._timerLaggingrp = false;
                    this._resetEventrp.Set();
                }
            }
            catch
            {
            }
        }

        public void Dispose()
        {
            try
            {
                this._resetEvent.WaitOne(TimeSpan.FromMinutes(5.0));
                this._resetEventrp.WaitOne(TimeSpan.FromMinutes(5.0));
            }
            catch
            {
            }
            this._disabled = true;
            this._disabledrp = true;
            try
            {
                if (this._timer != null)
                    this._timer.Dispose();
                if (this._timer2 != null)
                    this._timer2.Dispose();
            }
            catch
            {
            }
            this._timer = (Timer)null;
            this._timer2 = (Timer)null;
            this._player = (Habbo)null;
        }
    }
}
