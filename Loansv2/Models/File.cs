using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Loansv2.Models
{
    public class File
    {
        public int Id { get; set; }

        [ForeignKey("LoanAgreement")]
        [Required(ErrorMessage = "Необходимо указать договор")]
        [Display(Name = "Договор")]
        public int LoanAgreementId { get; set; }

        [StringLength(80, ErrorMessage = "Длина имени файла не должна превосходить 80 символов.")]
        [Required(ErrorMessage = "Необходимо выбрать файл")]
        [Remote("UniqueFileName", "File", ErrorMessage = "Файл с таким именем уже есть у данного договора")]
        [Display(Name = "Имя файла")]
        public string FileName { get; set; }

        [StringLength(80, ErrorMessage = "Длина описания содержимого файла не должна превосходить 80 символов.")]
        public string ContentType { get; set; }
        public byte[] Content { get; set; }

        
        public virtual LoanAgreement LoanAgreement { get; set; }
    }
}