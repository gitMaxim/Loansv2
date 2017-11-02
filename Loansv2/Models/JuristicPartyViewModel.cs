using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Loansv2.Models
{
    public class JuristicPartyViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Наименование")]
        [Required(ErrorMessage = "Необходимо указать наименование")]
        [Remote("UniqueName", "JuristicParty", ErrorMessage = "Такое имя уже есть в базе")]
        [MinLength(1, ErrorMessage = "Наименование должно содержать хотя бы один символ")]
        [MaxLength(80, ErrorMessage = "Наименование должно содержать не более 80 символов")]
        public string Name { get; set; }

        [Display(Name = "ИНН")]
        [DisplayFormat(NullDisplayText = "-")]
        [MinLength(2, ErrorMessage = "ИНН должен содержать не менее 2")]
        [MaxLength(20, ErrorMessage = "ИНН должен содержать не более 20 символов")]
        public string VatId { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}