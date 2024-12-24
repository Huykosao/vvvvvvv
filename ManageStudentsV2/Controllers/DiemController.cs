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
    [RoleAuthorize("Admin", "Teacher")]

    public class DiemController : Controller
    {
        private Quan_Ly_Sinh_Vien_Entities db = new Quan_Ly_Sinh_Vien_Entities();

        // GET: Diem
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

            var diems = db.Diems.Include(d => d.Hoc_sinh).Include(d => d.Mon_hoc).OrderBy(d => d.ma_sinh_vien).ToList();
            
            int pageSize = (size ?? 5);

            int pageNumber = (page ?? 1);

            return View(diems.ToPagedList(pageNumber, pageSize));
        }
        // GET: Diem/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Diem diem = db.Diems.Find(id);
            if (diem == null)
            {
                return HttpNotFound();
            }
            return View(diem);
        }

        // GET: Diem/Create
        public ActionResult Create()
        {
            ViewBag.ma_sinh_vien = new SelectList(db.Hoc_sinh, "ma_sinh_vien", "ten_sinh_vien");
            ViewBag.ma_mon = new SelectList(db.Mon_hoc, "ma_mon", "ten_mon");
            return View();
        }

        // POST: Diem/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ma_sinh_vien,ma_mon,diem_so_1,diem_so_2,diem_cuoi_ky")] Diem diem)
        {
            if (ModelState.IsValid)
            {
                db.Diems.Add(diem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ma_sinh_vien = new SelectList(db.Hoc_sinh, "ma_sinh_vien", "ten_sinh_vien", diem.ma_sinh_vien);
            ViewBag.ma_mon = new SelectList(db.Mon_hoc, "ma_mon", "ten_mon", diem.ma_mon);
            return View(diem);
        }

        // GET: Diem/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Diem diem = db.Diems.Find(id);
            if (diem == null)
            {
                return HttpNotFound();
            }
            ViewBag.ma_sinh_vien = new SelectList(db.Hoc_sinh, "ma_sinh_vien", "ten_sinh_vien", diem.ma_sinh_vien);
            ViewBag.ma_mon = new SelectList(db.Mon_hoc, "ma_mon", "ten_mon", diem.ma_mon);
            return View(diem);
        }

        // POST: Diem/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ma_sinh_vien,ma_mon,diem_so_1,diem_so_2,diem_cuoi_ky")] Diem diem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(diem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ma_sinh_vien = new SelectList(db.Hoc_sinh, "ma_sinh_vien", "ten_sinh_vien", diem.ma_sinh_vien);
            ViewBag.ma_mon = new SelectList(db.Mon_hoc, "ma_mon", "ten_mon", diem.ma_mon);
            return View(diem);
        }

        // GET: Diem/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Diem diem = db.Diems.Find(id);
            if (diem == null)
            {
                return HttpNotFound();
            }
            return View(diem);
        }

        // POST: Diem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Diem diem = db.Diems.Find(id);
            db.Diems.Remove(diem);
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
