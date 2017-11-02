using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Loansv2.Validations;

namespace Loansv2.Models
{
    public class LoanAgreement
    {
        public int Id { get; set; }

        [Display(Name = "№ договора")]
        [Required(ErrorMessage = "Необходимо указать номер договора")]
        [MinLength(1, ErrorMessage = "Номер договора должен содержать хотя бы один символ")]
        [MaxLength(20, ErrorMessage = "Номер договора должен содержать не более 20 символов")]
        public string Number { get; set; }

        [ForeignKey("Creditor")]
        [Display(Name = "Займодатель")]
        [Required(ErrorMessage = "Необходимо указать займодателя")]
        public int CreditorId { get; set; }

        [ForeignKey("CreditorProject")]
        [Display(Name = "Проект займодателя")]
        [DisplayFormat(NullDisplayText = "-")]
        public int? CreditorProjectId { get; set; }

        [ForeignKey("Debtor")]
        [Display(Name = "Заёмщик")]
        [Required(ErrorMessage = "Необходимо указать заёмщика")]
        [IntNotEqual("CreditorId", ErrorMessage = "Заёмщик не может являться займодавцем")]
        public int DebtorId { get; set; }

        [ForeignKey("DebtorProject")]
        [Display(Name = "Проект заёмщика")]
        [DisplayFormat(NullDisplayText = "-")]
        public int? DebtorProjectId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Дата подписания")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [Required(ErrorMessage = "Необходимо указать дату подписания договора")]
        public DateTime SignDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Дата окончания")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [Required(ErrorMessage = "Необходимо указать дату окончания договора")]
        [DateAfter("SignDate", ErrorMessage = "Дата окончания договора должна быть позднее даты подписания")]
        public DateTime DeadlineDate { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Сумма займа")]
        [Required(ErrorMessage = "Необходимо указать максимальную сумму займа")]
        [Range(typeof(decimal), "1", "79228162514264337593543950335",
            ErrorMessage = "Сумма должна быть положительна и не превышать максимально возможную")]
        public decimal Sum { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        #region Navigation properties
        public virtual Party Creditor { get; set; }
        public virtual Party Debtor { get; set; }
        public virtual Project CreditorProject { get; set; }
        public virtual Project DebtorProject { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<CreditPlan> CreditPlans { get; set; }
        public virtual ICollection<DebtPlan> DebtPlans { get; set; }
        public virtual ICollection<AnnumRate> AnnumRates { get; set; }
        public virtual ICollection<File> Files { get; set; }
        #endregion

        #region Constructors
        public LoanAgreement()
        {
        }

        public LoanAgreement(LoanAgreement other)
        {
            Id = other.Id;
            Number = other.Number;
            CreditorId = other.CreditorId;
            CreditorProjectId = other.CreditorProjectId;
            DebtorId = other.DebtorId;
            DebtorProjectId = other.DebtorProjectId;
            SignDate = other.SignDate;
            DeadlineDate = other.DeadlineDate;
            Sum = other.Sum;
            RowVersion = RowVersion;
            Creditor = other.Creditor;
            CreditorProject = other.CreditorProject;
            Debtor = other.Debtor;
            DebtorProject = other.DebtorProject;
            AnnumRates = other.AnnumRates;
            Payments = other.Payments;
        }
        #endregion
    }
}