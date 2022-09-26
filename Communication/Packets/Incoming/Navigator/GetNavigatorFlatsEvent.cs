using System.Collections.Generic;
using Akiled.Communication.Packets.Outgoing.Navigator;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Navigators;

namespace Akiled.Communication.Packets.Incoming.Navigator
{
    internal class GetNavigatorFlatsEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            ICollection<SearchResultList> categories = AkiledEnvironment.GetGame().GetNavigator().GetEventCategories();

            session.SendPacket(new NavigatorFlatCatsComposer(categories));
        }
    }
}