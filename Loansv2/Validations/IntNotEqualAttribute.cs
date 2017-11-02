using System.ComponentModel.DataAnnotations;

namespace Loansv2.Validations
{
    public class IntNotEqualAttribute : ValidationAttribute
    {
        private readonly string _numberName;


        public IntNotEqualAttribute(string numberName)
            : base("{0} не должно совпадать с указанным значением")
        {
            _numberName = numberName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyInfo = validationContext.ObjectType.GetProperty(_numberName);
            if (propertyInfo == null)
                return new ValidationResult(string.Format("Неивзестное свойство {0}", _numberName));

            var number = propertyInfo.GetValue(validationContext.ObjectInstance, null);
            if (number == null)
                return ValidationResult.Success;

            var errorMessage = FormatErrorMessage(validationContext.DisplayName);

            if (!(number is int))
                return new ValidationResult(errorMessage);

            var currValue = (int) value;
            if (currValue.CompareTo(number) == 0)
                return new ValidationResult(errorMessage);

            return ValidationResult.Success;
        }
    }
}