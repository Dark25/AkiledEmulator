using Akiled.HabboHotel.GameClients;namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd{    class lay : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);            if (room == null)                return;            RoomUser roomUserByHabbo = UserRoom;            if (roomUserByHabbo == null)                return;            try            {








                /*if (roomUserByHabbo.sentadoBol)                {                  roomUserByHabbo.sentadoBol = false;                  roomUserByHabbo.RemoveStatus("sit");                }*/
                if (roomUserByHabbo.Statusses.ContainsKey("lay") || roomUserByHabbo.Statusses.ContainsKey("sit"))                    return;                if (roomUserByHabbo.RotBody % 2 == 0 || roomUserByHabbo.transformation)                {                    if (roomUserByHabbo.RotBody == 4 || roomUserByHabbo.RotBody == 0 || roomUserByHabbo.transformation)                    {                        if (room.GetGameMap().CanWalk(roomUserByHabbo.X, roomUserByHabbo.Y + 1))                            roomUserByHabbo.RotBody = 0;                        else                            return;                    }                    else                    {                        if (!room.GetGameMap().CanWalk(roomUserByHabbo.X + 1, roomUserByHabbo.Y))                            return;                    }

                    //roomUserByHabbo.AddStatus("lay", Convert.ToString((double) room.GetGameMap().Model.SqFloorHeight[roomUserByHabbo.X, roomUserByHabbo.Y] + 0.85).Replace(",", "."));
                    if (UserRoom.transformation)                        roomUserByHabbo.SetStatus("lay", "");                    else                        roomUserByHabbo.SetStatus("lay", "0.7");                    roomUserByHabbo.IsLay = true;                    roomUserByHabbo.UpdateNeeded = true;                }            }            catch            {            }        }    }}