using MaterialDesignThemes.Wpf.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ElectronicStoreShop.Validation
{
    public class PriceValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = (value ?? "").ToString().Trim();

            if (input == string.Empty)
            {
                return new ValidationResult(false, "Введите цену товара");
            }

            if (!decimal.TryParse(input, out decimal inputPrice))
            {
                return new ValidationResult(false, "Некорректный ввод числа");
            }

            if (inputPrice < 0)
            {
                return new ValidationResult(false, "Цена не может быть меньше 0");
            }

            if (inputPrice > 1000000000)
            {
                return new ValidationResult(false, "Цена не может быть больше 1 000 000 000");
            }


            return ValidationResult.ValidResult;
        }
    }
}
