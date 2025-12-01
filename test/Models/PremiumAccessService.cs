using System;
using System.Linq;

namespace test.Models
{
    public class PremiumAccessService
    {
        private readonly NewsManagementDBEntities1 _db;

        public PremiumAccessService(NewsManagementDBEntities1 db)
        {
            _db = db;
        }

        public bool HasAccess(int userId, int articleId)
        {
            var user = _db.Users.Find(userId);
            if (user != null && user.IsPremium == true)
                return true;

            bool bought = _db.UserArticleAccesses
                .Any(x => x.UserID == userId && x.ArticleID == articleId);

            if (bought) return true;

            var now = DateTime.Now;

            bool vip = _db.MembershipSubscriptions
                .Any(x => x.UserID == userId && x.IsActive == true && x.EndDate >= now);

            return vip;
        }

        public void GrantAccessFromOrder(int userId, Order order)
        {
            var list = _db.OrderItems.Where(x => x.OrderID == order.OrderID).ToList();

            foreach (var i in list)
            {
                bool existed = _db.UserArticleAccesses
                    .Any(x => x.UserID == userId && x.ArticleID == i.ArticleID);

                if (!existed)
                {
                    _db.UserArticleAccesses.Add(new UserArticleAccess
                    {
                        UserID = userId,
                        ArticleID = i.ArticleID,
                        AccessDate = DateTime.Now
                    });
                }
            }

            _db.SaveChanges();
        }
    }
}
