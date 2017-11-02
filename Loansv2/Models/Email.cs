using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Loansv2.Models
{
    public class Email
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Party")]
        [Required(ErrorMessage = "Необходимо указать контрагента")]
        [Display(Name = "Контрагент")]
        public int PartyId { get; set; }

        [Required(ErrorMessage = "Необходимо указать адрес эл. почты")]
        [Display(Name = "Адрес эл. почты")]
        [Remote("UniqueEmail", "Email", AdditionalFields = "PartyId", ErrorMessage = "Контрагент уже обладает этой почтой")]
        [MinLength(6, ErrorMessage = "Адрес эл. почты должен содержать хотя бы 6 символов")]
        [MaxLength(30, ErrorMessage = "Адрес эл. почты должен содержать не более 30 символов")]
        [EmailAddress(ErrorMessage = "Невозможный адрес эл. почты")]
        public string Address { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }


        public virtual Party Party { get; set; }
    }
}