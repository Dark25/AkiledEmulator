using Akiled.HabboHotel.Navigators;
using System.Collections.Generic;
using System.Linq;

namespace Akiled.Communication.Packets.Outgoing.Navigator
{
    internal class NavigatorFlatCatsComposer : ServerPacket
    {
        public ICollection<SearchResultList> Categories { get; }

        public NavigatorFlatCatsComposer(ICollection<SearchResultList> categories)
            : base(ServerPacketHeader.NavigatorFlatCatsMessageComposer)
        {
            Categories = categories;

            WriteInteger(Categories.Count);
            foreach (SearchResultList category in Categories.ToList())
            {
                WriteInteger(category.Id);
                WriteString(category.PublicName);
                WriteBoolean(true); // TODO
            }
        }


    }
}