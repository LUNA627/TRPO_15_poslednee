using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ElectronicStoreShop.Validation
{
    class EmptyFieldValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = (value ?? "").ToString().Trim();

            if (string.IsNullOrEmpty(input))
            {
                return new ValidationResult(false, "Поле должно быть заполнено");
            }

            if (input.Length > 10000)
            {
                return new ValidationResult(false, "Количество текста не должно привышать 10000 символов");
            }


            return ValidationResult.ValidResult;
        }
    }
}
