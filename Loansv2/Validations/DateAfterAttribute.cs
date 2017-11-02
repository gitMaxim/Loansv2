using System;
using System.ComponentModel.DataAnnotations;

namespace Loansv2.Validations
{
    public class DateAfterAttribute : ValidationAttribute
    {
        private readonly string _dateName;


        public DateAfterAttribute(string dateName)
            : base("{0} должна превышать указанное значение")
        {
            _dateName = dateName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyInfo = validationContext.ObjectType.GetProperty(_dateName);
            if (propertyInfo == null)
                return new ValidationResult($"Неивзестное свойство {_dateName}");

            var date = propertyInfo.GetValue(validationContext.ObjectInstance, null);
            if (date == null)
                return ValidationResult.Success;

            if (!(value is DateTime))
                return new ValidationResult("Формат даты должен быть: дд.мм.гггг");

            var errorMessage = FormatErrorMessage(validationContext.DisplayName);

            var currDate = (DateTime) value;
            if (currDate.CompareTo(date) <= 0)
                return new ValidationResult(errorMessage);

            return ValidationResult.Success;
        }
    }
}