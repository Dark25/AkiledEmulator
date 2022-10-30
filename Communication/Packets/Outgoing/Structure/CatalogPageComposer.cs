using Akiled.Core;
using Akiled.HabboHotel.Catalog;
using Akiled.HabboHotel.Catalog.Utilities;
using Akiled.HabboHotel.Items;
using System.Linq;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    public class CatalogPageComposer : ServerPacket
    {
        public CatalogPageComposer(CatalogPage Page, string CataMode, Language Langue)
            : base(ServerPacketHeader.CatalogPageMessageComposer)
        {
            WriteInteger(Page.Id);
            WriteString(CataMode);
            WriteString(Page.Template);

            WriteInteger(Page.PageStrings1.Count);
            foreach (string s in Page.PageStrings1)
            {
                WriteString(s);
            }

            if (Page.GetPageStrings2ByLangue(Langue).Count == 1 && (Page.Template == "default_3x3" || Page.Template == "default_3x3_color_grouping") && string.IsNullOrEmpty(Page.GetPageStrings2ByLangue(Langue)[0]))
            {
                WriteInteger(1);
                WriteString(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("catalog.desc.default", Langue), Page.GetCaptionByLangue(Langue)));
            }
            else
            {
                WriteInteger(Page.GetPageStrings2ByLangue(Langue).Count);
                foreach (string s in Page.GetPageStrings2ByLangue(Langue))
                {
                    WriteString(s);
                }
            }

            if (!Page.Template.Equals("frontpage") && !Page.Template.Equals("club_buy"))
            {
                WriteInteger(Page.Items.Count);
                foreach (CatalogItem Item in Page.Items.Values)
                {
                    WriteInteger(Item.Id);
                    WriteString(Item.Name);
                    WriteBoolean(false);//IsRentable
                    WriteInteger(Item.CostCredits);

                    if (Item.CostDiamonds > 0)
                    {
                        WriteInteger(Item.CostDiamonds);
                        WriteInteger(105); // Diamonds
                    }
                    else
                    {
                        WriteInteger(Item.CostDuckets);
                        WriteInteger(0);
                    }

                    WriteBoolean(ItemUtility.CanGiftItem(Item));

                    WriteInteger(string.IsNullOrEmpty(Item.Badge) ? 1 : 2);

                    if (!string.IsNullOrEmpty(Item.Badge))
                    {
                        WriteString("b");
                        WriteString(Item.Badge);
                    }
                    base.WriteString(Item.Data.Type.ToString());
                    if (Item.Data.Type.ToString().ToLower() == "b")
                    {
                        //This is just a badge, append the name.
                        base.WriteString(Item.Data.ItemName);
                    }
                    else
                    {
                        WriteInteger(Item.Data.SpriteId);
                        if (Item.Data.InteractionType == InteractionType.WALLPAPER || Item.Data.InteractionType == InteractionType.FLOOR || Item.Data.InteractionType == InteractionType.LANDSCAPE)
                        {
                            WriteString(Item.Name.Split('_')[2]);
                        }
                        else if (Item.Data.InteractionType == InteractionType.bot)//Bots
                        {
                            CatalogBot CatalogBot = null;
                            if (!AkiledEnvironment.GetGame().GetCatalog().TryGetBot(Item.ItemId, out CatalogBot))
                                WriteString("hd-180-7.ea-1406-62.ch-210-1321.hr-831-49.ca-1813-62.sh-295-1321.lg-285-92");
                            else
                                WriteString(CatalogBot.Figure);
                        }
                        else
                        {
                            WriteString("");
                        }
                        WriteInteger(Item.Amount);
                        WriteBoolean(Item.IsLimited); // IsLimited
                        if (Item.IsLimited)
                        {
                            WriteInteger(Item.LimitedEditionStack);
                            WriteInteger(Item.LimitedEditionStack - Item.LimitedEditionSells);
                        }
                    }
                    WriteInteger(0); //club_level
                    WriteBoolean(ItemUtility.CanSelectAmount(Item));

                    WriteBoolean(false);// TODO: Figure out
                    WriteString("");//previewImage -> e.g; catalogue/pet_lion.png
                }
            }
            else
                WriteInteger(0);
            WriteInteger(-1);
            WriteBoolean(false);

            WriteInteger(AkiledEnvironment.GetGame().GetCatalog().GetPromotions().ToList().Count);//Count
            foreach (CatalogPromotion Promotion in AkiledEnvironment.GetGame().GetCatalog().GetPromotions().ToList())
            {
                WriteInteger(Promotion.Id);
                WriteString(Promotion.GetTitleByLangue(Langue));
                WriteString(Promotion.Image);
                WriteInteger(Promotion.Unknown);
                WriteString(Promotion.PageLink);
                WriteInteger(Promotion.ParentId);
            }
        }
    }
}