using Akiled.Core;

namespace Akiled.HabboHotel.Catalog
{
    public class CatalogPromotion
    {
        public int Id;
        public string Title;
        public string TitleEn;
        public string TitleBr;
        public string Image;
        public int Unknown;
        public string PageLink;
        public int ParentId;

        public CatalogPromotion(int id, string title, string titleEn, string titleBr, string image, int unknown, string pageLink, int parentId)
        {
            this.Id = id;
            this.Title = title;
            this.TitleEn = titleEn;
            this.TitleBr = titleBr;
            this.Image = image;
            this.Unknown = unknown;
            this.PageLink = pageLink;
            this.ParentId = parentId;
        }

        public string GetTitleByLangue(Language Langue)
        {
            if (Langue == Language.ANGLAIS)
                return TitleEn;
            else if (Langue == Language.PORTUGAIS)
                return TitleBr;

            return Title;
        }
    }
}
