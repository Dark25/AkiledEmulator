using Akiled.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class UserUpdateComposer : ServerPacket
    {
        public UserUpdateComposer(ICollection<RoomUser> RoomUsers)
            : base(ServerPacketHeader.UserUpdateMessageComposer)
        {
            WriteInteger(RoomUsers.Count);
            foreach (RoomUser User in RoomUsers.ToList())
            {
                WriteInteger(User.VirtualId);
                WriteInteger(User.X);
                WriteInteger(User.Y);
                WriteString(User.Z.ToString("0.00"));
                WriteInteger(User.RotHead);
                WriteInteger(User.RotBody);

                StringBuilder StatusComposer = new StringBuilder();
                StatusComposer.Append("/");

                foreach (KeyValuePair<string, string> Status in User.Statusses.ToList())
                {
                    StatusComposer.Append(Status.Key);

                    if (!String.IsNullOrEmpty(Status.Value))
                    {
                        StatusComposer.Append(" ");
                        StatusComposer.Append(Status.Value);
                    }

                    StatusComposer.Append("/");
                }

                StatusComposer.Append("/");
                WriteString(StatusComposer.ToString());
            }
        }
    }
}
