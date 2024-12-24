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
    public class NganhController : Controller
    {
        private Quan_Ly_Sinh_Vien_Entities db = new Quan_Ly_Sinh_Vien_Entities();

        // GET: Nganh
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
            page = page ?? 1;

            var nganhs = db.Nganhs.Include(n => n.Nien_khoa).OrderBy(n => n.ma_nganh);

            int pageSize = (size ?? 5);
            int pageNumber = (page ?? 1);

            return View(nganhs.ToPagedList(pageNumber, pageSize));
        }

        // GET: Nganh/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nganh nganh = db.Nganhs.Find(id);
            if (nganh == null)
            {
                return HttpNotFound();
            }
            return View(nganh);
        }

        // GET: Nganh/Create
        public ActionResult Create()
        {
            ViewBag.ma_nien_khoa = new SelectList(db.Nien_khoa, "ma_nien_khoa", "ten_nien_khoa");
            return View();
        }

        // POST: Nganh/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ma_nganh,ten_nganh,mo_ta_nganh,ma_nien_khoa")] Nganh nganh)
        {
            if (ModelState.IsValid)
            {
                db.Nganhs.Add(nganh);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ma_nien_khoa = new SelectList(db.Nien_khoa, "ma_nien_khoa", "ten_nien_khoa", nganh.ma_nien_khoa);
            return View(nganh);
        }

        // GET: Nganh/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nganh nganh = db.Nganhs.Find(id);
            if (nganh == null)
            {
                return HttpNotFound();
            }
            ViewBag.ma_nien_khoa = new SelectList(db.Nien_khoa, "ma_nien_khoa", "ten_nien_khoa", nganh.ma_nien_khoa);
            return View(nganh);
        }

        // POST: Nganh/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ma_nganh,ten_nganh,mo_ta_nganh,ma_nien_khoa")] Nganh nganh)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nganh).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ma_nien_khoa = new SelectList(db.Nien_khoa, "ma_nien_khoa", "ten_nien_khoa", nganh.ma_nien_khoa);
            return View(nganh);
        }

        // GET: Nganh/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nganh nganh = db.Nganhs.Find(id);
            if (nganh == null)
            {
                return HttpNotFound();
            }
            return View(nganh);
        }

        // POST: Nganh/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Nganh nganh = db.Nganhs.Find(id);
            db.Nganhs.Remove(nganh);
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
