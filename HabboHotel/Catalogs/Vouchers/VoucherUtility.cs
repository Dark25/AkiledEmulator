namespace Akiled.HabboHotel.Catalog.Vouchers
{
    public static class VoucherUtility
    {
        public static VoucherType GetType(string Type)
        {
            switch (Type)
            {
                default:
                case "credit":
                    return VoucherType.CREDIT;
                case "ducket":
                    return VoucherType.DUCKET;
                case "badge":
                    return VoucherType.BADGE;
                case "winwin":
                    return VoucherType.WINWIN;
            }
        }

        public static string FromType(VoucherType Type)
        {
            switch (Type)
            {
                default:
                case VoucherType.CREDIT:
                    return "credit";
                case VoucherType.DUCKET:
                    return "ducket";
                case VoucherType.BADGE:
                    return "badge";
                case VoucherType.WINWIN:
                    return "winwin";
            }
        }
    }
}
