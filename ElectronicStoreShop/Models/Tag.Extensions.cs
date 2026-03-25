using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicStoreShop.Models
{
    public partial class Tag
    {
        public virtual ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();

    }
}
