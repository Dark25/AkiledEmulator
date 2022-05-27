namespace Akiled.HabboHotel.Roleplay.Item
{
    public enum RPItemCategory
    {
        EQUIP,
        UTIL,
        RESSOURCE,
        QUETE,
    }
    public class RPItemCategorys
    {
        public static RPItemCategory GetTypeFromString(string pType)
        {
            switch (pType)
            {
                case "EQUIP":
                    return RPItemCategory.EQUIP;
                case "UTIL":
                    return RPItemCategory.UTIL;
                case "RESSOURCE":
                    return RPItemCategory.RESSOURCE;
                case "QUETE":
                    return RPItemCategory.QUETE;
                default:
                    return RPItemCategory.QUETE;
            }
        }
    }
}
