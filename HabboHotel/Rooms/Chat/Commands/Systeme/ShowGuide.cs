using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Guides;
using System.Collections.Generic;
using System.Text;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class ShowGuide : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            GuideManager guideManager = AkiledEnvironment.GetGame().GetGuideManager();
            if (guideManager.GuidesCount <= 0)
            {
                Session.SendHugeNotif("Ningun guía usa la herramienta Alfa");
                Session.SendHugeNotif(AkiledEnvironment.GetLanguageManager().TryGetValue("cmd.showguide", Session.Langue));
            }
            else
            {
                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.Append(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("cmd.showguide.1", Session.Langue)) + "\r\r");
                foreach (KeyValuePair<int, bool> entry in guideManager.GuidesOnDuty)
                {
                    GameClient guide = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(entry.Key);
                    if (guide == null)
                        continue;

                    if (entry.Value)
                        stringBuilder.Append(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("cmd.showguide.2", Session.Langue), guide.GetHabbo().Username)); 
                    else
                        stringBuilder.Append("- " + guide.GetHabbo().Username + " (Disponible)\r");
                        stringBuilder.Append(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("cmd.showguide.3", Session.Langue), guide.GetHabbo().Username));
                }
                stringBuilder.Append("\r");
                Session.SendHugeNotif(stringBuilder.ToString());
            }

        }
    }
}
