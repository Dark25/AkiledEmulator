using System;

namespace Akiled.HabboHotel.Rooms
{
    public class UserSaysArgs : EventArgs
    {
        public readonly RoomUser User;
        public readonly string Message;

        public UserSaysArgs(RoomUser user, string message)
        {
            this.User = user;
            this.Message = message;
        }
    }
}
