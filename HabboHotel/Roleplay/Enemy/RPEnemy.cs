namespace Akiled.HabboHotel.Roleplay.Enemy
{
    public class RPEnemy
    {
        public int Id;
        public int Health;
        public int WeaponGunId;
        public int WeaponCacId;
        public int DeadTimer;
        public int MoneyDrop;
        public int DropScriptId;
        public int TeamId;
        public int ZoneDistance;
        public bool ResetPosition;
        public int AggroDistance;
        public int LootItemId;
        public int LostAggroDistance;
        public bool ZombieMode;

        public RPEnemy(int pId, int pHealth, int pWeaponGunId, int pWeaponCacId, int pDeadTimer, int pLootItemId, int pMoneyDrop, int pDropScriptId, int pTeamId, int pAggroDistance, int pZoneDistance,
            bool pResetPosition, int pLostAggroDistance, bool pZombieMode)
        {
            this.Id = pId;
            this.Health = pHealth;
            this.WeaponGunId = pWeaponGunId;
            this.WeaponCacId = pWeaponCacId;
            this.DeadTimer = pDeadTimer;
            this.MoneyDrop = pMoneyDrop;
            this.LootItemId = pLootItemId;
            this.MoneyDrop = pMoneyDrop;
            this.DropScriptId = pDropScriptId;
            this.TeamId = pTeamId;
            this.AggroDistance = pAggroDistance;
            this.ZoneDistance = pZoneDistance;
            this.ResetPosition = pResetPosition;
            this.LostAggroDistance = pLostAggroDistance;
            this.ZombieMode = pZombieMode;
        }
    }
}
