namespace Akiled.Communication.Packets.Outgoing.WebSocket
{
    class PlaySoundComposer : ServerPacket
    {
        public PlaySoundComposer(string Name, int Type, bool Loop = false)
            : base(21)
        {
            WriteString(Name);
            WriteInteger(Type);
            WriteBoolean(Loop);
        }
    }
}
