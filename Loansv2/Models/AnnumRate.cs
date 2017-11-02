using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Loansv2.Models
{
    public class AnnumRate : IDateDecimal
    {
        public int Id { get; set; }

        [ForeignKey("LoanAgreement")]
        public int LoanAgreementId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "C даты")]
        [Remote("ValidateRateDate", "AnnumRate", AdditionalFields = "LoanAgreementId")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [Required(ErrorMessage = "Необходимо указать дату")]
        public DateTime Date { get; set; }

        [Display(Name = "Процентная ставка")]
        [Required(ErrorMessage = "Необходимо указать ставку")]
        [Range(typeof(decimal), "0", "79228162514264337593543950335",
            ErrorMessage = "Ставка должна быть неотрицательной и не превышать максимально возможную")]
        public decimal Value { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }


        public virtual LoanAgreement LoanAgreement { get; set; }


        #region Constructors
        public AnnumRate()
        {
        }

        public AnnumRate(DateTime date, decimal value)
        {
            Date = date;
            Value = value;
        }

        public AnnumRate(DateTime date, decimal value, int loanLoanAgreementId)
        {
            LoanAgreementId = loanLoanAgreementId;
            Date = date;
            Value = value;
        }

        public AnnumRate(AnnumRate other)
        {
            Id = other.Id;
            LoanAgreementId = other.Id;
            Date = other.Date;
            Value = other.Value;
        }
        #endregion
    }
}