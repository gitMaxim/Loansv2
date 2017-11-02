using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loansv2.Models
{
    [NotMapped]
    public class DateDecimal : IDateDecimal
    {
        [Display(Name = "Дата")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime Date { get; set; }

        [DataType(DataType.Currency)]
        public decimal Value { get; set; }


        #region Constructors
        public DateDecimal()
        {
        }

        public DateDecimal(DateTime date, decimal value)
        {
            Date = date;
            Value = value;
        }
        #endregion
    }
}