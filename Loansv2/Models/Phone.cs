using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Loansv2.Models
{
    public class Phone
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Party")]
        [Display(Name = "Владелец")]
        [Required(ErrorMessage = "Необходимо указать владельца")]
        public int PartyId { get; set; }

        [Display(Name = "Телефон")]
        [Required(ErrorMessage = "Необходимо указать номер телеофна")]
        [Remote("UniquePhone", "Phone", AdditionalFields = "PartyId", ErrorMessage = "Контрагент уже обладает этим телефоном")]
        [MinLength(1, ErrorMessage = "Номер телефона должен содержать хотя бы один символ")]
        [MaxLength(20, ErrorMessage = "Номер телефона должен содержать не более 20 символов")]
        [Phone(ErrorMessage = "Невозможный телефонный номер")]
        public string Number { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }


        public virtual Party Party { get; set; }
    }
}