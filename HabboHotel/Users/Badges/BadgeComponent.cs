using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using System.Collections;
using System.Collections.Generic;

namespace Akiled.HabboHotel.Users.Badges
{
    public class BadgeComponent
    {
        private Dictionary<string, Badge> _badges;
        private int _userId;

        public BadgeComponent(int userId, List<Badge> data)
        {
            this._badges = new Dictionary<string, Badge>();
            foreach (Badge badge in data)
            {
                if (!this._badges.ContainsKey(badge.Code))
                    this._badges.Add(badge.Code, badge);
            }
            this._userId = userId;
        }

        public int Count => this._badges.Count;

        public int EquippedCount
        {
            get
            {
                int num = 0;
                foreach (Badge badge in (IEnumerable)this._badges.Values)
                {
                    if (badge.Slot != 0)
                        ++num;
                }
                return num > 8 ? 8 : num;
            }
        }

        public Dictionary<string, Badge> BadgeList => this._badges;

        public void Destroy() => this._badges.Clear();

        public bool HasBadgeSlot(string Badge) => this._badges.ContainsKey(Badge) && this._badges[Badge].Slot > 0;

        public Badge GetBadge(string Badge) => this._badges.ContainsKey(Badge) ? this._badges[Badge] : (Badge)null;

        public bool HasBadge(string Badge) => this._badges.ContainsKey(Badge);

        public void GiveBadge(string Badge, int Slot, bool InDatabase, GameClient Session)
        {
            if (this.HasBadge(Badge))
                return;

            if (InDatabase)
            {
                using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryReactor.SetQuery("INSERT INTO user_badges (user_id,badge_id,badge_slot) VALUES (" + this._userId + ", @badge, " + Slot + ")");
                    queryReactor.AddParameter("badge", Badge);
                    queryReactor.RunQuery();
                }
            }

            this._badges.Add(Badge, new Badge(Badge, Slot));

            if (Session == null)
                return;

            Session.SendMessage(new BadgesComposer(Session));
            Session.SendMessage(new FurniListNotificationComposer(1, 4));
        }
        public ICollection<Badge> GetBadges() => (ICollection<Badge>)this._badges.Values;

        public void GiveBadge(string Badge, int Slot, bool InDatabase)
        {
            if (this.HasBadge(Badge))
                return;

            if (InDatabase)
            {
                using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryReactor.SetQuery("INSERT INTO user_badges (user_id,badge_id,badge_slot) VALUES (" + this._userId + ", @badge, " + Slot + ")");
                    queryReactor.AddParameter("badge", Badge);
                    queryReactor.RunQuery();
                }
            }

            this._badges.Add(Badge, new Badge(Badge, Slot));
        }

        public void ResetSlots()
        {
            foreach (Badge badge in (IEnumerable)this._badges.Values)
                badge.Slot = 0;
        }

        public void RemoveBadge(string Badge)
        {
            if (!this.HasBadge(Badge))
                return;
            using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.SetQuery("DELETE FROM user_badges WHERE badge_id = @badge AND user_id = " + this._userId.ToString() + " LIMIT 1");
                queryReactor.AddParameter("badge", Badge);
                queryReactor.RunQuery();
            }
            this._badges.Remove(this.GetBadge(Badge).Code);
        }

        public ServerPacket Serialize()
        {
            List<Badge> badgeList = new List<Badge>();
            ServerPacket serverPacket = new ServerPacket(717);
            serverPacket.WriteInteger(this.Count);
            foreach (Badge badge in (IEnumerable)this._badges.Values)
            {
                serverPacket.WriteInteger(0);
                serverPacket.WriteString(badge.Code);
                if (badge.Slot > 0)
                    badgeList.Add(badge);
            }
            serverPacket.WriteInteger(badgeList.Count);
            foreach (Badge badge in badgeList)
            {
                serverPacket.WriteInteger(badge.Slot);
                serverPacket.WriteString(badge.Code);
            }
            return serverPacket;
        }
    }
}

