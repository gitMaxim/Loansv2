using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loansv2.Models
{
    public enum PaymentType
    {
        [Display(Name = "Выдача займа")]
        Credit,
        [Display(Name = "Погашение займа")]
        DebtLoan,
        [Display(Name = "Погашение %")]
        DebtPercents
    }

    public class Payment : IDateDecimal
    {
        public int Id { get; set; }

        [ForeignKey("LoanAgreement")]
        [Required(ErrorMessage = "Необходимо указать договор")]
        [Display(Name = "№ договора")]
        public int LoanAgreementId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Дата")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [Required(ErrorMessage = "Необходимо указать дату")]
        public DateTime Date { get; set; }

        [DataType(DataType.Currency, ErrorMessage = "Невозможно считать сумму в указанном формате")]
        [Display(Name = "Сумма")]
        [Required(ErrorMessage = "Необходимо указать сумму платежа")]
        [Range(typeof(decimal), "1", "79228162514264337593543950335",
            ErrorMessage = "Сумма должна быть положительна и не превышать максимально возможную")]
        public decimal Value { get; set; }

        [Display(Name = "Тип")]
        [Required(ErrorMessage = "Необходимо указать тип платежа")]
        public PaymentType PaymentType { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }


        #region Constructors
        public Payment()
        {
        }

        public Payment(DateTime date, decimal value, PaymentType paymentType)
        {
            Date = date;
            Value = value;
            PaymentType = paymentType;
        }

        public Payment(int loanAgreementId, DateTime date, decimal value, PaymentType paymentType)
        {
            LoanAgreementId = loanAgreementId;
            Date = date;
            Value = value;
            PaymentType = paymentType;
        }
        #endregion


        public string DisplayTypeName()
        {
            switch (PaymentType)
            {
                case PaymentType.Credit:
                    return "Выдача займа";
                case PaymentType.DebtLoan:
                    return "Погашение займа";
                case PaymentType.DebtPercents:
                    return "Погашение %";
                default:
                    return "?";
            }
        }


        public virtual LoanAgreement LoanAgreement { get; set; }
    }
}