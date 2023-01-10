using System;
using System.Collections.Generic;
using System.Linq;

namespace Akiled.HabboHotel.NotifTop
{
    public class NotificationTopManager
    {
        private readonly List<string> _message;

        public NotificationTopManager()
        {
            this._message = new List<string>();
        }

        public void Init()
        {
            this._message.Clear();


            this._message.Add("MENSAJES");

        }

        public List<string> GetAllMessages()
        {
            return this._message.OrderBy(a => Guid.NewGuid()).ToList();
        }
    }
}
