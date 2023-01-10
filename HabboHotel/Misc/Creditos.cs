using Akiled;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using System;

namespace Akin.HabboHotel.Misc
{
    class Creditos
    {
        internal static void GiveCycleDiamonds(GameClient Session)
        {
            /* if (EmuSettings.DIAMONDS_ENABLED == false)
                 return;*/
            string cantdiamonds = (AkiledEnvironment.GetConfig().data["cantdiamonds"]);


            if ((AkiledEnvironment.GetUnixTimestamp() - Session.GetHabbo().DiamondsCycleUpdate) > 1 * 60)
            {
                int DiamondsAmount = Convert.ToInt32(cantdiamonds);

                Session.GetHabbo().DiamondsCycleUpdate = AkiledEnvironment.GetUnixTimestamp();
                Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().AkiledPoints, DiamondsAmount));
            }
        }
    }
}
