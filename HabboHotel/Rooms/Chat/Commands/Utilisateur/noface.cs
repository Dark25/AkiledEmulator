using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;using System;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd{    class noface : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {            if (UserRoom.transformation || UserRoom.IsSpectator)                return;            string look = Session.GetHabbo().Look;            if (look.Contains("hd-"))            {                string hdlook = look.Split(new string[] { "hd-" }, StringSplitOptions.None)[1];                string hdcode = "hd-" + hdlook.Split(new char[] { '.' })[0]; //ex : hd-180-22
                string hdcodecolor = "";                if (hdcode.Split('-').Length == 3)                    hdcodecolor = hdcode.Split('-')[2];                string hdcodenoface = "hd-99999-" + hdcodecolor; //hd-9999-22

                look = look.Replace(hdcode, hdcodenoface);                Session.GetHabbo().Look = look;                if (!Session.GetHabbo().InRoom)                    return;                Room currentRoom = Session.GetHabbo().CurrentRoom;                if (currentRoom == null)                    return;

                currentRoom.SendPacket(new UserChangeComposer(UserRoom, false));            }        }    }}