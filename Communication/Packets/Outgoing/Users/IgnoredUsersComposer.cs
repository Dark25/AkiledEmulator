using System.Collections.Generic;

namespace Akiled.Communication.Packets.Outgoing.Users
{
    public class IgnoredUsersComposer : ServerPacket
    {
        public IReadOnlyCollection<string> IgnoredUsers { get; }

        public IgnoredUsersComposer(IReadOnlyCollection<string> ignoredUsers)
            : base(ServerPacketHeader.IgnoredUsersMessageComposer)
        {
            IgnoredUsers = ignoredUsers;

            WriteInteger(IgnoredUsers.Count);
            foreach (string username in IgnoredUsers)
            {
                WriteString(username);
            }
        }


    }
}
