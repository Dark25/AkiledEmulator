namespace Akiled.HabboHotel.Navigators
{
    public static class NavigatorSearchAllowanceUtility
    {
        public static NavigatorSearchAllowance GetSearchAllowanceByString(string CategoryType)
        {
            switch (CategoryType.ToUpper())
            {
                default:
                case "NOTHING":
                    return NavigatorSearchAllowance.NOTHING;
                case "SHOW_MORE":
                    return NavigatorSearchAllowance.SHOW_MORE;
                case "GO_BACK":
                    return NavigatorSearchAllowance.GO_BACK;
            }
        }

        public static int GetIntegerValue(NavigatorSearchAllowance SearchAllowance)
        {
            switch (SearchAllowance)
            {
                default:
                case NavigatorSearchAllowance.NOTHING:
                    return 0;
                case NavigatorSearchAllowance.SHOW_MORE:
                    return 1;
                case NavigatorSearchAllowance.GO_BACK:
                    return 2;
            }
        }
    }
}
