using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Navigators;
using System.Collections.Generic;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class NavigatorSearchEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string Category = Packet.PopString();
            string Search = Packet.PopString();

            ICollection<SearchResultList> Categories = new List<SearchResultList>();

            if (!string.IsNullOrEmpty(Search))
            {
                SearchResultList QueryResult = null;
                if (AkiledEnvironment.GetGame().GetNavigator().TryGetSearchResultList(0, out QueryResult))
                {
                    Categories.Add(QueryResult);
                }
            }
            else
            {
                Categories = AkiledEnvironment.GetGame().GetNavigator().GetCategorysForSearch(Category);
                if (Categories.Count == 0)
                {
                    //Are we going in deep?!
                    Categories = AkiledEnvironment.GetGame().GetNavigator().GetResultByIdentifier(Category);
                    if (Categories.Count > 0)
                    {
                        Session.SendPacket(new NavigatorSearchResultSetComposer(Category, Search, Categories, Session, 2, 50));
                        return;
                    }
                }
            }

            Session.SendPacket(new NavigatorSearchResultSetComposer(Category, Search, Categories, Session));
        }
    }
}
