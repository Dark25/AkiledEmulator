using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Users;
using Akiled.HabboHotel.WebClients;
using System;
using System.Data;
using System.Text;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class userinfoforid : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("id invalido, debe ser numero entero y real, nada de id falsas.");
                return;
            }
            String UserID = (Params[1]);

            if (UserID.Equals(0))
            {
                //Session.SendMessage(new NuxAlertMessageComposer("habbopages/chat/emoji.txt"));
            }
            else
            {
                int idNum;
                bool isNumeric = int.TryParse(UserID, out idNum);
                if (isNumeric)
                {
                    switch (idNum)
                    {
                        default:
                            bool isValid = true;
                            if (idNum < 1)
                            {
                                isValid = false;
                            }

                            if (idNum > 999999999)
                            {
                                isValid = false;
                            }
                            if (isValid)
                            {
                                DataRow Userdata = null;
                                string Username = "";
                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT username FROM users WHERE id = " + UserID + " LIMIT 1");
                                    Userdata = dbClient.GetRow();
                                    Username = Convert.ToString(Userdata["username"]);
                                }
                                Session.SendWhisper("El nombre de usuario del ID es: " + Username + ")");
                            }
                            else
                            {
                                Session.SendWhisper("id invalido, debe ser numero entero y real, nada de id falsas.");
                            }
                            break;
                    }
                }
                else
                {
                    Session.SendWhisper("id invalido, debe ser numero entero y real, nada de id falsas.");
                }
            }
            return;
        }
    }
}