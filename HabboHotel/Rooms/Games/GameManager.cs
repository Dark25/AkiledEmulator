using Akiled.HabboHotel.Items;

using System;
using System.Collections.Generic;

namespace Akiled.HabboHotel.Rooms.Games
{
    public class GameManager
    {
        public int[] TeamPoints;
        private Dictionary<int, Item> redTeamItems;
        private Dictionary<int, Item> blueTeamItems;
        private Dictionary<int, Item> greenTeamItems;
        private Dictionary<int, Item> yellowTeamItems;
        private Room room;

        public int[] Points
        {
            get
            {
                return this.TeamPoints;
            }
            set
            {
                this.TeamPoints = value;
            }
        }

        public event TeamScoreChangedDelegate OnScoreChanged;

        public event RoomEventDelegate OnGameStart;

        public event RoomEventDelegate OnGameEnd;

        public GameManager(Room room)
        {
            this.TeamPoints = new int[5];
            this.redTeamItems = new Dictionary<int, Item>();
            this.blueTeamItems = new Dictionary<int, Item>();
            this.greenTeamItems = new Dictionary<int, Item>();
            this.yellowTeamItems = new Dictionary<int, Item>();
            this.room = room;
        }

        public Dictionary<int, Item> GetItems(Team team)
        {
            switch (team)
            {
                case Team.red:
                    return this.redTeamItems;
                case Team.green:
                    return this.greenTeamItems;
                case Team.blue:
                    return this.blueTeamItems;
                case Team.yellow:
                    return this.yellowTeamItems;
                default:
                    return new Dictionary<int, Item>();
            }
        }

        public Team getWinningTeam()
        {
            int NbTeam = 0;
            int MaxPoints = 0;
            for (int i = 1; i < 5; ++i)
            {
                if (this.TeamPoints[i] == MaxPoints)
                {
                    NbTeam = 0;
                }

                if (this.TeamPoints[i] > MaxPoints && this.TeamPoints[i] > 0)
                {
                    MaxPoints = this.TeamPoints[i];
                    NbTeam = i;
                }
            }
            return (Team)NbTeam;
        }

        public void AddPointToTeam(Team team, RoomUser user)
        {
            this.AddPointToTeam(team, 1, user);
        }

        public void AddPointToTeam(Team team, int points, RoomUser user)
        {
            int points1 = this.TeamPoints[(int)team] += points;
            if (points1 < 0)
                points1 = 0;
            if (points1 > 999)
                points1 = 999;

            this.TeamPoints[(int)team] = points1;

            if (this.OnScoreChanged != null)
                this.OnScoreChanged(null, new TeamScoreChangedArgs(points1, team, user));

            foreach (Item roomItem in this.GetFurniItems(team).Values)
            {
                if (IsScoreItem(roomItem.GetBaseItem().InteractionType))
                {
                    roomItem.ExtraData = this.TeamPoints[(int)team].ToString();
                    roomItem.UpdateState();
                }
            }
        }

        public void Reset()
        {
            this.AddPointToTeam(Team.blue, this.GetScoreForTeam(Team.blue) * -1, (RoomUser)null);
            this.AddPointToTeam(Team.green, this.GetScoreForTeam(Team.green) * -1, (RoomUser)null);
            this.AddPointToTeam(Team.red, this.GetScoreForTeam(Team.red) * -1, (RoomUser)null);
            this.AddPointToTeam(Team.yellow, this.GetScoreForTeam(Team.yellow) * -1, (RoomUser)null);
        }

        private int GetScoreForTeam(Team team)
        {
            return this.TeamPoints[(int)team];
        }

        private Dictionary<int, Item> GetFurniItems(Team team)
        {
            switch (team)
            {
                case Team.red:
                    return this.redTeamItems;
                case Team.green:
                    return this.greenTeamItems;
                case Team.blue:
                    return this.blueTeamItems;
                case Team.yellow:
                    return this.yellowTeamItems;
                default:
                    return new Dictionary<int, Item>();
            }
        }

        private static bool isSoccerGoal(InteractionType type)
        {
            if (type != InteractionType.footballgoalblue && type != InteractionType.footballgoalgreen && type != InteractionType.footballgoalred && type != InteractionType.footballgoalyellow)
                return false;
            else
                return true;
        }

        private static bool IsScoreItem(InteractionType type)
        {
            switch (type)
            {
                case InteractionType.banzaiscoreblue:
                case InteractionType.banzaiscorered:
                case InteractionType.banzaiscoreyellow:
                case InteractionType.banzaiscoregreen:

                case InteractionType.freezebluecounter:
                case InteractionType.freezegreencounter:
                case InteractionType.freezeredcounter:
                case InteractionType.freezeyellowcounter:
                    return true;
            }
            return false;

        }

        public void AddFurnitureToTeam(Item item, Team team)
        {
            switch (team)
            {
                case Team.red:
                    if (!redTeamItems.ContainsKey(item.Id))
                        this.redTeamItems.Add(item.Id, item);
                    break;
                case Team.green:
                    if (!greenTeamItems.ContainsKey(item.Id))
                        this.greenTeamItems.Add(item.Id, item);
                    break;
                case Team.blue:
                    if (!blueTeamItems.ContainsKey(item.Id))
                        this.blueTeamItems.Add(item.Id, item);
                    break;
                case Team.yellow:
                    if (!yellowTeamItems.ContainsKey(item.Id))
                        this.yellowTeamItems.Add(item.Id, item);
                    break;
            }
        }

