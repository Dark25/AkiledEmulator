namespace Akiled.Communication.Packets.Outgoing.LandingView;

using Akiled.Communication.Packets.Outgoing;
using System.Collections.Generic;
using Akiled.HabboHotel.Users;
using System;

internal sealed class CommunityGoalHallOfFameComposer : ServerPacket
{
    public CommunityGoalHallOfFameComposer(List<Habbo> users)
        : base(ServerPacketHeader.COMMUNITY_GOAL_HALL_OF_FAME)
    {
        this.WriteString(""); // goalCode
      
        this.WriteInteger(users.Count); // count
        foreach (var user in users)
        {
        
            this.WriteInteger(user.Id); // userId
            this.WriteString(user.Username); // userName
            this.WriteString(user.Look); // figure
            this.WriteInteger(0); // rank
            this.WriteInteger(user.GamePointsMonth); // currentScore
        }
    }
}