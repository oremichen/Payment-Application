using System;

namespace ContentServiceManagementAPI.Helpers
{
    public static class DateHelpers
    {
        public static bool IsSameDay(DateTime Date1, DateTime Date2)
        {
            if(Date1.Year == Date2.Year
                && Date1.Month == Date2.Month
                && Date1.Day == Date2.Day)
            {
                return true;
            }
            return false;
        }
    }
}
