using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loansv2.Models
{
    public enum PartyType
    {
        [Display(Name = "ФЛ")]
        Physical,
        [Display(Name = "ИП")]
        Individual,
        [Display(Name = "ЮЛ")]
        Juristic
    }

    public class Party
    {
        public int Id { get; set; }

        [Display(Name = "Тип")]
        [Required(ErrorMessage = "Необходимо указать тип контрагента")]
        public PartyType PartyType { get; set; }

        [Display(Name = "Наименование")]
        [Required(ErrorMessage = "Необходимо указать наименование")]
        [MinLength(1, ErrorMessage = "Наименование должно содержать хотя бы один символ")]
        [MaxLength(80, ErrorMessage = "Наименование должно содержать не более 40 символов")]
        public string Name { get; set; }

        [Display(Name = "ИНН")]
        [DisplayFormat(NullDisplayText = "Неизвестен")]
        [MinLength(2, ErrorMessage = "ИНН должен содержать не менее 2 символов")]
        [MaxLength(20, ErrorMessage = "ИНН должен содержать не более 20 символов")]
        public string VatId { get; set; }

        public string DisplayTypeName()
        {
            switch (PartyType)
            {
                case PartyType.Physical:
                    return "ФЛ";
                case PartyType.Individual:
                    return "ИП";
                case PartyType.Juristic:
                    return "ЮЛ";
                default:
                    return "?";
            }
        }

        #region Constructors
        public Party()
        {
        }

        public Party(PartyType type, string vatId)
        {
            PartyType = type;
            VatId = vatId;
        }

        public Party(PartyType type, string vatId, string name) : this(type, vatId)
        {
            Name = name;
        }

        public Party(Party other)
        {
            Id = other.Id;
            PartyType = other.PartyType;
            Name = other.Name;
            VatId = other.VatId;
            Phones = other.Phones;
            Emails = other.Emails;
        }
        #endregion

        #region Navigation properties
        public virtual ICollection<Phone> Phones { get; set; }
        public virtual ICollection<Email> Emails { get; set; }
        #endregion
    }


    public class PhysicalParty
    {
        [Key]
        [ForeignKey("Party")]
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


        [Display(Name = "ФЛ")]
        public virtual Party Party { get; set; }


        #region Constructors
        public PhysicalParty()
        {
        }

        public PhysicalParty(PhysicalParty other)
        {
            Id = other.Id;
            FirstName = other.FirstName;
            MiddleName = other.MiddleName;
            LastName = other.LastName;
            RowVersion = other.RowVersion;
        }
        #endregion
    }


    public class IndividualParty
    {
        [Key]
        [ForeignKey("Party")]
        public int Id { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }


        [Display(Name = "ИП")]
        public virtual Party Party { get; set; }

        #region Constructors
        public IndividualParty()
        {
        }

        public IndividualParty(IndividualParty other)
        {
            Id = other.Id;
            RowVersion = other.RowVersion;
        }
        #endregion
    }


    public class JuristicParty
    {
        [Key]
        [ForeignKey("Party")]
        public int Id { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }


        [Display(Name = "ЮЛ")]
        public virtual Party Party { get; set; }

        #region Constructors
        public JuristicParty()
        {
        }

        public JuristicParty(JuristicParty other)
        {
            Id = other.Id;
            RowVersion = other.RowVersion;
        }
        #endregion
    }
}