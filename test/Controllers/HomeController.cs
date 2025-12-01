using System;
using System.Linq;
using System.Web.Mvc;
using DoAnLTWeb_CV1.Models; // Đổi tên theo Project của bạn

namespace YourProjectName.Controllers
{
    public class HomeController : Controller
    {
        NewsManagementDBEntities2 db = new NewsManagementDBEntities2();

        // Hàm giả lập User (Để test chức năng)
        private int GetCurrentUserID()
        {
            if (Session["UserID"] == null)
            {
                Session["UserID"] = 2; // Giả sử là Nguyen Van A
                Session["FullName"] = "Nguyen Van A";
            }
            return (int)Session["UserID"];
        }

        // 1. TRANG CHỦ (Hiển thị danh sách & Lọc danh mục)
        // Layout gọi Action "Search" nhưng ta sẽ tái sử dụng Logic này
        public ActionResult Index(int? categoryId)
        {
            var articles = db.Articles.Include("Categories").Include("Users")
                             .Where(a => a.Status == "Published");

            if (categoryId != null)
            {
                articles = articles.Where(x => x.CategoryID == categoryId);
            }

            ViewBag.Categories = db.Categories.ToList();
            ViewBag.CurrentCategory = categoryId; // Để giữ trạng thái dropdown
            return View(articles.OrderByDescending(a => a.CreatedAt).ToList());
        }

        // 2. TÌM KIẾM (Action này dành riêng cho Thanh tìm kiếm trên Header của Layout)
        public ActionResult Search(string q)
        {
            var articles = db.Articles.Include("Categories").Include("Users")
                             .Where(a => a.Status == "Published");

            if (!String.IsNullOrEmpty(q))
            {
                articles = articles.Where(s => s.Title.Contains(q) || s.Summary.Contains(q));
            }

            ViewBag.SearchKeyword = q;
            // Trả về View Index để hiển thị kết quả
            return View("Index", articles.OrderByDescending(a => a.CreatedAt).ToList());
        }

        // 3. CHI TIẾT & LỊCH SỬ
        public ActionResult Details(int id)
        {
            var article = db.Articles.Find(id);
            if (article == null) return HttpNotFound();

            article.ViewCount++;

            // Lưu lịch sử (Dùng User giả lập)
            int userId = GetCurrentUserID();
            var history = db.ReadingHistory.FirstOrDefault(h => h.ArticleID == id && h.UserID == userId);

            if (history == null)
            {
                ReadingHistory newH = new ReadingHistory();
                newH.UserID = userId;
                newH.ArticleID = id;
                newH.ReadAt = DateTime.Now;
                db.ReadingHistory.Add(newH);
            }
            else
            {
                history.ReadAt = DateTime.Now;
            }
            db.SaveChanges();

            return View(article);
        }

        // 4. BÌNH LUẬN
        [HttpPost]
        public ActionResult AddComment(int ArticleID, string Content, int? ParentID)
        {
            if (!string.IsNullOrWhiteSpace(Content))
            {
                Comments c = new Comments();
                c.ArticleID = ArticleID;
                c.UserID = GetCurrentUserID(); // Hàm lấy User giả lập
                c.Content = Content;
                c.ParentID = ParentID; // <--- QUAN TRỌNG: Lưu ID của bình luận cha
                c.CommentDate = DateTime.Now;
                c.Status = "Approved";

                db.Comments.Add(c);
                db.SaveChanges();
            }
            return RedirectToAction("Details", new { id = ArticleID });
        }
    }
}