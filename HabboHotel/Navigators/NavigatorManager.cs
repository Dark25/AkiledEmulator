using Akiled.Core;
using Akiled.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Akiled.HabboHotel.Navigators
{

    public sealed class NavigatorManager
    {
        private readonly Dictionary<int, FeaturedRoom> _featuredRooms;
        private readonly Dictionary<int, StaffPick> _staffPicks;
        private readonly Dictionary<int, TopLevelItem> _topLevelItems;
        private readonly Dictionary<int, SearchResultList> _searchResultLists;

        public NavigatorManager()
        {
            this._topLevelItems = new Dictionary<int, TopLevelItem>();
            this._searchResultLists = new Dictionary<int, SearchResultList>();
            this._staffPicks = new Dictionary<int, StaffPick>();
            this._topLevelItems.Add(1, new TopLevelItem(1, "official_view", "", ""));
            this._topLevelItems.Add(2, new TopLevelItem(2, "hotel_view", "", ""));
            //this._topLevelItems.Add(3, new TopLevelItem(3, "rooms_game", "", ""));
            this._topLevelItems.Add(4, new TopLevelItem(4, "myworld_view", "", ""));

            this._featuredRooms = new Dictionary<int, FeaturedRoom>();
        }

        public void Init()
        {
            if (this._searchResultLists.Count > 0)
                this._searchResultLists.Clear();

            if (this._featuredRooms.Count > 0)
                this._featuredRooms.Clear();

            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataTable Table = null;

                dbClient.SetQuery("SELECT * FROM `navigator_categories` ORDER BY `id` ASC");
                Table = dbClient.GetTable();

                if (Table != null)
                {
                    foreach (DataRow Row in Table.Rows)
                    {
                        if (Convert.ToInt32(Row["enabled"]) == 1)
                        {
                            if (!this._searchResultLists.ContainsKey(Convert.ToInt32(Row["id"])))
                                this._searchResultLists.Add(Convert.ToInt32(Row["id"]), new SearchResultList(Convert.ToInt32(Row["id"]), Convert.ToString(Row["category"]), Convert.ToString(Row["category_identifier"]), Convert.ToString(Row["public_name"]), true, -1, Convert.ToInt32(Row["required_rank"]), Convert.ToInt32(Row["minimized"]) == 1, NavigatorViewModeUtility.GetViewModeByString(Convert.ToString(Row["view_mode"])), Convert.ToString(Row["category_type"]), Convert.ToString(Row["search_allowance"]), Convert.ToInt32(Row["order_id"])));
                        }
                    }
                }

                dbClient.SetQuery("SELECT `room_id`,`image_url`,`enabled`, `langue` FROM `navigator_publics` ORDER BY `order_num` ASC");
                DataTable GetPublics = dbClient.GetTable();

                if (GetPublics != null)
                {
                    foreach (DataRow Row in GetPublics.Rows)
                    {
                        if (Convert.ToInt32(Row["enabled"]) == 1)
                        {
                            if (!this._featuredRooms.ContainsKey(Convert.ToInt32(Row["room_id"])))
                            {
                                this._featuredRooms.Add(Convert.ToInt32(Row["room_id"]), new FeaturedRoom(Convert.ToInt32(Row["room_id"]), Convert.ToString(Row["image_url"]), LanguageManager.ParseLanguage(Convert.ToString(Row["langue"]))));
                            }
                        }
                        dbClient.SetQuery("SELECT `room_id`,`image` FROM `navigator_staff_picks`");
                        DataTable table3 = dbClient.GetTable();
                        if (table3 != null)
                        {
                            foreach (DataRow row in (InternalDataCollectionBase)table3.Rows)
                            {
                                if (!this._staffPicks.ContainsKey(Convert.ToInt32(row["room_id"])))
                                    this._staffPicks.Add(Convert.ToInt32(row["room_id"]), new StaffPick(Convert.ToInt32(row["room_id"]), Convert.ToString(row["image"])));
                            }
                        }
                    }
                }
            }
            Console.WriteLine("Navegador -> Listo!");
        }

        public List<SearchResultList> GetCategorysForSearch(string Category)
        {
            IEnumerable<SearchResultList> Categorys =
                (from Cat in this._searchResultLists
                 where Cat.Value.Category == Category
                 orderby Cat.Value.OrderId ascending
                 select Cat.Value);
            return Categorys.ToList();
        }

        public ICollection<SearchResultList> GetResultByIdentifier(string Category)
        {
            IEnumerable<SearchResultList> Categorys =
                (from Cat in this._searchResultLists
                 where Cat.Value.CategoryIdentifier == Category
                 orderby Cat.Value.OrderId ascending
                 select Cat.Value);
            return Categorys.ToList();
        }

        public ICollection<SearchResultList> GetFlatCategories()
        {
            IEnumerable<SearchResultList> Categorys =
                (from Cat in this._searchResultLists
                 where Cat.Value.CategoryType == NavigatorCategoryType.CATEGORY
                 orderby Cat.Value.OrderId ascending
                 select Cat.Value);
            return Categorys.ToList();
        }

        public ICollection<SearchResultList> GetEventCategories()
        {
            IEnumerable<SearchResultList> Categorys =
                (from Cat in this._searchResultLists
                 where Cat.Value.CategoryType == NavigatorCategoryType.PROMOTION_CATEGORY
                 orderby Cat.Value.OrderId ascending
                 select Cat.Value);
            return Categorys.ToList();
        }

        public ICollection<TopLevelItem> GetTopLevelItems()
        {
            return this._topLevelItems.Values;
        }

        public ICollection<SearchResultList> GetSearchResultLists()
        {
            return this._searchResultLists.Values;
        }
        public ICollection<StaffPick> GetStaffPicks() => (ICollection<StaffPick>)this._staffPicks.Values;

        public bool TryGetTopLevelItem(int Id, out TopLevelItem TopLevelItem)
        {
            return this._topLevelItems.TryGetValue(Id, out TopLevelItem);
        }

        public bool TryGetSearchResultList(int Id, out SearchResultList SearchResultList)
        {
            return this._searchResultLists.TryGetValue(Id, out SearchResultList);
        }

        public bool TryGetFeaturedRoom(int RoomId, out FeaturedRoom PublicRoom)
        {
            return this._featuredRooms.TryGetValue(RoomId, out PublicRoom);
        }

        public bool TryGetStaffPickedRoom(int roomId, out StaffPick room) => this._staffPicks.TryGetValue(roomId, out room);

        public bool TryAddStaffPickedRoom(int roomId)
        {
            if (this._staffPicks.ContainsKey(roomId))
                return false;
            this._staffPicks.Add(roomId, new StaffPick(roomId, ""));
            return true;
        }

        public bool TryRemoveStaffPickedRoom(int roomId) => this._staffPicks.ContainsKey(roomId) && this._staffPicks.Remove(roomId);


        public ICollection<FeaturedRoom> GetFeaturedRooms(Language Langue)
        {
            return this._featuredRooms.Values;//.Where(F => F.Langue == Langue).ToList();
        }


    }
}
