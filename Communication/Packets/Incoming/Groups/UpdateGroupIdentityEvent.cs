using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;using Akiled.HabboHotel.GameClients;using Akiled.HabboHotel.Groups;namespace Akiled.Communication.Packets.Incoming.Structure{    class UpdateGroupIdentityEvent : IPacketEvent    {        public void Parse(GameClient Session, ClientPacket Packet)        {
            int GroupId = Packet.PopInt();
            string Name = AkiledEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Packet.PopString());
            string Desc = AkiledEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Packet.PopString());

            Group Group = null;
            if (!AkiledEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group))
                return;

            if (Group.CreatorId != Session.GetHabbo().Id)
                return;

            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `groups` SET `name`= @name, `desc` = @desc WHERE `id` = @groupId LIMIT 1");
                dbClient.AddParameter("name", Name);
                dbClient.AddParameter("desc", Desc);
                dbClient.AddParameter("groupId", GroupId);
                dbClient.RunQuery();
            }

            Group.Name = Name;
            Group.Description = Desc;

            Session.SendPacket(new GroupInfoComposer(Group, Session));        }    }}