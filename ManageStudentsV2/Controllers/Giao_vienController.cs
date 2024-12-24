using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ManageStudentsV2.Models;
using PagedList;

namespace ManageStudentsV2.Controllers
{
    [RoleAuthorize("Admin")]

    public class Giao_vienController : Controller
    {
        private Quan_Ly_Sinh_Vien_Entities db = new Quan_Ly_Sinh_Vien_Entities();

        // GET: Giao_vien
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
            var giao_vien = db.Giao_vien
                                .Include(g => g.Khoa)
                                .Include(g => g.User)
                                .OrderBy(g => g.ten_giao_vien)
                                .ToList();
            int pageSize = (size ?? 5);
            int pageNumber = (page ?? 1);

            return View(giao_vien.ToPagedList(pageNumber, pageSize));
        }

        // GET: Giao_vien/Details/5

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Giao_vien giao_vien = db.Giao_vien.Find(id);
            if (giao_vien == null)
            {
                return HttpNotFound();
            }
            return View(giao_vien);
        }

        // GET: Giao_vien/Create

        public ActionResult Create()
        {
            ViewBag.ma_khoa = new SelectList(db.Khoas, "ma_khoa", "ten_khoa");
            ViewBag.UserID = new SelectList(db.Users, "ID", "Username");
            return View();
        }

        // POST: Giao_vien/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Create([Bind(Include = "ma_giao_vien,ten_giao_vien,ma_khoa,UserID")] Giao_vien giao_vien)
        {
            if (ModelState.IsValid)
            {
                db.Giao_vien.Add(giao_vien);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ma_khoa = new SelectList(db.Khoas, "ma_khoa", "ten_khoa", giao_vien.ma_khoa);
            ViewBag.UserID = new SelectList(db.Users, "ID", "Username", giao_vien.UserID);
            return View(giao_vien);
        }

        // GET: Giao_vien/Edit/5

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Giao_vien giao_vien = db.Giao_vien.Find(id);
            if (giao_vien == null)
            {
                return HttpNotFound();
            }
            ViewBag.ma_khoa = new SelectList(db.Khoas, "ma_khoa", "ten_khoa", giao_vien.ma_khoa);
            ViewBag.UserID = new SelectList(db.Users, "ID", "Username", giao_vien.UserID);
            return View(giao_vien);
        }

        // POST: Giao_vien/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Edit([Bind(Include = "ma_giao_vien,ten_giao_vien,ma_khoa,UserID")] Giao_vien giao_vien)
        {
            if (ModelState.IsValid)
            {
                db.Entry(giao_vien).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ma_khoa = new SelectList(db.Khoas, "ma_khoa", "ten_khoa", giao_vien.ma_khoa);
            ViewBag.UserID = new SelectList(db.Users, "ID", "Username", giao_vien.UserID);
            return View(giao_vien);
        }

        // GET: Giao_vien/Delete/5

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Giao_vien giao_vien = db.Giao_vien.Find(id);
            if (giao_vien == null)
            {
                return HttpNotFound();
            }
            return View(giao_vien);
        }

        // POST: Giao_vien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        

        public ActionResult DeleteConfirmed(int id)
        {
            Giao_vien giao_vien = db.Giao_vien.Find(id);
            db.Giao_vien.Remove(giao_vien);
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
