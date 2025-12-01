using System;
using System.Linq;
using System.Web.Mvc;
using test.Models;

namespace test.Controllers
{
    public class OrderController : Controller
    {
        private NewsManagementDBEntities1 db = new NewsManagementDBEntities1();
        private int CurrentUserId => 2;

        public ActionResult Checkout()
        {
            var cartSvc = new CartService(db, CurrentUserId);
            var items = cartSvc.GetItems().ToList();

            if (!items.Any())
                return RedirectToAction("Index", "Cart");

            var vm = new CheckoutVM
            {
                Items = items.Select(i => new CartItemVM
                {
                    CartItemID = i.CartItemID,
                    ArticleID = i.ArticleID,
                    Title = i.Article.Title,
                    Price = i.Article.Price ?? 0
                }).ToList()
            };

            vm.TotalAmount = vm.Items.Sum(x => x.Price);

            return View(vm);
        }

        [HttpPost]
        public ActionResult Checkout(CheckoutVM model)
        {
            var cartSvc = new CartService(db, CurrentUserId);
            var items = cartSvc.GetItems().ToList();

            if (!items.Any())
                return RedirectToAction("Index", "Cart");

            decimal total = items.Sum(x => x.Article.Price ?? 0);

            var order = new Order
            {
                UserID = CurrentUserId,
                TotalAmount = total,
                PaymentMethod = model.PaymentMethod,
                PaymentStatus = "Completed",
                CreatedAt = DateTime.Now
            };

            db.Orders.Add(order);
            db.SaveChanges();

            foreach (var i in items)
            {
                db.OrderItems.Add(new OrderItem
                {
                    OrderID = order.OrderID,
                    ArticleID = i.ArticleID,
                    Price = i.Article.Price ?? 0
                });
            }
            db.SaveChanges();

            var premium = new PremiumAccessService(db);
            premium.GrantAccessFromOrder(CurrentUserId, order);

            cartSvc.Clear();

            return RedirectToAction("Success", new { id = order.OrderID });
        }

        public ActionResult Success(int id)
        {
            var order = db.Orders.Find(id);
            if (order == null)
                return HttpNotFound();

            return View(order);
        }
    }
}
