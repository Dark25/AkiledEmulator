using Akiled.Communication.Packets.Outgoing.Users;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Users;
using System.Collections.Generic;

namespace Akiled.Communication.Packets.Incoming.Users
{
    internal class GetIgnoredUsersEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            List<string> ignoredUsers = new();

            var ignored = session.GetHabbo().GetIgnores()?.IgnoredUserIds();
            
            if (ignored == null)
                return;

            foreach (int userId in ignored)
            {
                Habbo player = AkiledEnvironment.GetHabboById(userId);
                if (player != null)
                {
                    if (!ignoredUsers.Contains(player.Username))
                        ignoredUsers.Add(player.Username);
                }
            }

            session.SendPacket(new IgnoredUsersComposer(ignoredUsers));
        }
    }
}