        public void RemoveFurnitureFromTeam(Item item, Team team)
        {
            switch (team)
            {
                case Team.red:
                    this.redTeamItems.Remove(item.Id);
                    break;
                case Team.green:
                    this.greenTeamItems.Remove(item.Id);
                    break;
                case Team.blue:
                    this.blueTeamItems.Remove(item.Id);
                    break;
                case Team.yellow:
                    this.yellowTeamItems.Remove(item.Id);
                    break;
            }
        }

        public void UnlockGates()
        {
            foreach (Item roomItem in this.redTeamItems.Values)
                this.UnlockGate(roomItem);
            foreach (Item roomItem in this.greenTeamItems.Values)
                this.UnlockGate(roomItem);
            foreach (Item roomItem in this.blueTeamItems.Values)
                this.UnlockGate(roomItem);
            foreach (Item roomItem in this.yellowTeamItems.Values)
                this.UnlockGate(roomItem);
        }

        private void LockGate(Item item)
        {
            switch (item.GetBaseItem().InteractionType)
            {
                case InteractionType.freezebluegate:
                case InteractionType.freezegreengate:
                case InteractionType.freezeredgate:
                case InteractionType.freezeyellowgate:
                case InteractionType.banzaigateblue:
                case InteractionType.banzaigategreen:
                case InteractionType.banzaigatered:
                case InteractionType.banzaigateyellow:
                    //this.room.GetGameMap().UpdateGameMap(item, false);
                    break;
            }
        }

        public void UpdateGatesTeamCounts()
        {
            foreach (Item roomItem in this.redTeamItems.Values)
                this.UpdateGateTeamCount(roomItem);
            foreach (Item roomItem in this.greenTeamItems.Values)
                this.UpdateGateTeamCount(roomItem);
            foreach (Item roomItem in this.blueTeamItems.Values)
                this.UpdateGateTeamCount(roomItem);
            foreach (Item roomItem in this.yellowTeamItems.Values)
                this.UpdateGateTeamCount(roomItem);
        }

        private void UpdateGateTeamCount(Item item)
        {
            switch (item.GetBaseItem().InteractionType)
            {
                case InteractionType.banzaigateblue:
                case InteractionType.freezebluegate:
                    item.ExtraData = this.room.GetTeamManager().BlueTeam.Count.ToString();
                    item.UpdateState();
                    break;
                case InteractionType.banzaigatered:
                case InteractionType.freezeredgate:
                    item.ExtraData = this.room.GetTeamManager().RedTeam.Count.ToString();
                    item.UpdateState();
                    break;
                case InteractionType.banzaigategreen:
                case InteractionType.freezegreengate:
                    item.ExtraData = this.room.GetTeamManager().GreenTeam.Count.ToString();
                    item.UpdateState();
                    break;
                case InteractionType.banzaigateyellow:
                case InteractionType.freezeyellowgate:
                    item.ExtraData = this.room.GetTeamManager().YellowTeam.Count.ToString();
                    item.UpdateState();
                    break;
            }
        }

        private void UnlockGate(Item item)
        {
            switch (item.GetBaseItem().InteractionType)
            {
                case InteractionType.banzaigateblue:
                case InteractionType.freezebluegate:
                case InteractionType.freezegreengate:
                case InteractionType.banzaigategreen:
                case InteractionType.freezeredgate:
                case InteractionType.banzaigatered:
                case InteractionType.freezeyellowgate:
                case InteractionType.banzaigateyellow:
                    //this.room.GetGameMap().UpdateGameMap(item, true);
                    break;
            }
        }

        public void LockGates()
        {
            foreach (Item roomItem in this.redTeamItems.Values)
                this.LockGate(roomItem);
            foreach (Item roomItem in this.greenTeamItems.Values)
                this.LockGate(roomItem);
            foreach (Item roomItem in this.blueTeamItems.Values)
                this.LockGate(roomItem);
            foreach (Item roomItem in this.yellowTeamItems.Values)
                this.LockGate(roomItem);
        }

        public void StopGame()
        {
            this.room.GetBanzai().BanzaiEnd();
            this.room.GetFreeze().StopGame();

            //this.room.GetGameManager().UnlockGates();

            if (this.OnGameEnd != null)
                this.OnGameEnd(null, (EventArgs)null);
        }

        public void StartGame()
        {
            this.room.GetBanzai().BanzaiStart();
            this.room.GetFreeze().StartGame();

            //this.room.GetGameManager().LockGates();

            if (this.OnGameStart != null)
                this.OnGameStart(null, (EventArgs)null);
            this.room.lastTimerReset = DateTime.Now;
        }

        public Room GetRoom()
        {
            return this.room;
        }

        public void Destroy()
        {
            Array.Clear((Array)this.TeamPoints, 0, this.TeamPoints.Length);
            this.redTeamItems.Clear();
            this.blueTeamItems.Clear();
            this.greenTeamItems.Clear();
            this.yellowTeamItems.Clear();
            this.TeamPoints = (int[])null;
            this.OnScoreChanged = (TeamScoreChangedDelegate)null;
            this.OnGameStart = (RoomEventDelegate)null;
            this.OnGameEnd = (RoomEventDelegate)null;
            this.redTeamItems = (Dictionary<int, Item>)null;
            this.blueTeamItems = (Dictionary<int, Item>)null;
            this.greenTeamItems = (Dictionary<int, Item>)null;
            this.yellowTeamItems = (Dictionary<int, Item>)null;
            this.room = (Room)null;
        }
    }
}
