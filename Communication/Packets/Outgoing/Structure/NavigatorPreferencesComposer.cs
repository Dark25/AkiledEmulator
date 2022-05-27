namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class NavigatorPreferencesComposer : ServerPacket
    {
        public NavigatorPreferencesComposer()
            : base(ServerPacketHeader.NavigatorPreferencesMessageComposer)
        {
            string searchnavigator = (AkiledEnvironment.GetConfig().data["search_navigator_open"]);
            WriteInteger(68);//X
            WriteInteger(42);//Y
            WriteInteger(425);//Width
            WriteInteger(592);//Height
            if (searchnavigator == "true")
                WriteBoolean(true);//Show or hide saved searches.
            else
                WriteBoolean(false);//Show or hide saved searches.
            WriteInteger(0);//No idea?
        }
    }
}
