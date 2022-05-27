using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.HabboHotel.GameClients;
using System;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class YoutubeRoom : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 2)
                return;

            string Url = Params[1];

            if (string.IsNullOrEmpty(Url) || (!Url.Contains("?v=") && !Url.Contains("youtu.be/") && !Url.Contains("youtube.com/"))) //https://youtu.be/_mNig3ZxYbM
            {
                return;
            }

            string Split = "";

            if (Url.Contains("?v="))
            {
                Split = Url.Split(new string[] { "?v=" }, StringSplitOptions.None)[1];
            }
            else if (Url.Contains("youtu.be/"))
            {
                Split = Url.Split(new string[] { "youtu.be/" }, StringSplitOptions.None)[1];
            }
            else if (Url.Contains("youtube.com/"))
            {
                Split = Url.Split(new string[] { "youtube.com/" }, StringSplitOptions.None)[1];
            }


            if (Split.Length < 11)
            {
                return;
            }
            string VideoId = Split.Substring(0, 11);

            Room.SendPacketWeb(new YoutubeTvComposer(0, VideoId));
        }
    }
}