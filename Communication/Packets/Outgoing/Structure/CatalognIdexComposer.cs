using Akiled.Core;
using Akiled.HabboHotel.Catalog;
using Akiled.HabboHotel.GameClients;
using System.Collections.Generic;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class CatalogIndexComposer : ServerPacket
    {
       public CatalogIndexComposer(GameClient Session, ICollection<CatalogPage> Pages, int Sub = 0)
            : base(ServerPacketHeader.GetCatalogIndexComposer)
        {
            WriteRootIndex(Session, Pages);

            foreach (CatalogPage Parent in Pages)
            {
                if (Parent.ParentId != -1 || Parent.MinimumRank > Session.GetHabbo().Rank)
                    continue;

                WritePage(Parent, CalcTreeSize(Session, Pages, Parent.Id), Session.Langue);

                foreach (CatalogPage child in Pages)
                {
                    if (child.ParentId != Parent.Id || child.MinimumRank > Session.GetHabbo().Rank)
                        continue;

                    if (child.Enabled)
                        WritePage(child, CalcTreeSize(Session, Pages, child.Id), Session.Langue);
                    else
                        WriteNodeIndex(child, CalcTreeSize(Session, Pages, child.Id), Session.Langue);

                    foreach (CatalogPage SubChild in Pages)
                    {
                        if (SubChild.ParentId != child.Id || SubChild.MinimumRank > Session.GetHabbo().Rank)
                            continue;

                        WritePage(SubChild, 0, Session.Langue);
                    }
                }
            }

            WriteBoolean(false);
            WriteString("NORMAL");
        }

        public void WriteRootIndex(GameClient session, ICollection<CatalogPage> pages)
        {
            WriteBoolean(true);
            WriteInteger(0);
            WriteInteger(-1);
            WriteString("root");
            WriteString(string.Empty);
            WriteInteger(0);
            WriteInteger(CalcTreeSize(session, pages, -1));
        }

        public void WriteNodeIndex(CatalogPage page, int treeSize, Language Langue)
        {
            WriteBoolean(true); // Visible
            WriteInteger(page.Icon);
            WriteInteger(-1);
            WriteString(page.PageLink);
            WriteString(page.GetCaptionByLangue(Langue));
            WriteInteger(0);
            WriteInteger(treeSize);
        }

        public void WritePage(CatalogPage page, int treeSize, Language Langue)
        {
            WriteBoolean(true);
            WriteInteger(page.Icon);
            WriteInteger(page.Id);
            WriteString(page.PageLink);
            WriteString(page.GetCaptionByLangue(Langue));

            WriteInteger(page.ItemOffers.Count);
            foreach (int i in page.ItemOffers.Keys)
            {
                WriteInteger(i);
            }

            WriteInteger(treeSize);
        }

        public int CalcTreeSize(GameClient Session, ICollection<CatalogPage> Pages, int ParentId)
        {
            int i = 0;
            foreach (CatalogPage Page in Pages)
            {
                if (Page.MinimumRank > Session.GetHabbo().Rank || Page.ParentId != ParentId)
                    continue;

                if (Page.ParentId == ParentId)
                    i++;
            }

            return i;
        }
    }
}