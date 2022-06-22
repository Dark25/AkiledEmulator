

using Akiled.HabboHotel.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Akiled.HabboHotel.Cache.Process
{
    internal sealed class ProcessComponent
    {
        private Timer _timer = (Timer)null;
        private bool _timerRunning = false;
        private bool _timerLagging = false;
        private bool _disabled = false;
        private AutoResetEvent _resetEvent = new AutoResetEvent(true);
        private static int _runtimeInSec = 1200;

        public void Init() => this._timer = new Timer(new TimerCallback(this.Run), (object)null, ProcessComponent._runtimeInSec * 1000, ProcessComponent._runtimeInSec * 1000);

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
                    List<UserCache> list1 = AkiledEnvironment.GetGame().GetCacheManager().GetUserCache().ToList<UserCache>();
                    if (list1.Count > 0)
                    {
                        foreach (UserCache userCache in list1)
                        {
                            try
                            {
                                if (userCache != null)
                                {
                                    UserCache User = (UserCache)null;
                                    if (userCache.isExpired())
                                        AkiledEnvironment.GetGame().GetCacheManager().TryRemoveUser(userCache.Id, out User);
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                    List<Habbo> list2 = AkiledEnvironment.GetUsersCached().ToList<Habbo>();
                    if (list2.Count > 0)
                    {
                        foreach (Habbo habbo in list2)
                        {
                            try
                            {
                                if (habbo != null)
                                {
                                    Habbo Data = (Habbo)null;
                                    if (habbo.CacheExpired())
                                        AkiledEnvironment.RemoveFromCache(habbo.Id, out Data);
                                    Data?.Dispose();
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                    this._timerRunning = false;
                    this._timerLagging = false;
                    this._resetEvent.Set();
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void Dispose()
        {
            try
            {
                this._resetEvent.WaitOne(TimeSpan.FromMinutes(5.0));
            }
            catch
            {
            }
            this._disabled = true;
            try
            {
                if (this._timer != null)
                    this._timer.Dispose();
            }
            catch
            {
            }
            this._timer = (Timer)null;
        }
    }
}
