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
    class GolpeCommand : IChatCommand
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

            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor escriba el nombre del usuari@ que quieres golpear.");
                return;
            }

            if (!TargetRoom.RoomData.GolpeEnabled && !TargetRoom.CheckRights(Session, true) && !Session.GetHabbo().HasFuse("room_override_custom_config"))
            {
                Session.SendWhisper("Disculpa, pero el dueño de la sala ha desactivado este comando o no tienes permiso en la sala.");
                return;
            }

            GameClient TargetClient = AkiledEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Este usuario no se encuentra en la sala o está desconectad@.");
                return;
            }
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabboId(TargetClient.GetHabbo().Id);
            if (TargetUser == null)
            {
                Session.SendWhisper("Uy, ha ocurrido un error.");
            }
            if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("Te quieres golpear tu mism@? lol!");
                return;
            }
            if ((TargetClient.GetHabbo().Username == "Emmanuelrtpo") || (TargetClient.GetHabbo().Username == "Norman") || (TargetClient.GetHabbo().Username == "Nicofer"))
            {
                Session.SendWhisper("No se puede golpear a este usuario.");
                return;
            }

            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (ThisUser == null)
                return;

            if (!((Math.Abs(TargetUser.X - ThisUser.X) >= 2) || (Math.Abs(TargetUser.Y - ThisUser.Y) >= 2)))
            {
                Room.SendPacketWeb(new PlaySoundComposer("golpe", 2)); //Type = Trax
                Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "@red@ *" + Params[1] + " ha recibido un golpe en la cara*", 0, ThisUser.LastBubble));
                Room.SendPacket(new ChatComposer(TargetUser.VirtualId, "@cyan@ ¡Menuda ostia men!", 0, ThisUser.LastBubble));

                if (TargetUser.RotBody == 4)
                {
                    TargetUser.Statusses.Add("lay", "1.0 null");
                    TargetUser.Z -= 0.35;
                    TargetUser.IsSit = true;
                    TargetUser.UpdateNeeded = true;
                    TargetUser.ApplyEffect(157);
                }

                if (ThisUser.RotBody == 0)
                {
                    TargetUser.Statusses.Add("lay", "1.0 null");
                    TargetUser.Z -= 0.35;
                    TargetUser.IsSit = true;
                    TargetUser.UpdateNeeded = true;
                    TargetUser.ApplyEffect(157);
                }

                if (ThisUser.RotBody == 6)
                {
                    TargetUser.Statusses.Add("lay", "1.0 null");
                    TargetUser.Z -= 0.35;
                    TargetUser.IsSit = true;
                    TargetUser.UpdateNeeded = true;
                    TargetUser.ApplyEffect(157);
                }

                if (ThisUser.RotBody == 2)
                {
                    TargetUser.Statusses.Add("lay", "1.0 null");
                    TargetUser.Z -= 0.35;
                    TargetUser.IsSit = true;
                    TargetUser.UpdateNeeded = true;
                    TargetUser.ApplyEffect(157);
                }

                if (ThisUser.RotBody == 3)
                {
                    TargetUser.Statusses.Add("lay", "1.0 null");
                    TargetUser.Z -= 0.35;
                    TargetUser.IsSit = true;
                    TargetUser.UpdateNeeded = true;
                    TargetUser.ApplyEffect(157);
                }

                if (ThisUser.RotBody == 1)
                {
                    TargetUser.Statusses.Add("lay", "1.0 null");
                    TargetUser.Z -= 0.35;
                    TargetUser.IsSit = true;
                    TargetUser.UpdateNeeded = true;
                    TargetUser.ApplyEffect(157);
                }

                if (ThisUser.RotBody == 7)
                {
                    TargetUser.Statusses.Add("lay", "1.0 null");
                    TargetUser.Z -= 0.35;
                    TargetUser.IsSit = true;
                    TargetUser.UpdateNeeded = true;
                    TargetUser.ApplyEffect(157);
                }

                if (ThisUser.RotBody == 5)
                {
                    TargetUser.Statusses.Add("lay", "1.0 null");
                    TargetUser.Z -= 0.35;
                    TargetUser.IsSit = true;
                    TargetUser.UpdateNeeded = true;
                    TargetUser.ApplyEffect(157);
                }
                Session.GetHabbo().last_golpe = AkiledEnvironment.GetIUnixTimestamp();


            }
            else
            {
                Session.SendWhisper("@green@ ¡Oops, " + Params[1] + " no está lo suficientemente cerca!");
            }

        }
    }
}