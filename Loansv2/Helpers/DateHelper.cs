using System;

namespace Loansv2.Helpers
{
    public static class DateHelper
    {
        public static int GetDaysInYear(DateTime date)
        {
            var lastDate = new DateTime(date.Year, 12, 31);
            return lastDate.DayOfYear;
        }

        public static DateTime Min(DateTime date1, DateTime date2)
        {
            return date1 < date2 ? date1 : date2;
        }

        public static DateTime Max(DateTime date1, DateTime date2)
        {
            return date1 > date2 ? date1 : date2;
        }
    }
}