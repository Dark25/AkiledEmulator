﻿using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class PlaySoundRoom : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
                return;

            string SongName = Params[1];

            Room.SendPacketWeb(new PlaySoundComposer(SongName, 1)); //Type = Trax
        }
    }
}
