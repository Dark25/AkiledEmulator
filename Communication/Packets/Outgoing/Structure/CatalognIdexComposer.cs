using Akiled.Core;
using Akiled.HabboHotel.Catalog;
using Akiled.HabboHotel.GameClients;
using System.Collections.Generic;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    public class CatalogIndexComposer : ServerPacket
    {
        public CatalogIndexComposer(GameClient session, ICollection<CatalogPage> pages)
            : base(ServerPacketHeader.GetCatalogIndexComposer)
        {
            base.WriteBoolean(true);
            base.WriteInteger(0);
            base.WriteInteger(-1);
            base.WriteString("root");
            base.WriteString("");
            base.WriteInteger(0);

            base.WriteInteger(pages.Count);
            foreach (CatalogPage page in pages)
            {
                Append(session, page, session.Langue);
            }
            base.WriteBoolean(false);
            base.WriteString("NORMAL");
        }

        public void Append(GameClient session, CatalogPage page, Language Langue)
        {
            ICollection<CatalogPage> pages = AkiledEnvironment.GetGame().GetCatalog().GetPages(session, page.Id);

            base.WriteBoolean(true);
            base.WriteInteger(page.Icon);
            base.WriteInteger(page.Enabled ? page.Id : -1);
            base.WriteString(page.PageLink);
            base.WriteString(page.GetCaptionByLangue(Langue));

            base.WriteInteger(page.ItemOffers.Count);
            foreach (int key in page.ItemOffers.Keys)
            {
                base.WriteInteger(key);
            }

            base.WriteInteger(pages.Count);
            foreach (CatalogPage nextPage in pages)
            {
                Append(session, nextPage, session.Langue);
            }
        }
    }
}