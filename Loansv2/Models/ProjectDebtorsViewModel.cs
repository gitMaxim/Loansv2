using System;
using System.ComponentModel.DataAnnotations;

namespace Loansv2.Models
{
    public class ProjectDebtorsViewModel
    {
        public int Id { get; set; }

        [Display(Name = "№ договора")]
        public string Number { get; set; }

        [Display(Name = "Займодатель")]
        public string Creditor { get; set; }

        [Display(Name = "Проект займодателя")]
        [DisplayFormat(NullDisplayText = "-")]
        public string CreditorProject { get; set; }

        [Display(Name = "Заёмщик")]
        public string Debtor { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Дата подписания")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime SignDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Дата окончания")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime DeadlineDate { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Погашенно займа")]
        public decimal RepayedSum { get; set; }

        [DataType(DataType.Currency)]
        public decimal Sum { get; set; }
    }
}