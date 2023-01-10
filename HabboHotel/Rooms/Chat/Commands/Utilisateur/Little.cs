using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;using Akiled.HabboHotel.Rooms.Games;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd{    class Little : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {
            if (Params.Length != 2)
                return;

            if (UserRoom.Team != Team.none || UserRoom.InGame)
                return;            if (Session.GetHabbo().SpectatorMode || UserRoom.InGame)                return;

            if (!UserRoom.SetPetTransformation("little" + Params[1], 0))
            {
                Session.SendHugeNotif(AkiledEnvironment.GetLanguageManager().TryGetValue("cmd.littleorbig.help", Session.Langue));
                return;
            }

            UserRoom.transformation = true;

            Room.SendPacket(new UserRemoveComposer(UserRoom.VirtualId));
            Room.SendPacket(new UsersComposer(UserRoom));        }    }}