using System;
using System.Globalization;
using System.Web.Mvc;

namespace Loansv2
{
    public class DecimalModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext,
            ModelBindingContext bindingContext)
        {
            ValueProviderResult valueResult = bindingContext.ValueProvider
                .GetValue(bindingContext.ModelName);
            ModelState modelState = new ModelState { Value = valueResult };
            object actualValue = null;
            try
            {
                //if with period use InvariantCulture
                if (valueResult.AttemptedValue.Contains("."))
                {
                    actualValue = Convert.ToDecimal(valueResult.AttemptedValue,
                        CultureInfo.InvariantCulture);
                }
                else
                {
                    //if with comma use CurrentCulture
                    actualValue = Convert.ToDecimal(valueResult.AttemptedValue,
                        CultureInfo.CurrentCulture);
                }

            }
            catch (FormatException e)
            {
                modelState.Errors.Add(e);
            }

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
            return actualValue;
        }
    }
}