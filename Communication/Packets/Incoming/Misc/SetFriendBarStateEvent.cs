namespace Akiled.Communication.Packets.Incoming.Structure
{
    class SetFriendBarStateEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            //Session.GetHabbo().FriendbarState = Packet.PopInt() == 1;
            //Session.SendPacket(new SoundSettingsComposer(Session.GetHabbo()._clientVolume, Session.GetHabbo().ChatPreference, Session.GetHabbo().AllowMessengerInvites, Session.GetHabbo().FocusPreference, FriendBarStateUtility.GetInt(Session.GetHabbo().FriendbarState)));
        }
    }
}
