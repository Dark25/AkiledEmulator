using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using System;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class info : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            TimeSpan Uptime = DateTime.Now - AkiledEnvironment.ServerStarted;

            int OnlineUsers = AkiledEnvironment.GetGame().GetClientManager().Count;
            string about_image = (AkiledEnvironment.GetConfig().data["about_image"]);
            string name_hotel = (AkiledEnvironment.GetConfig().data["namehotel_text"]);
            int OnlineWeb = AkiledEnvironment.GetGame().GetClientWebManager().Count;
            int RoomCount = AkiledEnvironment.GetGame().GetRoomManager().Count;


            Session.SendMessage(new RoomNotificationComposer(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("About.1", Session.Langue), name_hotel), string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("About.2", Session.Langue), OnlineUsers, RoomCount, Uptime.Days, Uptime.Hours, Uptime.Minutes), about_image, ""));
        }
    }
}