using System;

namespace Loansv2.Helpers
{
    public static class DateLabel
    {
        public static string GetMonthYear(DateTime date)
        {
            var str = "";
            switch (date.Month)
            {
                case 1:
                    str = "Янв";
                    break;
                case 2:
                    str = "Фев";
                    break;
                case 3:
                    str = "Мрт";
                    break;
                case 4:
                    str = "Апр";
                    break;
                case 5:
                    str = "Май";
                    break;
                case 6:
                    str = "Июн";
                    break;
                case 7:
                    str = "Июл";
                    break;
                case 8:
                    str = "Авг";
                    break;
                case 9:
                    str = "Сен";
                    break;
                case 10:
                    str = "Окт";
                    break;
                case 11:
                    str = "Нбр";
                    break;
                case 12:
                    str = "Дек";
                    break;
            }

            str += $" {date.Year}";
            return str;
        }
    }
}