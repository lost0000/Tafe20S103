using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace StartFinance.Models
{
    public class ShoppingDetails
    {
        [Unique, PrimaryKey]
        public string ShoppingItemID { get; set; }

        [NotNull]
        public String ShopName { get; set;}

        [NotNull]
        public String ItemName { get; set; }

        [NotNull]
        public String ShoppingDate { get; set; }

        [NotNull]
        public double QuotedPrice { get; set; }
    }
}
