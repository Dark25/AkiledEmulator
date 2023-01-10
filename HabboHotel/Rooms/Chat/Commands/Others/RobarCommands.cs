using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms.Games;
using System;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class RobarCommands : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {

            if (UserRoom.Team != Team.none || UserRoom.InGame)
                return;

            Room TargetRoom = Session.GetHabbo().CurrentRoom;

            if ((double)Session.GetHabbo().last_kiss > AkiledEnvironment.GetUnixTimestamp() - 30.0 && !Session.GetHabbo().HasFuse("override_limit_command"))
            {
                Session.SendWhisper("Debes esperar 30 segundos, para volver a usar el comando", 1);
                return;
            }

            if (TargetRoom == null)
                return;

            RoomUser roomuser = TargetRoom.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomuser == null)
                return;

            if (Params.Length != 2)
                return;

            RoomUser TargetUser = TargetRoom.GetRoomUserManager().GetRoomUserByHabbo(Params[1]);

            if (Params.Length == 1)
            {
                Session.SendWhisper("Introduce el nombre del usuario que deseas dar una Atracar(Robar).");
                return;
            }

            if (!TargetRoom.RoomData.BesarEnabled && !TargetRoom.CheckRights(Session, true) && !Session.GetHabbo().HasFuse("room_override_custom_config"))
            {
                Session.SendWhisper("Oops, al parecer el dueño de la sala ha prohibido los atracos (Robos) en su sala.");
                return;
            }

            if ((TargetUser.GetClient().GetHabbo().HasFuse("no_accept_use_custom_commands")))
            {
                Session.SendWhisper("No se puede tocar a este usuario.");
                return;
            }

            if (TargetUser == null)
            {
                Session.SendWhisper(Convert.ToString(Params[1]) + " el usuario ya no se encuentra aquí.");
                return;
            }

            if (TargetUser.GetClient().GetHabbo().Id == Session.GetHabbo().Id)
                return;

            if (TargetUser.GetClient().GetHabbo().PremiumProtect && !Session.GetHabbo().HasFuse("fuse_mod"))
            {
                roomuser.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("premium.notallowed", Session.Langue));
            }

            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (ThisUser == null)
                return;

            if (Session.GetHabbo().Rank <= 3)
            {
                if (!((Math.Abs(TargetUser.X - ThisUser.X) >= 2) || (Math.Abs(TargetUser.Y - ThisUser.Y) >= 2)))
                {
                    Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "* Dame El Iphone Pendej@ " + Params[1] + " *", 0, ThisUser.LastBubble));
                    System.Threading.Thread.Sleep(500);
                    Room.SendPacket(new ChatComposer(TargetUser.VirtualId, "@red@ * He Sido Robad@ Por: * " + Session.GetHabbo().Username + " :( *", 0, ThisUser.LastBubble));
                    ThisUser.ApplyEffect(101);
                    TargetUser.ApplyEffect(65);
                    return;

                }
                else
                {
                    Session.SendWhisper("@green@ ¡Oops, " + Params[1] + " no está lo suficientemente cerca!");
                }
            }
            if (Session.GetHabbo().Rank >= 4)
            {
                Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "* Dame El Iphone Pendej@ " + Params[1] + " *", 0, ThisUser.LastBubble));
                System.Threading.Thread.Sleep(500);
                Room.SendPacket(new ChatComposer(TargetUser.VirtualId, "@red@ * He Sido Robad@ Por: * " + Session.GetHabbo().Username + " :( *", 0, ThisUser.LastBubble));
                ThisUser.ApplyEffect(101);
                TargetUser.ApplyEffect(65);
                return;
            }

        }
    }
}
