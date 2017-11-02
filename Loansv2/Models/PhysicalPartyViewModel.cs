using System.ComponentModel.DataAnnotations;

namespace Loansv2.Models
{
    public class PhysicalPartyViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Имя")]
        [Required(ErrorMessage = "Необходимо указать имя")]
        [MinLength(1, ErrorMessage = "Имя должно содержать не менее 1 символа")]
        [MaxLength(25, ErrorMessage = "Имя должно содержать не более 25 символов")]
        public string FirstName { get; set; }

        [Display(Name = "Отчество")]
        [MinLength(1, ErrorMessage = "Отчество должно содержать не менее 1 символа")]
        [MaxLength(25, ErrorMessage = "Отчество должно содержать не более 25 символов")]
        public string MiddleName { get; set; }

        [Display(Name = "Фамилия")]
        [Required(ErrorMessage = "Необходимо указать фамилию")]
        [MinLength(1, ErrorMessage = "Фамилия должна содержать не менее 1 символа")]
        [MaxLength(25, ErrorMessage = "Фамилия должна содержать не более 25 символов")]
        public string LastName { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        [Display(Name = "ИНН")]
        [DisplayFormat(NullDisplayText = "Неизвестен")]
        [MinLength(2, ErrorMessage = "ИНН должен содержать не менее 2 символов")]
        [MaxLength(20, ErrorMessage = "ИНН должен содержать не более 20 символов")]
        public string VatId { get; set; }


        #region Constructors
        public PhysicalPartyViewModel()
        {
        }

        public PhysicalPartyViewModel(PhysicalParty party)
        {
            if (party.Party != null)
                VatId = party.Party.VatId;
        }
        #endregion


        public string ShortName => FirstName == null || LastName == null
            ? null
            : (MiddleName != null
                ? $"{LastName} {FirstName[0]}. {MiddleName[0]}."
                : $"{LastName} {FirstName[0]}.");
    }
}