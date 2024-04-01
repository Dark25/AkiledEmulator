using Akiled.HabboHotel.GameClients;
using System;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class NameSizeCommand : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length == 1)
            {
                
                Session.SendWhisper(AkiledEnvironment.GetLanguageManager().TryGetValue("NameSizeCommand.name_size", Session.Langue));
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
                        Session.SendWhisper(AkiledEnvironment.GetLanguageManager().TryGetValue("NameSizeCommand.name", Session.Langue));
                    }
                    else
                    {
                        bool flag = !(result < 1);
                        if (result > 20 && Session.GetHabbo().Rank < 6)
                            flag = false;
                        if (flag)
                        {
                            string str = Session.GetHabbo().PrefixSize.Split(';')[0];
                            Session.GetHabbo().PrefixSize = string.IsNullOrEmpty(str) ? ";" + result.ToString() : str + ";" + Convert.ToString(result);
                            Session.SendWhisper("    " + Convert.ToString(result));
                            Session.SendWhisper(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("NameSizeCommand.name.1", Session.Langue),Convert.ToString(result)));
                        }
                        else
                            Session.SendWhisper(AkiledEnvironment.GetLanguageManager().TryGetValue("NameSizeCommand.name_size.1", Session.Langue));
                    }
                }
                else
                    Session.SendWhisper("Tamaño invalido, debe ser numero de 1-20.");
                    Session.SendWhisper(AkiledEnvironment.GetLanguageManager().TryGetValue("NameSizeCommand.name_size.2", Session.Langue));
            }
        }
    }
}
