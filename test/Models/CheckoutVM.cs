using System.Collections.Generic;

namespace test.Models
{
    public class CheckoutVM
    {
        public List<CartItemVM> Items { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
    }
}
