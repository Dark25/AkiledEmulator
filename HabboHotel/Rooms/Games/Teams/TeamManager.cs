using System.Collections.Generic;

namespace Akiled.HabboHotel.Rooms.Games
{
    public class TeamManager
    {
        public List<RoomUser> BlueTeam;
        public List<RoomUser> RedTeam;
        public List<RoomUser> YellowTeam;
        public List<RoomUser> GreenTeam;

        public TeamManager()
        {
            this.BlueTeam = new List<RoomUser>();
            this.RedTeam = new List<RoomUser>();
            this.GreenTeam = new List<RoomUser>();
            this.YellowTeam = new List<RoomUser>();
        }

        public List<RoomUser> GetAllPlayer()
        {
            List<RoomUser> Players = new List<RoomUser>();

            Players.AddRange(this.BlueTeam);
            Players.AddRange(this.RedTeam);
            Players.AddRange(this.GreenTeam);
            Players.AddRange(this.YellowTeam);

            return Players;
        }

        public bool CanEnterOnTeam(Team t)
        {
            if (t.Equals(Team.blue))
                return this.BlueTeam.Count < 5;
            if (t.Equals(Team.red))
                return this.RedTeam.Count < 5;
            if (t.Equals(Team.yellow))
                return this.YellowTeam.Count < 5;
            if (t.Equals(Team.green))
                return this.GreenTeam.Count < 5;
            else
                return false;
        }

        public void AddUser(RoomUser user)
        {
            if (user.Team.Equals(Team.blue))
                this.BlueTeam.Add(user);
            else if (user.Team.Equals(Team.red))
                this.RedTeam.Add(user);
            else if (user.Team.Equals(Team.yellow))
                this.YellowTeam.Add(user);
            else if (user.Team.Equals(Team.green))
                this.GreenTeam.Add(user);
        }

        public void OnUserLeave(RoomUser user)
        {
            if (user.Team.Equals(Team.blue))
                this.BlueTeam.Remove(user);
            else if (user.Team.Equals(Team.red))
                this.RedTeam.Remove(user);
            else if (user.Team.Equals(Team.yellow))
                this.YellowTeam.Remove(user);
            else if (user.Team.Equals(Team.green))
                this.GreenTeam.Remove(user);
        }
    }
}
