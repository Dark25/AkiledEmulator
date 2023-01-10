using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Roleplay.Player;
using Akiled.HabboHotel.Rooms.Map.Movement;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class Pan : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (!Room.IsRoleplay || !Room.Pvp || UserRoom.Freeze)
                return;

            RolePlayer Rp = UserRoom.Roleplayer;
            if (Rp == null)
                return;

            if (Rp.Dead || !Rp.PvpEnable || Rp.SendPrison)
                return;

            if (Rp.Munition <= 0)
            {
                UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("rp.munitionnotfound", Session.Langue));
                return;
            }

            if (Rp.GunLoad <= 0)
            {
                UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("rp.reloadweapon", Session.Langue));
                return;
            }

            MovementDirection movement = MovementManagement.GetMovementByDirection(UserRoom.RotBody);

            int WeaponEanble = Rp.WeaponGun.Enable;

            UserRoom.ApplyEffect(WeaponEanble, true);
            UserRoom.TimerResetEffect = Rp.WeaponGun.FreezeTime;

            Rp.AggroTimer = 30;

            if (UserRoom.FreezeEndCounter <= Rp.WeaponGun.FreezeTime)
            {
                UserRoom.Freeze = true;
                UserRoom.FreezeEndCounter = Rp.WeaponGun.FreezeTime;
            }

            for (int i = 0; i < Rp.WeaponGun.FreezeTime; i++)
            {
                if (Rp.Munition <= 0 || Rp.GunLoad <= 0)
                    break;

                Rp.Munition--;
                Rp.GunLoad--;

                int Dmg = AkiledEnvironment.GetRandomNumber(Rp.WeaponGun.DmgMin, Rp.WeaponGun.DmgMax);

                if (Rp.WeaponGun.Id == 21)
                {
                    Room.GetProjectileManager().AddGrenade(UserRoom.VirtualId, UserRoom.SetX, UserRoom.SetY, UserRoom.SetZ, movement, Dmg, Rp.WeaponGun.Distance);
                }
                else
                    Room.GetProjectileManager().AddProjectile(UserRoom.VirtualId, UserRoom.SetX, UserRoom.SetY, UserRoom.SetZ, movement, Dmg, Rp.WeaponGun.Distance);

            }


            if (Rp.WeaponGun.Id == 5)
            {
                Room.SendPacketWeb(new PlaySoundComposer("revolver", 2)); //Type = Trax
            }
            if (Rp.WeaponGun.Id == 6)
            {
                Room.SendPacketWeb(new PlaySoundComposer("ak47", 2)); //Type = Trax
            }
            if (Rp.WeaponGun.Id == 7)
            {
                Room.SendPacketWeb(new PlaySoundComposer("sniper", 2)); //Type = Trax
            }
            if (Rp.WeaponGun.Id == 8)
            {
                Room.SendPacketWeb(new PlaySoundComposer("glock", 2)); //Type = Trax
            }
            if (Rp.WeaponGun.Id == 9)
            {
                Room.SendPacketWeb(new PlaySoundComposer("shotgun", 2)); //Type = Trax
            }
            if (Rp.WeaponGun.Id == 10)
            {
                Room.SendPacketWeb(new PlaySoundComposer("mp5", 2)); //Type = Trax
            }
            if (Rp.WeaponGun.Id == 11)//pistoladoble
            {
                Room.SendPacketWeb(new PlaySoundComposer("pistoladoble", 2)); //Type = Trax
            }
            if (Rp.WeaponGun.Id == 12)//pistoladeoro
            {
                Room.SendPacketWeb(new PlaySoundComposer("pistoladeoro", 2)); //Type = Trax
            }
            if (Rp.WeaponGun.Id == 13)//pistolasilenciador
            {
                Room.SendPacketWeb(new PlaySoundComposer("pistolasilenciadora", 2)); //Type = Trax
            }
            if (Rp.WeaponGun.Id == 14)//OTRAPISTOLA
            {
                Room.SendPacketWeb(new PlaySoundComposer("otrapistola", 2)); //Type = Trax
            }
            if (Rp.WeaponGun.Id == 15)//OTRAPISTOLA
            {
                Room.SendPacketWeb(new PlaySoundComposer("otrapistola1", 2)); //Type = Trax
            }
            if (Rp.WeaponGun.Id == 16)//OTRAPISTOLA
            {
                Room.SendPacketWeb(new PlaySoundComposer("otrapistola2", 2)); //Type = Trax
            }
            if (Rp.WeaponGun.Id == 17)//OTRAPISTOLA
            {
                Room.SendPacketWeb(new PlaySoundComposer("otrapistola3", 2)); //Type = Trax
            }
            if (Rp.WeaponGun.Id == 18)//OTRAPISTOLA
            {
                Room.SendPacketWeb(new PlaySoundComposer("otrapistola4", 2)); //Type = Trax
            }
            if (Rp.WeaponGun.Id == 19)//OTRAPISTOLA
            {
                Room.SendPacketWeb(new PlaySoundComposer("otrapistola5", 2)); //Type = Trax
            }
            if (Rp.WeaponGun.Id == 20)//OTRAPISTOLA
            {
                Room.SendPacketWeb(new PlaySoundComposer("otrapistola6", 2)); //Type = Trax
            }
            if (Rp.WeaponGun.Id == 21)//GRANADA
            {
                Room.SendPacketWeb(new PlaySoundComposer("granada", 2)); //Type = Trax
            }

            Rp.SendUpdate();
        }
    }
}
