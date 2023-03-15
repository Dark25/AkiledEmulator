using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Catalog.Clothing;
using Akiled.HabboHotel.Catalog.Marketplace;
using Akiled.HabboHotel.Catalog.Pets;
using Akiled.HabboHotel.Catalog.Vouchers;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using System;
using System.Collections.Generic;
using System.Data;

namespace Akiled.HabboHotel.Catalog
{
    public class CatalogManager
    {

        private readonly MarketplaceManager _marketplace;
        private readonly PetRaceManager _petRaceManager;
        private readonly VoucherManager _voucherManager;
        private ClothingManager _clothingManager;

        private Dictionary<int, int> _itemOffers;
        private readonly Dictionary<int, CatalogPage> _pages;
        private readonly Dictionary<int, CatalogBot> _botPresets;
        private readonly Dictionary<int, Dictionary<int, CatalogItem>> _items;
        private readonly Dictionary<int, CatalogPromotion> _promotions;
        private readonly Dictionary<int, int> _itemsPage;
        private readonly List<string> _badges;

        public CatalogManager()
        {
            this._marketplace = new MarketplaceManager();
            this._petRaceManager = new PetRaceManager();

            this._voucherManager = new VoucherManager();
            this._voucherManager.Init();
            this._clothingManager = new ClothingManager();
            this._clothingManager.Init();
            this._itemOffers = new Dictionary<int, int>();
            this._pages = new Dictionary<int, CatalogPage>();
            this._botPresets = new Dictionary<int, CatalogBot>();
            this._items = new Dictionary<int, Dictionary<int, CatalogItem>>();
            this._promotions = new Dictionary<int, CatalogPromotion>();
            this._itemsPage = new Dictionary<int, int>();
            this._badges = new List<string>();
        }

        public void Init(ItemDataManager ItemDataManager)
        {
            if (this._pages.Count > 0) this._pages.Clear();
            if (this._botPresets.Count > 0) this._botPresets.Clear();
            if (this._items.Count > 0) this._items.Clear();
            if (this._promotions.Count > 0) this._promotions.Clear();
            if (this._itemsPage.Count > 0) this._itemsPage.Clear();
            if (this._badges.Count > 0) this._badges.Clear();

            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`item_id`,`catalog_name`,`cost_credits`,`cost_pixels`,`cost_diamonds`,`amount`,`page_id`,`limited_sells`,`limited_stack`,`offer_active`,`badge`,`offer_id` FROM `catalog_items` ORDER by ID DESC");
                DataTable CatalogueItems = dbClient.GetTable();

                if (CatalogueItems != null)
                {
                    foreach (DataRow Row in CatalogueItems.Rows)
                    {
                        if (Convert.ToInt32(Row["amount"]) <= 0)
                            continue;

                        int ItemId = Convert.ToInt32(Row["id"]);
                        int PageId = Convert.ToInt32(Row["page_id"]);
                        int BaseId = Convert.ToInt32(Row["item_id"]);
                        int OfferId = Convert.ToInt32(Row["offer_id"]);

                        ItemData Data = null;
                        if (!ItemDataManager.GetItem(BaseId, out Data))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Atención: El item: " + ItemId + " esta en el catalogo, pero no se encuentra en furniture.");
                            continue;
                        }

                        if (!this._badges.Contains((string)Row["badge"]))
                            this._badges.Add((string)Row["badge"]);

                        if (!this._items.ContainsKey(PageId))
                            this._items[PageId] = new Dictionary<int, CatalogItem>();
                        if (OfferId != -1 && !this._itemOffers.ContainsKey(OfferId))
                            this._itemOffers.Add(OfferId, PageId);

                        this._items[PageId].Add(Convert.ToInt32(Row["id"]), new(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["item_id"]),
                            Data, Convert.ToString(Row["catalog_name"]), Convert.ToInt32(Row["page_id"]), Convert.ToInt32(Row["cost_credits"]), Convert.ToInt32(Row["cost_pixels"]), Convert.ToInt32(Row["cost_diamonds"]),
                            Convert.ToInt32(Row["amount"]), Convert.ToInt32(Row["limited_sells"]), Convert.ToInt32(Row["limited_stack"]), AkiledEnvironment.EnumToBool(Row["offer_active"].ToString()), Convert.ToString(Row["badge"]), Convert.ToInt32(Row["offer_id"])));

                        this._itemsPage.Add(Convert.ToInt32(Row["id"]), PageId);
                    }
                }


