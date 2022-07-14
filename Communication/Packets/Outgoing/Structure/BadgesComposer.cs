using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Users.Badges;
using System.Collections.Generic;
using System.Linq;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    internal class BadgesComposer : ServerPacket
    {
        public BadgesComposer(GameClient Session)
          : base(717)
        {
            List<Badge> badgeList = new List<Badge>();
            this.WriteInteger(Session.GetHabbo().GetBadgeComponent().Count);
            foreach (Badge badge in Session.GetHabbo().GetBadgeComponent().GetBadges().ToList<Badge>())
            {
                this.WriteInteger(1);
                this.WriteString(badge.Code);
                if (badge.Slot > 0)
                    badgeList.Add(badge);
            }
            this.WriteInteger(badgeList.Count);
            foreach (Badge badge in badgeList)
            {
                this.WriteInteger(badge.Slot);
                this.WriteString(badge.Code);
            }
        }
    }
}
