
using Akiled.HabboHotel.GameClients;
using System;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class PrefixSizeCommand : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Oops, debes escribir un numero de 1-20!");
            }
            else
            {
                int result;
                if (int.TryParse(Params[1], out result))
                {
                    if (string.IsNullOrEmpty(Session.GetHabbo().Prefixnamecolor))
                        Session.GetHabbo().Prefixnamecolor = "000000;";
                    if (result == 12)
                    {
                        Session.GetHabbo().PrefixSize = "12;12";
                        Session.SendWhisper("Tu tamaño de prefijo, Ha vuelto a la normalidad");
                    }
                    else
                    {
                        bool flag = true;
                        if (result < 1)
                            flag = false;
                        if (result > 20 && Session.GetHabbo().Rank < 6)
                            flag = false;
                        if (flag)
                        {
                            string str = Session.GetHabbo().PrefixSize.Split(';')[1];
                            if (!string.IsNullOrEmpty(str))
                            {
                                Session.GetHabbo().PrefixSize = result.ToString() + ";" + str;
                                Session.SendWhisper("El tamaño ha sido cambiado a " + Convert.ToString(result));
                            }
                            else
                            {
                                Session.GetHabbo().PrefixSize = Convert.ToString(result) + ";";
                                Session.SendWhisper("El tamaño ha sido cambiado a " + Convert.ToString(result));
                            }
                        }
                        else
                            Session.SendWhisper("Tamaño invalido, debe ser numero de 1-20.");
                    }
                }
                else
                    Session.SendWhisper("Tamaño invalido, debe ser numero de 1-20.");
            }
        }
    }
}
