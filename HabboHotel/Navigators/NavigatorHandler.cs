using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Groups;
using Akiled.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Akiled.HabboHotel.Navigators
{
    static class NavigatorHandler
    {
        public static void Search(ServerPacket Message, SearchResultList SearchResult, string SearchData, GameClient Session, int FetchLimit)
        {
            //Switching by categorys.
            switch (SearchResult.CategoryType)
            {
                default:
                    Message.WriteInteger(0);
                    break;

                case NavigatorCategoryType.QUERY:
                    {
                        #region Query
                        if (SearchData.ToLower().StartsWith("owner:"))
                        {
                            if (SearchData.Length > 0)
                            {
                                DataTable GetRooms = null;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    if (SearchData.ToLower().StartsWith("owner:"))
                                    {
                                        dbClient.SetQuery("SELECT * FROM `rooms` WHERE `owner` = @username and `state` != 'invisible' ORDER BY `users_now` DESC");
                                        dbClient.AddParameter("username", SearchData.Remove(0, 6));
                                        GetRooms = dbClient.GetTable();
                                    }
                                }

                                List<RoomData> Results = new List<RoomData>();
                                if (GetRooms != null)
                                {
                                    foreach (DataRow Row in GetRooms.Rows)
                                    {
                                        RoomData RoomData = AkiledEnvironment.GetGame().GetRoomManager().FetchRoomData(Convert.ToInt32(Row["id"]), Row);
                                        if (RoomData != null && !Results.Contains(RoomData))
                                            Results.Add(RoomData);
                                    }
                                }

                                Message.WriteInteger(Results.Count);
                                foreach (RoomData Data in Results.ToList())
                                {
                                    RoomAppender.WriteRoom(Message, Data);
                                }
                            }
                        }
                        else if (SearchData.ToLower().StartsWith("tag:"))
                        {
                            SearchData = SearchData.Remove(0, 4);
                            ICollection<RoomData> TagMatches = AkiledEnvironment.GetGame().GetRoomManager().SearchTaggedRooms(SearchData);

                            Message.WriteInteger(TagMatches.Count);
                            foreach (RoomData Data in TagMatches.ToList())
                            {
                                RoomAppender.WriteRoom(Message, Data);
                            }
                        }
                        else if (SearchData.ToLower().StartsWith("group:"))
                        {
                            SearchData = SearchData.Remove(0, 6);
                            ICollection<RoomData> GroupRooms = AkiledEnvironment.GetGame().GetRoomManager().SearchGroupRooms(SearchData);

                            Message.WriteInteger(GroupRooms.Count);
                            foreach (RoomData Data in GroupRooms.ToList())
                            {
                                RoomAppender.WriteRoom(Message, Data);
                            }
                        }
                        else
                        {
                            if (SearchData.Length > 0)
                            {
                                DataTable Table = null;
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT * FROM rooms WHERE caption LIKE @query OR owner LIKE '@query' ORDER BY users_now DESC LIMIT 50");
                                    dbClient.AddParameter("query", SearchData.Replace("%", "\\%").Replace("_", "\\_") + "%");
                                    Table = dbClient.GetTable();
                                }

                                List<RoomData> Results = new List<RoomData>();
                                if (Table != null)
                                {
                                    foreach (DataRow Row in Table.Rows)
                                    {
                                        if (Convert.ToString(Row["state"]) == "invisible")
                                            continue;

                                        RoomData RData = AkiledEnvironment.GetGame().GetRoomManager().FetchRoomData(Convert.ToInt32(Row["id"]), Row);
                                        if (RData != null && !Results.Contains(RData))
                                            Results.Add(RData);
                                    }
                                }

                                Message.WriteInteger(Results.Count);
                                foreach (RoomData Data in Results.ToList())
                                {
                                    RoomAppender.WriteRoom(Message, Data);
                                }
                            }
                        }
                        #endregion

                        break;
                    }

                case NavigatorCategoryType.FEATURED:
                    #region Featured
                    List<RoomData> Rooms = new List<RoomData>();
                    ICollection<FeaturedRoom> Featured = AkiledEnvironment.GetGame().GetNavigator().GetFeaturedRooms(Session.Langue);
                    foreach (FeaturedRoom FeaturedItem in Featured.ToList())
                    {
                        if (FeaturedItem == null)
                            continue;

                        RoomData Data = AkiledEnvironment.GetGame().GetRoomManager().GenerateRoomData(FeaturedItem.RoomId);
                        if (Data == null)
                            continue;

                        if (!Rooms.Contains(Data))
                            Rooms.Add(Data);
                    }

                    Message.WriteInteger(Rooms.Count);
                    foreach (RoomData Data in Rooms.ToList())
                    {
                        RoomAppender.WriteRoom(Message, Data);
                    }
                    #endregion
                    break;

                case NavigatorCategoryType.POPULAR:
                    {
                        List<RoomData> PopularRooms = new List<RoomData>();

                        //RoomData FistRoom = AkiledEnvironment.GetGame().GetRoomManager().GenerateRoomData(5351934);
                        //if(FistRoom != null)
                        //PopularRooms.Add(FistRoom);

                        PopularRooms.AddRange(AkiledEnvironment.GetGame().GetRoomManager().GetPopularRooms(-1, 50, Session.Langue)); //FetchLimit

                        Message.WriteInteger(PopularRooms.Count);
                        foreach (RoomData Data in PopularRooms.ToList())
                        {
                            RoomAppender.WriteRoom(Message, Data);
                        }
                        break;
                    }

                case NavigatorCategoryType.RECOMMENDED:
                    {
                        List<RoomData> RecommendedRooms = AkiledEnvironment.GetGame().GetRoomManager().GetRecommendedRooms(FetchLimit);

                        Message.WriteInteger(RecommendedRooms.Count);
                        foreach (RoomData Data in RecommendedRooms.ToList())
                        {
                            RoomAppender.WriteRoom(Message, Data);
                        }
                        break;
                    }
                case NavigatorCategoryType.STAFF_PICKS:
                    List<RoomData> source2 = new List<RoomData>();
                    foreach (StaffPick staffPick in AkiledEnvironment.GetGame().GetNavigator().GetStaffPicks().ToList<StaffPick>())
                    {
                        if (staffPick != null)
                        {
                            RoomData roomData = AkiledEnvironment.GetGame().GetRoomManager().GenerateRoomData(staffPick.RoomId);
                            if (roomData != null && !source2.Contains(roomData))
                                source2.Add(roomData);
                        }
                    }
                    Message.WriteInteger(source2.Count);
                    using (List<RoomData>.Enumerator enumerator = source2.ToList<RoomData>().GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            RoomData current = enumerator.Current;
                            RoomAppender.WriteRoom(Message, current);
                        }
                        break;
                    }

                case NavigatorCategoryType.CATEGORY:
                    {
                        List<RoomData> GetRoomsByCategory = AkiledEnvironment.GetGame().GetRoomManager().GetRoomsByCategory(SearchResult.Id, FetchLimit);

                        Message.WriteInteger(GetRoomsByCategory.Count);
                        foreach (RoomData Data in GetRoomsByCategory.ToList())
                        {
                            RoomAppender.WriteRoom(Message, Data);
                        }
                        break;
                    }

                case NavigatorCategoryType.MY_ROOMS:

                    Message.WriteInteger(Session.GetHabbo().UsersRooms.Count);
                    foreach (RoomData Data in Session.GetHabbo().UsersRooms.OrderBy(a => a.Name).ToList())
                    {
                        RoomAppender.WriteRoom(Message, Data);
                    }
                    break;

                case NavigatorCategoryType.MY_FAVORITES:
                    List<RoomData> Favourites = new List<RoomData>();
                    foreach (RoomData Room in Session.GetHabbo().FavoriteRooms.ToArray())
                    {

                        if (!Favourites.Contains(Room))
                            Favourites.Add(Room);
                    }

                    Favourites = Favourites.Take(FetchLimit).ToList();

                    Message.WriteInteger(Favourites.Count);
                    foreach (RoomData Data in Favourites.ToList())
                    {
                        RoomAppender.WriteRoom(Message, Data);
                    }
                    break;

                case NavigatorCategoryType.MY_GROUPS:
                    List<RoomData> MyGroups = new List<RoomData>();

                    foreach (int GroupId in Session.GetHabbo().MyGroups.ToList())
                    {
                        Group Group;
                        if (!AkiledEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group))
                            continue;

                        RoomData Data = AkiledEnvironment.GetGame().GetRoomManager().GenerateRoomData(Group.RoomId);
                        if (Data == null)
                            continue;

                        if (!MyGroups.Contains(Data))
                            MyGroups.Add(Data);
                    }

                    MyGroups = MyGroups.Take(FetchLimit).ToList();

                    Message.WriteInteger(MyGroups.Count);
                    foreach (RoomData Data in MyGroups.ToList())
                    {
                        RoomAppender.WriteRoom(Message, Data);
                    }
                    break;

                /*case NavigatorCategoryType.MY_FRIENDS_ROOMS:
                    List<RoomData> MyFriendsRooms = new List<RoomData>();
                    foreach (MessengerBuddy buddy in Session.GetHabbo().GetMessenger().GetFriends().Where(p => p.))
                    {
                        if (buddy == null || !buddy.InRoom || buddy.UserId == Session.GetHabbo().Id)
                            continue;

                        if (!MyFriendsRooms.Contains(buddy.CurrentRoom.RoomData))
                            MyFriendsRooms.Add(buddy.CurrentRoom.RoomData);
                    }

                    Message.WriteInteger(MyFriendsRooms.Count);
                    foreach (RoomData Data in MyFriendsRooms.ToList())
                    {
                        RoomAppender.WriteRoom(Message, Data);
                    }
                    break;*/

                case NavigatorCategoryType.MY_RIGHTS:
                    List<RoomData> MyRights = new List<RoomData>();

                    foreach (RoomData Room in Session.GetHabbo().RoomRightsList.ToArray())
                    {
                        if (Room == null)
                            continue;

                        if (!MyRights.Contains(Room))
                            MyRights.Add(Room);
                    }

                    MyRights = MyRights.Take(FetchLimit).ToList();

                    Message.WriteInteger(MyRights.Count);
                    foreach (RoomData Data in MyRights.ToList())
                    {
                        RoomAppender.WriteRoom(Message, Data);
                    }
                    break;
            }
        }
    }
}