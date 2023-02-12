using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms.RoomBots;
using System.Collections.Generic;
using System.Drawing;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class SuperBot : IChatCommand
    {
        

        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            
            if (Params.Length > 1)
            {
                int count;
                if (Params.Length == 2)
                {
                    int.TryParse(Params[1], out count);
                   
                    for (int i = 0; i < count; i++)
                    {
                        if (!Room.IsRoleplay)
                        {
                            RoomUser superBot = Room.GetRoomUserManager().DeploySuperBot(new RoomBot(-i, Session.GetHabbo().Id, Room.Id, AIType.SuperBot, false, Session.GetHabbo().Username, "SuperBot", Session.GetHabbo().Gender, Session.GetHabbo().Look, UserRoom.X, UserRoom.Y, 0, 2, false, "", 0, false, 0, 0, 0));
                            superBot.BotData.FollowUser = UserRoom.VirtualId;
                          
                        }
                        else
                        {
                            Room.GetRoomUserManager().DeploySuperBot(new RoomBot(-i, Session.GetHabbo().Id, Room.Id, AIType.SuperBot, false, Session.GetHabbo().Username, "SuperBot", Session.GetHabbo().Gender, Session.GetHabbo().Look, UserRoom.X, UserRoom.Y, 0, 2, false, "", 0, false, 0, 0, 0));
                        }
                    }
                }
                else if (Params.Length > 2)
                {
                    RoomUser GetUserRoom = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Params[1]);
                    if (GetUserRoom == null)
                        return;

                    if (Session.Langue != GetUserRoom.GetClient().Langue)
                    {
                        UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue(string.Format("cmd.authorized.langue.user", GetUserRoom.GetClient().Langue), Session.Langue));
                        return;
                    }

                    int.TryParse(Params[2], out count);
                   
                    
                    for (int i = 0; i < count; i++)
                    {
                        RoomUser superBot = Room.GetRoomUserManager().DeploySuperBot(new RoomBot(-i, GetUserRoom.GetClient().GetHabbo().Id, Room.Id, AIType.SuperBot, false, GetUserRoom.GetClient().GetHabbo().Username, "SuperBot", GetUserRoom.GetClient().GetHabbo().Gender, GetUserRoom.GetClient().GetHabbo().Look, GetUserRoom.X, GetUserRoom.Y, 0, 2, false, "", 0, false, 0, 0, 0));
                        superBot.BotData.FollowUser = GetUserRoom.VirtualId;
                    }
                }
            }
            else
            {
                ICollection<GameClient> Users = AkiledEnvironment.GetGame().GetClientManager().GetClients;

                if (Users == null) return;

                foreach (GameClient GameClient in Users)
                {
                    if (GameClient.GetHabbo() == null) continue;

                    Point randomWalkableSquare = Room.GetGameMap().getRandomWalkableSquare(UserRoom.X, UserRoom.Y);

                    RoomUser superBot = Room.GetRoomUserManager().DeploySuperBot(new RoomBot(0, Session.GetHabbo().Id, Room.Id, AIType.SuperBot, false, GameClient.GetHabbo().Username, GameClient.GetHabbo().Motto, GameClient.GetHabbo().Gender, GameClient.GetHabbo().Look, UserRoom.X, UserRoom.Y, 0, 2, false, "", 0, false, 0, 0, 0));
                    superBot.MoveTo(randomWalkableSquare, true);
                }

                /*
                int Id = 1;
                for (int Y = 0; Y < Room.GetGameMap().Model.MapSizeY; ++Y)
                {
                    for (int X = 0; X < Room.GetGameMap().Model.MapSizeX; ++X)
                    {
                        if (!Room.GetGameMap().CanWalk(X, Y, false))
                            continue;

                        Id++;
                     
                        RoomUser superBot = Room.GetRoomUserManager().DeploySuperBot(new RoomBot(-Id, Session.GetHabbo().Id, Room.Id, AIType.SuperBot, false, Session.GetHabbo().Username, Session.GetHabbo().Motto, Session.GetHabbo().Gender, Session.GetHabbo().Look, UserRoom.X, UserRoom.Y, 0, 2, false, "", 0, false, 0, 0, 0));
                        superBot.MoveTo(X, Y, true);
                    }
                }
                */
            }
        }
    }
}
