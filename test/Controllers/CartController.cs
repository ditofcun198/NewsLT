using System.Linq;
using System.Web.Mvc;
using test.Models;

namespace test.Controllers
{
    public class CartController : Controller
    {
        private NewsManagementDBEntities1 db = new NewsManagementDBEntities1();
        private int CurrentUserId => 2;

        public ActionResult Index()
        {
            var svc = new CartService(db, CurrentUserId);
            var items = svc.GetItems().ToList();

            var vm = items.Select(i => new CartItemVM
            {
                CartItemID = i.CartItemID,
                ArticleID = i.ArticleID,
                Title = i.Article.Title,
                Price = i.Article.Price ?? 0
            }).ToList();

            return View(vm);
        }

        public ActionResult Add(int articleId)
        {
            var svc = new CartService(db, CurrentUserId);
            svc.AddItem(articleId);
            return RedirectToAction("Index");
        }

        public ActionResult Remove(int id)
        {
            var svc = new CartService(db, CurrentUserId);
            svc.RemoveItem(id);
            return RedirectToAction("Index");
        }

        public ActionResult Clear()
        {
            var svc = new CartService(db, CurrentUserId);
            svc.Clear();
            return RedirectToAction("Index");
        }
    }
}
