using System;
using System.Linq;

namespace test.Models
{
    public class CartService
    {
        private readonly NewsManagementDBEntities1 _db;
        private readonly int _userId;

        public CartService(NewsManagementDBEntities1 db, int userId)
        {
            _db = db;
            _userId = userId;
        }

        public Cart GetOrCreateCart()
        {
            var cart = _db.Carts.FirstOrDefault(c => c.UserID == _userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserID = _userId,
                    CreatedAt = DateTime.Now
                };
                _db.Carts.Add(cart);
                _db.SaveChanges();
            }

            return cart;
        }

        public IQueryable<CartItem> GetItems()
        {
            var cart = GetOrCreateCart();
            return _db.CartItems.Where(x => x.CartID == cart.CartID);
        }

        public void AddItem(int articleId)
        {
            var cart = GetOrCreateCart();
            var article = _db.Articles.Find(articleId);

            if (article == null || article.IsPremium == false)
                throw new Exception("Không thể thêm bài không Premium.");

            var exist = _db.CartItems.FirstOrDefault(
                x => x.CartID == cart.CartID && x.ArticleID == articleId
            );

            if (exist == null)
            {
                _db.CartItems.Add(new CartItem
                {
                    CartID = cart.CartID,
                    ArticleID = articleId,
                    Quantity = 1
                });

                _db.SaveChanges();
            }
        }

        public void RemoveItem(int id)
        {
            var item = _db.CartItems.Find(id);
            if (item != null)
            {
                _db.CartItems.Remove(item);
                _db.SaveChanges();
            }
        }

        public void Clear()
        {
            var items = GetItems().ToList();
            _db.CartItems.RemoveRange(items);
            _db.SaveChanges();
        }
    }
}
