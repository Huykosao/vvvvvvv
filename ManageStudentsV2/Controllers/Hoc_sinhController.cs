using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using ManageStudentsV2.Models;

namespace ManageStudentsV2.Controllers
{
    [RoleAuthorize("Admin")]
   
    public class Hoc_sinhController : Controller
    {
        private Quan_Ly_Sinh_Vien_Entities db = new Quan_Ly_Sinh_Vien_Entities();

        // GET: Hoc_sinh
        public ActionResult Index(int? size, int? page)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "5", Value = "5" });
            items.Add(new SelectListItem { Text = "10", Value = "10" });
            items.Add(new SelectListItem { Text = "20", Value = "20" });
            items.Add(new SelectListItem { Text = "25", Value = "25" });
            items.Add(new SelectListItem { Text = "50", Value = "50" });
            items.Add(new SelectListItem { Text = "100", Value = "100" });
            items.Add(new SelectListItem { Text = "200", Value = "200" });
            foreach (var item in items)
            {
                if (item.Value == size.ToString()) item.Selected = true;
            }
            ViewBag.size = items; // ViewBag DropDownList
            ViewBag.currentSize = size; // tạo biến kích thước trang hiện tại
            page = page ?? 1; //if (page == null) page = 1;
            var hoc_sinh = db.Hoc_sinh
                                .Include(h => h.Lop_chinh.Nganh.Nien_khoa.Khoa) // Gồm các liên kết bảng
                                .Include(h => h.User) // Bao gồm thông tin tài khoản người dùng
                                .OrderBy(h => h.ten_sinh_vien) // Thêm sắp xếp để đảm bảo thứ tự nhất quán
                                .ToList();
            int pageSize = (size ?? 5);
            
            int pageNumber = (page ?? 1);

            return View(hoc_sinh.ToPagedList(pageNumber, pageSize));

            //var Hoc_sinh = from hs in db.Hoc_sinh
            //               join lh in db.Lop_chinh on hs.ma_lop equals lh.ma_lop
            //               join nganh in db.Nganhs on lh.ma_nganh equals nganh.ma_nganh
            //               join nien_khoa in db.Nien_khoa on nganh.ma_nien_khoa equals nien_khoa.ma_nien_khoa
            //               join khoa in db.Khoas on nien_khoa.ma_khoa equals khoa.ma_khoa
            //               select new
            //               {
            //                   HocSinh = hs,
            //                   Lop_chinh = lh,
            //                   Nganh = nganh,
            //                   Nien_khoa = nien_khoa,
            //                   Khoa = khoa,
            //               };
        }

        // GET: Hoc_sinh/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var result = db.Hoc_sinh
                    .Include(h => h.Lop_chinh.Nganh.Nien_khoa.Khoa) // Nạp các bảng liên quan
                    .FirstOrDefault(h => h.ma_sinh_vien == id);

            if (result == null)
            {
                return HttpNotFound();
            }
            return View(result);
        }

        // GET: Hoc_sinh/Create
        public ActionResult Create()
        {
            ViewBag.ma_lop = new SelectList(db.Lop_chinh, "ma_lop", "ten_lop");
            var usersNotReferenced = db.Users
                                      .Where(user => !db.Hoc_sinh.Any(hs => hs.UserID == user.ID))
                                      .Where(user => !db.Giao_vien.Any(gv => gv.UserID == user.ID))
                                      .ToList();

            // Truyền vào ViewBag cho dropdown list
            ViewBag.UserID = new SelectList(usersNotReferenced, "ID", "Username");
            return View();
        }

        // POST: Hoc_sinh/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ma_sinh_vien,ten_sinh_vien,ma_lop,UserID")] Hoc_sinh hoc_sinh)
        {
            if (ModelState.IsValid)
            {
                db.Hoc_sinh.Add(hoc_sinh);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ma_lop = new SelectList(db.Lop_chinh, "ma_lop", "ten_lop", hoc_sinh.ma_lop);
            ViewBag.UserID = new SelectList(db.Users, "ID", "Username", hoc_sinh.UserID);
            return View(hoc_sinh);
        }

        // GET: Hoc_sinh/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hoc_sinh hoc_sinh = db.Hoc_sinh.Find(id);
            if (hoc_sinh == null)
            {
                return HttpNotFound();
            }
            ViewBag.ma_lop = new SelectList(db.Lop_chinh, "ma_lop", "ten_lop", hoc_sinh.ma_lop);
            ViewBag.UserID = new SelectList(db.Users, "ID", "Username", hoc_sinh.UserID);
            return View(hoc_sinh);
        }

        // POST: Hoc_sinh/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ma_sinh_vien,ten_sinh_vien,ma_lop,UserID")] Hoc_sinh hoc_sinh)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hoc_sinh).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ma_lop = new SelectList(db.Lop_chinh, "ma_lop", "ten_lop", hoc_sinh.ma_lop);
            ViewBag.UserID = new SelectList(db.Users, "ID", "Username", hoc_sinh.UserID);
            return View(hoc_sinh);
        }

        // GET: Hoc_sinh/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hoc_sinh hoc_sinh = db.Hoc_sinh.Find(id);
            if (hoc_sinh == null)
            {
                return HttpNotFound();
            }
            return View(hoc_sinh);
        }

        // POST: Hoc_sinh/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Hoc_sinh hoc_sinh = db.Hoc_sinh.Find(id);
            db.Hoc_sinh.Remove(hoc_sinh);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
