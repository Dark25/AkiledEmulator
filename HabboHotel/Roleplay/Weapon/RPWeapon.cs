namespace Akiled.HabboHotel.Roleplay.Weapon
{
    public class RPWeapon
    {
        public int Id;
        public int DmgMin;
        public int DmgMax;
        public RPWeaponInteraction Interaction;
        public int Enable;
        public int FreezeTime;
        public int Distance;

        public RPWeapon(int pId, int pDmgMin, int pDmgMax, RPWeaponInteraction pInteraction, int pEnable, int pFreezeTime, int pDistance)
        {
            this.Id = pId;
            this.DmgMin = pDmgMin;
            this.DmgMax = pDmgMax;
            this.Interaction = pInteraction;
            this.Enable = pEnable;
            this.FreezeTime = pFreezeTime;
            this.Distance = pDistance;
        }
    }
}
