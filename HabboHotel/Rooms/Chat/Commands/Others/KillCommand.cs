using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms.Games;
using System;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.SpecialPvP
{
    class KillCommand : IChatCommand
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

            if (Session.GetHabbo().IgnoreAll)
                return;

            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor escriba el nombre del usuari@ que quieres matar.");
                return;
            }

            if (!TargetRoom.RoomData.MatarEnabled && !TargetRoom.CheckRights(Session, true) && !Session.GetHabbo().HasFuse("room_override_custom_config"))
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
                Session.SendWhisper("Te quieres matar tu mism@? lol!");
                return;
            }


            if ((TargetUser.GetClient().GetHabbo().HasFuse("no_accept_use_custom_commands")))
            {
                Session.SendWhisper("No se puede matar a este usuario.");
                return;
            }

            if ((TargetClient.GetHabbo().Username == "Emmanuelrtpo") || (TargetClient.GetHabbo().Username == "Norman") || (TargetClient.GetHabbo().Username == "Nicofer"))
            {
                Session.SendWhisper("No se puede matar a este usuario.");
                return;
            }

            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (ThisUser == null)
                return;

            if (!((Math.Abs(TargetUser.X - ThisUser.X) > 1) || (Math.Abs(TargetUser.Y - ThisUser.Y) > 1)))
            {
                ThisUser.ApplyEffect(1000);
                System.Threading.Thread.Sleep(1000);
                Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "* Quieto Mmgv@ " + Params[1] + ", Tas Pega@ # PlopLO *", 0, ThisUser.LastBubble));
                System.Threading.Thread.Sleep(500);
                Room.SendPacket(new ChatComposer(ThisUser.VirtualId, "* Disparar en la cara a " + Params[1] + " *", 0, ThisUser.LastBubble));
                Room.SendPacketWeb(new PlaySoundComposer("pistoladeoro", 2)); //Type = Trax
                System.Threading.Thread.Sleep(1000);
                Room.SendPacket(new ChatComposer(TargetUser.VirtualId, "@red@ * :x :x Ayudaaaa :( me mataron*", 0, ThisUser.LastBubble));
                TargetUser.RotBody--;//
                TargetUser.Statusses.Add("lay", "1.0 null");
                TargetUser.Z -= 0.35;
                TargetUser.IsSit = true;
                TargetUser.UpdateNeeded = true;
                Session.GetHabbo().last_kiss = AkiledEnvironment.GetIUnixTimestamp();

            }
            else
            {
                Session.SendWhisper("¡Oops! " + Params[1] + " esta muy alejado.");
            }

        }
    }
}