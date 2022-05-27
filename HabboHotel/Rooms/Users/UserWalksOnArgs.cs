using System;

namespace Akiled.HabboHotel.Rooms
{
    public class UserWalksOnArgs : EventArgs
    {
        public readonly RoomUser User;

        public UserWalksOnArgs(RoomUser user)
        {
            this.User = user;
        }
    }
}
