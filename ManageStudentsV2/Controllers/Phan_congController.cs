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
    public class Phan_congController : Controller
    {
        private Quan_Ly_Sinh_Vien_Entities db = new Quan_Ly_Sinh_Vien_Entities();

        // GET: Phan_cong
        public ActionResult Index()
        {
            var phan_cong = db.Phan_cong.Include(p => p.Giao_vien).Include(p => p.Lop_hoc_phan);
            return View(phan_cong.ToList());
        }

        // GET: Phan_cong/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Phan_cong phan_cong = db.Phan_cong.Find(id);
            if (phan_cong == null)
            {
                return HttpNotFound();
            }
            return View(phan_cong);
        }

        // GET: Phan_cong/Create
        public ActionResult Create()
        {
            ViewBag.ma_giao_vien = new SelectList(db.Giao_vien, "ma_giao_vien", "ten_giao_vien");
            ViewBag.ma_hoc_phan = new SelectList(db.Lop_hoc_phan, "ma_hoc_phan", "ma_hoc_phan");
            return View();
        }

        // POST: Phan_cong/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ma_giao_vien,ma_hoc_phan,ngay_bat_dau,ngay_ket_thuc")] Phan_cong phan_cong)
        {
            if (ModelState.IsValid)
            {
                db.Phan_cong.Add(phan_cong);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ma_giao_vien = new SelectList(db.Giao_vien, "ma_giao_vien", "ten_giao_vien", phan_cong.ma_giao_vien);
            ViewBag.ma_hoc_phan = new SelectList(db.Lop_hoc_phan, "ma_hoc_phan", "ma_hoc_phan", phan_cong.ma_hoc_phan);
            return View(phan_cong);
        }

        // GET: Phan_cong/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Phan_cong phan_cong = db.Phan_cong.Find(id);
            if (phan_cong == null)
            {
                return HttpNotFound();
            }
            ViewBag.ma_giao_vien = new SelectList(db.Giao_vien, "ma_giao_vien", "ten_giao_vien", phan_cong.ma_giao_vien);
            ViewBag.ma_hoc_phan = new SelectList(db.Lop_hoc_phan, "ma_hoc_phan", "ma_hoc_phan", phan_cong.ma_hoc_phan);
            return View(phan_cong);
        }

        // POST: Phan_cong/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ma_giao_vien,ma_hoc_phan,ngay_bat_dau,ngay_ket_thuc")] Phan_cong phan_cong)
        {
            if (ModelState.IsValid)
            {
                db.Entry(phan_cong).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ma_giao_vien = new SelectList(db.Giao_vien, "ma_giao_vien", "ten_giao_vien", phan_cong.ma_giao_vien);
            ViewBag.ma_hoc_phan = new SelectList(db.Lop_hoc_phan, "ma_hoc_phan", "ma_hoc_phan", phan_cong.ma_hoc_phan);
            return View(phan_cong);
        }

        // GET: Phan_cong/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Phan_cong phan_cong = db.Phan_cong.Find(id);
            if (phan_cong == null)
            {
                return HttpNotFound();
            }
            return View(phan_cong);
        }

        // POST: Phan_cong/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Phan_cong phan_cong = db.Phan_cong.Find(id);
            db.Phan_cong.Remove(phan_cong);
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
