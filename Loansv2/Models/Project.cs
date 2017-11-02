using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Loansv2.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Display(Name = "Проект")]
        [Required(ErrorMessage = "Необходимо ввести навзвание проекта")]
        [Remote("UniqueProjectName", "Project", ErrorMessage = "Название проекта уже есть в базе")]
        [MinLength(1, ErrorMessage = "Название проекта должно содержать хотя бы один символ")]
        [MaxLength(20, ErrorMessage = "Название проекта должно содержать не более 20 символов")]
        public string Name { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }


        #region Constructors
        public Project()
        {
        }

        public Project(Project other)
        {
            Id = other.Id;
            Name = other.Name;
            RowVersion = RowVersion;
        }
        #endregion
    }
}