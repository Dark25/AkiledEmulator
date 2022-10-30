using Akiled;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using System;

namespace Akin.HabboHotel.Misc
{
    class Diamantes
    {
        internal static void GiveCycleDiamonds(GameClient Session)
        {
            /* if (EmuSettings.DIAMONDS_ENABLED == false)
                 return;*/
            string cantdiamonds = (AkiledEnvironment.GetConfig().data["cantdiamonds"]);

            if ((AkiledEnvironment.GetUnixTimestamp() - Session.GetHabbo().DiamondsCycleUpdate) > 1 * 6)
            {
                int DiamondsAmount = Convert.ToInt32(cantdiamonds);

                Session.GetHabbo().DiamondsCycleUpdate = AkiledEnvironment.GetUnixTimestamp();
                Session.SendWhisper("Recibiste 60 por estar conectado.");
                Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, 50));


                Session.GetHabbo().AkiledPoints += 10;
                Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().AkiledPoints, 10, 105));

                //Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().AkiledPoints,DiamondsAmount));
            }
        }
    }
}
