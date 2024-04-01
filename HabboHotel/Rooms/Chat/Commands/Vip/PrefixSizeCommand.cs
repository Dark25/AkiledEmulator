
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
                Session.SendWhisper(AkiledEnvironment.GetLanguageManager().TryGetValue("PrefixSizeCommand.name_size", Session.Langue));   
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
                        Session.SendWhisper(AkiledEnvironment.GetLanguageManager().TryGetValue("PrefixSizeCommand.name", Session.Langue));
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
                                Session.SendWhisper(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("PrefixSizeCommand.name.1", Session.Langue), Convert.ToString(result)));
                            }
                            else
                            {
                                Session.GetHabbo().PrefixSize = Convert.ToString(result) + ";";
                                Session.SendWhisper(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("PrefixSizeCommand.name.1", Session.Langue), Convert.ToString(result)));   
                            }
                        }
                        else
                        Session.SendWhisper(AkiledEnvironment.GetLanguageManager().TryGetValue("PrefixSizeCommand.name_size.1", Session.Langue));
                    }
                }
                else
                    Session.SendWhisper(AkiledEnvironment.GetLanguageManager().TryGetValue("PrefixSizeCommand.name_size.1", Session.Langue));
            }
        }
    }
}
