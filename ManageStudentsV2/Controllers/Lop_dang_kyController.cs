using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ManageStudentsV2.Models;

namespace ManageStudentsV2.Controllers
{
    [RoleAuthorize("Admin")]
    public class Lop_dang_kyController : Controller
    {
        private Quan_Ly_Sinh_Vien_Entities db = new Quan_Ly_Sinh_Vien_Entities();

        // GET: Lop_dang_ky
        public ActionResult Index()
        {
            var lop_dang_ky = db.Lop_dang_ky.Include(l => l.Hoc_sinh).Include(l => l.Lop_hoc_phan);
            return View(lop_dang_ky.ToList());
        }

        // GET: Lop_dang_ky/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lop_dang_ky lop_dang_ky = db.Lop_dang_ky.Find(id);
            if (lop_dang_ky == null)
            {
                return HttpNotFound();
            }
            return View(lop_dang_ky);
        }

        // GET: Lop_dang_ky/Create
        public ActionResult Create()
        {
            ViewBag.ma_sinh_vien = new SelectList(db.Hoc_sinh, "ma_sinh_vien", "ten_sinh_vien");
            ViewBag.ma_hoc_phan = new SelectList(db.Lop_hoc_phan, "ma_hoc_phan", "ma_hoc_phan");
            return View();
        }

        // POST: Lop_dang_ky/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ma_sinh_vien,ma_hoc_phan,ngay_dk")] Lop_dang_ky lop_dang_ky)
        {
            if (ModelState.IsValid)
            {
                db.Lop_dang_ky.Add(lop_dang_ky);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ma_sinh_vien = new SelectList(db.Hoc_sinh, "ma_sinh_vien", "ten_sinh_vien", lop_dang_ky.ma_sinh_vien);
            ViewBag.ma_hoc_phan = new SelectList(db.Lop_hoc_phan, "ma_hoc_phan", "ma_hoc_phan", lop_dang_ky.ma_hoc_phan);
            return View(lop_dang_ky);
        }

        // GET: Lop_dang_ky/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lop_dang_ky lop_dang_ky = db.Lop_dang_ky.Find(id);
            if (lop_dang_ky == null)
            {
                return HttpNotFound();
            }
            ViewBag.ma_sinh_vien = new SelectList(db.Hoc_sinh, "ma_sinh_vien", "ten_sinh_vien", lop_dang_ky.ma_sinh_vien);
            ViewBag.ma_hoc_phan = new SelectList(db.Lop_hoc_phan, "ma_hoc_phan", "ma_hoc_phan", lop_dang_ky.ma_hoc_phan);
            return View(lop_dang_ky);
        }

        // POST: Lop_dang_ky/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ma_sinh_vien,ma_hoc_phan,ngay_dk")] Lop_dang_ky lop_dang_ky)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lop_dang_ky).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ma_sinh_vien = new SelectList(db.Hoc_sinh, "ma_sinh_vien", "ten_sinh_vien", lop_dang_ky.ma_sinh_vien);
            ViewBag.ma_hoc_phan = new SelectList(db.Lop_hoc_phan, "ma_hoc_phan", "ma_hoc_phan", lop_dang_ky.ma_hoc_phan);
            return View(lop_dang_ky);
        }

        // GET: Lop_dang_ky/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lop_dang_ky lop_dang_ky = db.Lop_dang_ky.Find(id);
            if (lop_dang_ky == null)
            {
                return HttpNotFound();
            }
            return View(lop_dang_ky);
        }

        // POST: Lop_dang_ky/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Lop_dang_ky lop_dang_ky = db.Lop_dang_ky.Find(id);
            db.Lop_dang_ky.Remove(lop_dang_ky);
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
