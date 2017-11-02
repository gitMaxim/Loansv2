using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loansv2.Models
{
    public class PercentViewModel : DateTwoDecimals
    {
        [NotMapped]
        [DataType(DataType.Currency)]
        public decimal Payment { get; set; }


        #region Constructors
        public PercentViewModel()
        {
        }

        public PercentViewModel(DateTime date, decimal value, decimal aux, decimal payment)
        {
            Date = date;
            Value = value;
            Aux = aux;
            Payment = payment;
        }
        #endregion


        public string DisplayValue(decimal val)
        {
            if (Math.Abs(val) < (decimal) 0.01)
                return "-";
            return $"{val:N}";
        }
    }
}