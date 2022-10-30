using Akiled.HabboHotel.Rooms.Games;
using System;

namespace Akiled.HabboHotel.Rooms
{
    public class TeamScoreChangedArgs : EventArgs
    {
        public readonly int Points;
        public readonly Team Team;
        public readonly RoomUser user;

        public TeamScoreChangedArgs(int points, Team team, RoomUser user)
        {
            this.Points = points;
            this.Team = team;
            this.user = user;
        }
    }
}
