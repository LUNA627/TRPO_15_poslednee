using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ElectronicStoreShop.Validation
{
    public class StrockValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = (value ?? "").ToString().Trim();

            if (string.IsNullOrEmpty(input))
            {
                return new ValidationResult(false, "Введите остаток товара");
            }

            if (!int.TryParse(input, out int inputStrock))
            {
                return new ValidationResult(false, "Некорректный ввод остатка товара");
            }

            if (inputStrock < 0)
            {
                return new ValidationResult(false, "Остаток товара должен быть больше 0");
            }

            if (inputStrock > 1000000)
            {
                return new ValidationResult(false, "Остаток товара должен быть больше 1 000 000");
            }


            return ValidationResult.ValidResult;
        }

    }
}
