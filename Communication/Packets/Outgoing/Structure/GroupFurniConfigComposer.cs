using Akiled.HabboHotel.Groups;
using System.Collections.Generic;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class GroupFurniConfigComposer : ServerPacket
    {
        public GroupFurniConfigComposer(ICollection<Group> groups)
            : base(ServerPacketHeader.GroupFurniConfigMessageComposer)
        {
            WriteInteger(groups.Count);
            foreach (Group group in groups)
            {
                WriteInteger(group.Id);
                WriteString(group.Name);
                WriteString(group.Badge);
                WriteString(AkiledEnvironment.GetGame().GetGroupManager().GetColourCode(group.Colour1, true));
                WriteString(AkiledEnvironment.GetGame().GetGroupManager().GetColourCode(group.Colour2, false));
                WriteBoolean(false);
                WriteInteger(group.CreatorId);
                WriteBoolean(group.ForumEnabled);
            }
        }
    }
}
