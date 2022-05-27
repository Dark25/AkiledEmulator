namespace Akiled.HabboHotel.Rooms
{

    public enum ItemEffectType
    {
        None,
        Swim,
        SwimLow,
        SwimHalloween,
        Iceskates,
        Normalskates,
        PublicPool,
        Trampoline,
        TreadMill,
        CrossTrainer,
    }

    public static class ByteToItemEffectEnum
    {
        public static ItemEffectType Parse(byte pByte)
        {
            switch (pByte)
            {
                case (byte)0:
                    return ItemEffectType.None;
                case (byte)1:
                    return ItemEffectType.Swim;
                case (byte)2:
                    return ItemEffectType.Normalskates;
                case (byte)3:
                    return ItemEffectType.Iceskates;
                case (byte)4:
                    return ItemEffectType.SwimLow;
                case (byte)5:
                    return ItemEffectType.SwimHalloween;
                case (byte)6:
                    return ItemEffectType.PublicPool;
                case (byte)7:
                    return ItemEffectType.Trampoline;
                case (byte)8:
                    return ItemEffectType.TreadMill;
                case (byte)9:
                    return ItemEffectType.CrossTrainer;
                default:
                    return ItemEffectType.None;
            }
        }
    }
}
