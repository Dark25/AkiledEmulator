using Akiled.HabboHotel.Achievements;
using Akiled.HabboHotel.Users.Badges;
using Akiled.HabboHotel.Users.Messenger;
using System.Collections.Generic;

namespace Akiled.HabboHotel.Users.UserData
{
    public class UserData
    {
        public int userID;
        public Dictionary<string, UserAchievement> achievements;
        public List<int> favouritedRooms;
        public List<Badge> badges;
        public Dictionary<int, MessengerBuddy> friends;
        public Dictionary<int, MessengerRequest> requests;
        public Dictionary<int, int> quests;
        public List<int> MyGroups;
        public Habbo user;
        public Dictionary<int, Relationship> Relationships;
        public List<int> RoomRightsList;

        public UserData(int userID, Dictionary<string, UserAchievement> achievements, List<int> favouritedRooms, List<Badge> badges, Dictionary<int, MessengerBuddy> friends, Dictionary<int, MessengerRequest> requests, Dictionary<int, int> quests, List<int> MyGroups, Habbo user, Dictionary<int, Relationship> relationships, List<int> RoomRightsList)
        {
            this.userID = userID;
            this.achievements = achievements;
            this.favouritedRooms = favouritedRooms;
            this.badges = badges;
            this.friends = friends;
            this.requests = requests;
            this.quests = quests;
            this.user = user;
            this.MyGroups = MyGroups;
            this.Relationships = relationships;
            this.RoomRightsList = RoomRightsList;
        }
    }
}
