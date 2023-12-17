namespace Akiled.Communication.Packets.Incoming.LandingView;
using Akiled.Communication.Packets;
using Akiled.Communication.Packets.Incoming;
using Akiled.Communication.Packets.Outgoing.LandingView;
using Akiled.HabboHotel.GameClients;
using System;

internal sealed class GetCommunityGoalHallOfFameEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var hof = AkiledEnvironment.GetGame().GetHallOFFame();

        session.SendPacket(new CommunityGoalHallOfFameComposer(hof.UserRanking));
      
    }
}