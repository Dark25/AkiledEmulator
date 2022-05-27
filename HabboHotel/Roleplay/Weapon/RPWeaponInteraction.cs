namespace Akiled.HabboHotel.Roleplay.Weapon
{
    public enum RPWeaponInteraction
    {
        NONE,
    }
    public class RPWeaponInteractions
    {
        public static RPWeaponInteraction GetTypeFromString(string pType)
        {
            switch (pType)
            {
                case "none":
                    return RPWeaponInteraction.NONE;
                default:
                    return RPWeaponInteraction.NONE;
            }
        }
    }
}
