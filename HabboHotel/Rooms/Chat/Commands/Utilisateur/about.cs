using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using System;
using System.Text;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class About : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            TimeSpan Uptime = DateTime.Now - AkiledEnvironment.ServerStarted;

            int OnlineUsers = AkiledEnvironment.GetGame().GetClientManager().Count;
            string about_image = (AkiledEnvironment.GetConfig().data["about_image"]);
            string name_hotel = (AkiledEnvironment.GetConfig().data["namehotel_text"]);
            int OnlineWeb = AkiledEnvironment.GetGame().GetClientWebManager().Count;
            int RoomCount = AkiledEnvironment.GetGame().GetRoomManager().Count;



            Session.SendMessage(new RoomNotificationComposer(Encoding.GetEncoding("Windows-1252").GetString(Encoding.GetEncoding("UTF-8").GetBytes("Akiled Emulator for " + name_hotel + "")),
                        Encoding.GetEncoding("Windows-1252").GetString(Encoding.GetEncoding("UTF-8").GetBytes("<b><font color='#9200cc'>AkiledEmu</font></b>\n<font color='#363636'>AkiledEmu es un proyecto de varios fanaticos de habbo para tener todas las Funciones de Habbo.\n\n</font><font color='#9200cc'><b>Creditos para</b>:</font>\n" +
                        "Carlos Mota (Program)\n" +
                        "<b><font color='#9200cc'>Información:</font></b>\n" +
                        "<font color='#363636'><b>Usuarios:</b> " + OnlineUsers + "\n" +
                        "<b>Salas:</b> " + RoomCount + "\n" +
                        "<b>Tiempo:</b> " + Uptime.Days + " day(s), " + Uptime.Hours + " hours and " + Uptime.Minutes + " minutes.</font>\n\n")), about_image, ""));

        }
    }
}