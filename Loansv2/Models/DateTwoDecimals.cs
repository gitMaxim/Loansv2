using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loansv2.Models
{
    [NotMapped]
    public class DateTwoDecimals : DateDecimal
    {
        [DataType(DataType.Currency)]
        public decimal Aux { get; set; }

        #region Constructors
        public DateTwoDecimals() : base()
        {
        }

        public DateTwoDecimals(DateTime date, decimal value, decimal aux)
            : base(date, value)
        {
            Aux = aux;
        }
        #endregion
    }
}