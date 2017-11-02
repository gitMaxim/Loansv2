using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loansv2.Models
{
    public class LoanViewModel : DateTwoDecimals
    {
        [NotMapped]
        [Display(Name = "Тип")]
        public PaymentType PaymentType { get; set; }


        #region Constructors
        public LoanViewModel() : base()
        {
            
        }

        public LoanViewModel(DateTime date, decimal value, decimal aux, PaymentType paymentType)
            : base(date, value, aux)
        {
            PaymentType = paymentType;
        }
        #endregion


        public string DisplayTypeName()
        {
            switch (PaymentType)
            {
                case PaymentType.Credit:
                    return "Выдача";
                case PaymentType.DebtLoan:
                    return "Погашение";
                default:
                    return "?";
            }
        }
    }
}