                dbClient.SetQuery("SELECT catalog_pages.id, catalog_pages.parent_id, catalog_pages.caption, catalog_pages.page_link, catalog_pages.enabled, catalog_pages.min_rank, catalog_pages.icon_image," +
                                    " catalog_pages.page_layout, catalog_pages.page_strings_1, catalog_pages.page_strings_2, catalog_pages_langue.caption_en, catalog_pages_langue.caption_br," +
                                    " catalog_pages_langue.page_strings_2_en, catalog_pages_langue.page_strings_2_br" +
                                    " FROM catalog_pages" +
                                    " LEFT JOIN catalog_pages_langue ON catalog_pages.id = catalog_pages_langue.page_id" +
                                    " ORDER BY order_num;");
                DataTable CatalogPages = dbClient.GetTable();

                if (CatalogPages != null)
                {
                    foreach (DataRow Row in CatalogPages.Rows)
                    {
                        this._pages.Add(Convert.ToInt32(Row["id"]), new CatalogPage(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["parent_id"]), Row["enabled"].ToString(), Convert.ToString(Row["caption"]),
                            Convert.ToString(Row["page_link"]), Convert.ToInt32(Row["icon_image"]), Convert.ToInt32(Row["min_rank"]), Convert.ToString(Row["page_layout"]),
                            Convert.ToString(Row["page_strings_1"]), Convert.ToString(Row["page_strings_2"]), Convert.ToString(Row["caption_en"]),
                            Convert.ToString(Row["caption_br"]), Convert.ToString(Row["page_strings_2_en"]), Convert.ToString(Row["page_strings_2_br"]),
                            this._items.ContainsKey(Convert.ToInt32(Row["id"])) ? this._items[Convert.ToInt32(Row["id"])] : new Dictionary<int, CatalogItem>(),
                            ref this._itemOffers));
                    }
                }

                dbClient.SetQuery("SELECT `id`,`name`,`figure`,`motto`,`gender`,`ai_type` FROM `catalog_bot_presets`");
                DataTable bots = dbClient.GetTable();

                if (bots != null)
                {
                    foreach (DataRow Row in bots.Rows)
                    {
                        this._botPresets.Add(Convert.ToInt32(Row[0]), new CatalogBot(Convert.ToInt32(Row[0]), Convert.ToString(Row[1]), Convert.ToString(Row[2]), Convert.ToString(Row[3]), Convert.ToString(Row[4]), Convert.ToString(Row[5])));
                    }
                }

                dbClient.SetQuery("SELECT * FROM `catalog_promotions`");
                DataTable GetPromotions = dbClient.GetTable();

                if (GetPromotions != null)
                {
                    foreach (DataRow Row in GetPromotions.Rows)
                    {
                        if (!this._promotions.ContainsKey(Convert.ToInt32(Row["id"])))
                            this._promotions.Add(Convert.ToInt32(Row["id"]), new CatalogPromotion(Convert.ToInt32(Row["id"]), Convert.ToString(Row["title"]), Convert.ToString(Row["title_en"]), Convert.ToString(Row["title_br"]), Convert.ToString(Row["image"]), Convert.ToInt32(Row["unknown"]), Convert.ToString(Row["page_link"]), Convert.ToInt32(Row["parent_id"])));
                    }
                }

                this._petRaceManager.Init();
                this._clothingManager.Init();
            }
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Catalogo del Hotel -> Listo!");
        }

        public bool HasBadge(string Code) => this._badges.Contains(Code);

        public CatalogItem FindItem(int ItemId, int Rank)
        {
            if (!this._itemsPage.ContainsKey(ItemId))
                return null;

            int PageId = this._itemsPage[ItemId];
            if (!this._pages.ContainsKey(PageId))
                return null;

            CatalogPage page = this._pages[PageId];
            if (page == null || !page.Enabled || page.MinimumRank > Rank)
                return null;

            if (page.Items.ContainsKey(ItemId))
                return (CatalogItem)page.Items[ItemId];
            return null;
        }

        public bool TryGetBot(int ItemId, out CatalogBot Bot)
        {
            return this._botPresets.TryGetValue(ItemId, out Bot);
        }

        public Dictionary<int, int> ItemOffers
        {
            get { return this._itemOffers; }
        }

        public bool TryGetPage(int pageId, out CatalogPage page)
        {
            return this._pages.TryGetValue(pageId, out page);
        }

        public ICollection<CatalogPage> GetPages(GameClient session, int pageId)
        {
            List<CatalogPage> pages = new List<CatalogPage>();
            foreach (CatalogPage page in this._pages.Values)
            {
                if (page.ParentId != pageId || page.MinimumRank > session.GetHabbo().Rank)
                {
                    continue;
                }
                pages.Add(page);
            }
            return pages;
        }

        public ICollection<CatalogPromotion> GetPromotions()
        {
            return this._promotions.Values;
        }

        public MarketplaceManager GetMarketplace()
        {
            return this._marketplace;
        }

        public PetRaceManager GetPetRaceManager()
        {
            return this._petRaceManager;
        }

        public VoucherManager GetVoucherManager()
        {
            return this._voucherManager;
        }
        public ClothingManager GetClothingManager()
        {
            return this._clothingManager;
        }
    }
}