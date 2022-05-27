using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.HabboHotel.ChatMessageStorage;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.SpecialPvP
{
    class BurnCommand : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {

            if (UserRoom.Team != Team.none || UserRoom.InGame)
                return;

            Room TargetRoom = Session.GetHabbo().CurrentRoom;

            if ((double)Session.GetHabbo().last_burn > AkiledEnvironment.GetUnixTimestamp() - 30.0 && !Session.GetHabbo().HasFuse("override_limit_command"))
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
                Session.SendWhisper("Introduce el nombre del usuario que deseas besar.");
                return;
            }

            if (!TargetRoom.RoomData.BurnEnabled && !TargetRoom.CheckRights(Session, true) && !Session.GetHabbo().HasFuse("room_override_custom_config"))
            {
                Session.SendWhisper("Oops, al parecer el dueño de la sala ha prohibido la parrilla humana en su sala.");
                return;
            }

            if ((TargetUser.GetClient().GetHabbo().HasFuse("no_accept_use_custom_commands")))
            {
                Session.SendWhisper("No se puede quemar a este usuario.");
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
                return;
            }

            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (ThisUser == null)
                return;

            if (!((Math.Abs(TargetUser.X - ThisUser.X) >= 2) || (Math.Abs(TargetUser.Y - ThisUser.Y) >= 2)))
            {
                GameClient Target = AkiledEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
                RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabboId(Target.GetHabbo().Id);
                Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@green@ *Sacar bomba molotov*", 0, ThisUser.LastBubble));
                ThisUser.ApplyEffect(1005);
                System.Threading.Thread.Sleep(1500);
                Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@ * Lanzarle bomba a  " + Target.GetHabbo().Username + "*", 0, ThisUser.LastBubble));
                Room.SendPacketWeb(new PlaySoundComposer("molotov", 2)); //Type = Trax
                ThisUser.ApplyEffect(0);
                System.Threading.Thread.Sleep(500);
                Room.SendPacket(new ChatComposer(TargetUser.VirtualId, "@red@ *¡AHHHHHH! ¡AYUDAAAA! ¡ME QUEMO!*", 0, TargetUser.LastBubble));
                TargetUser.ApplyEffect(25);
                System.Threading.Thread.Sleep(500);
                TargetUser.ApplyEffect(0);
                Session.GetHabbo().last_burn = AkiledEnvironment.GetIUnixTimestamp();

            }
            else
            {
                Session.SendWhisper("@green@ ¡Oops, " + Params[1] + " no estas lo suficientemente cerca!");
            }

        }
    }
}