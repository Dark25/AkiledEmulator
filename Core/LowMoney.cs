using System;
using Akiled.Database.Interfaces;
using System.Diagnostics;
using Akiled.HabboHotel.Users;
using System.Threading;

namespace Akiled.Core
{
    public class LowMoney
    {

		string segundospremio = (AkiledEnvironment.GetConfig().data["segundospremio"]);

		private Habbo _player = null;

        private Timer _timer = null;

        private bool _timerRunning = false;

        private bool _timerLagging = false;

        private bool _disabled = false;

        private AutoResetEvent _resetEvent = new AutoResetEvent(true);

        private static int _runtimeInSec = 30;


		public bool Init(Habbo Player)
        {
            if (Player == null)
            {
                return false;
            }
            if (_player != null)
            {
                return false;
            }
            _player = Player;
            _timer = new Timer(Run, null, _runtimeInSec * Convert.ToInt32(segundospremio), _runtimeInSec * Convert.ToInt32(segundospremio));
            return true;
		}


		public void Run(object State)
		{
			try
			{
				if (!_disabled)
				{
					if (_timerRunning)
					{
						_timerLagging = true;

					}
					else
					{
						_resetEvent.Reset();
						if (_player.TimeMuted > 0.0)
						{
							_player.TimeMuted -= 30.0;
						}
						_player.CheckCreditsTimer();
						_timerRunning = true;
						_timerLagging = true;
						_resetEvent.Set();
					}
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
				_resetEvent.WaitOne(TimeSpan.FromMinutes(5.0));
			}
			catch
			{
			}
			_disabled = true;
			try
			{
				if (_timer != null)
				{
					_timer.Dispose();
				}
			}
			catch
			{
			}
			_timer = null;
			_player = null;
		}
	}
}
