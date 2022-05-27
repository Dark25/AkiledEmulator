namespace Akiled.Communication.Packets.Outgoing.WebSocket
{
    class YoutubeTvComposer : ServerPacket
    {
        public YoutubeTvComposer(int ItemId, string VideoId)
            : base(3)
        {
            WriteInteger(ItemId);
            WriteString(VideoId);
        }
    }
}
