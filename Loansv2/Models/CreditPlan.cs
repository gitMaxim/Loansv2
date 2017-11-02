using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Loansv2.Models
{
    public class CreditPlan : IDateDecimal
    {
        public int Id { get; set; }

        [ForeignKey("LoanAgreement")]
        public int LoanAgreementId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Дата")]
        [Remote("ValidateCreditDate", "CreditPlan", AdditionalFields = "LoanAgreementId")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [Required(ErrorMessage = "Необходимо указать дату")]
        public DateTime Date { get; set; }

        [DataType(DataType.Currency, ErrorMessage = "Невозможно считать сумму в указанном формате")]
        [Display(Name = "Предоставляемая сумма")]
        [Remote("ValidateCreditSum", "CreditPlan", AdditionalFields = "LoanAgreementId")]
        [Required(ErrorMessage = "Необходимо указать предоставляемую сумму займа")]
        [Range(typeof(decimal), "1", "79228162514264337593543950335",
            ErrorMessage = "Сумма должна быть положительна и не превышать максимально возможную")]
        public decimal Value { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }





        public virtual LoanAgreement LoanAgreement { get; set; }


        #region Constructors
        public CreditPlan()
        {
        }

        public CreditPlan(DateTime date, decimal value)
        {
            Date = date;
            Value = value;
        }

        public CreditPlan(DateTime date, decimal value, int loanLoanAgreementId)
        {
            LoanAgreementId = loanLoanAgreementId;
            Date = date;
            Value = value;
        }

        public CreditPlan(CreditPlan other)
        {
            Id = other.Id;
            LoanAgreementId = other.Id;
            Date = other.Date;
            Value = other.Value;
        }
        #endregion
    }
}