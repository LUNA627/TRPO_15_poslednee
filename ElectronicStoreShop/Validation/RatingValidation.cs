using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;


namespace ElectronicStoreShop.Validation
{
    public class RatingValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = (value ?? "").ToString().Trim();

            string normInput = input.Replace('.', ',');


            if (string.IsNullOrEmpty(input))
            {
                return new ValidationResult(false, "Введите рейтинг товара");
            }

            if (!float.TryParse(normInput, out float inputRating))
            {
                return new ValidationResult(false, "Некорректный ввод рейтинга товара");
            }

            if (inputRating < 0)
            {
                return new ValidationResult(false, "Рейтинг товара не может быть ниже 0");
            }

            if (inputRating > 5.0)
            {
                return new ValidationResult(false, "Рейтинг товара не может быть выше 5");
            }





            return ValidationResult.ValidResult;
        }
    }
}
