using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicStoreShop.Models
{
    public partial class Product
    {
        public virtual ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();

        [NotMapped]
        public string DisplayTags
        {
            get
            {
                if (ProductTags == null || !ProductTags.Any())
                    return string.Empty;

                return string.Join(" ",
                    ProductTags
                        .Where(pt => pt.Tag != null)
                        .Select(pt => $"#{pt.Tag.Name}"));
            }
        }

    }
}
