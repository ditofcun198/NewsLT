using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test.Models
{
    public class CartItemVM
    {
        public int CartItemID { get; set; }
        public int ArticleID { get; set; }

        public string Title { get; set; }
        public decimal Price { get; set; }
    }
}
