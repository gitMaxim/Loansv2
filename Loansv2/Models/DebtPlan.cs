using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Loansv2.Models
{
    public class DebtPlan : IDateDecimal
    {
        public int Id { get; set; }

        [ForeignKey("LoanAgreement")]
        public int LoanAgreementId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Дата")]
        [Remote("ValidateDebtDate", "DebtPlan", AdditionalFields = "LoanAgreementId")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [Required(ErrorMessage = "Необходимо указать дату")]
        public DateTime Date { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Сумма к погашению")]
        [Remote("ValidateDebtSum", "DebtPlan", AdditionalFields = "LoanAgreementId")]
        [Required(ErrorMessage = "Необходимо указать сумму погашения займа")]
        [Range(typeof(decimal), "1", "79228162514264337593543950335",
            ErrorMessage = "Сумма должна быть положительна и не превышать максимально возможную")]
        public decimal Value { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }


        public virtual LoanAgreement LoanAgreement { get; set; }

        #region Constructors
        public DebtPlan()
        {
        }

        public DebtPlan(DateTime date, decimal value)
        {
            Date = date;
            Value = value;
        }

        public DebtPlan(DateTime date, decimal value, int loanLoanAgreementId)
        {
            LoanAgreementId = loanLoanAgreementId;
            Date = date;
            Value = value;
        }

        public DebtPlan(DebtPlan other)
        {
            Id = other.Id;
            LoanAgreementId = other.Id;
            Date = other.Date;
            Value = other.Value;
        }
        #endregion
    }
}