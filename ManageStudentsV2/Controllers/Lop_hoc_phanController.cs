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

    public class Lop_hoc_phanController : Controller
    {
        private Quan_Ly_Sinh_Vien_Entities db = new Quan_Ly_Sinh_Vien_Entities();

        // GET: Lop_hoc_phan
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
            var lop_hoc_phan = db.Lop_hoc_phan
                                    .Include(lhp => lhp.Lop_chinh)
                                    .Include(lhp => lhp.Mon_hoc)
                                    .Include(lhp => lhp.Giao_vien)
                                    .Select(lhp => new ManageStudentsV2.Models.LopHocPhanViewModel
                                    {
                                        MaHocPhan = lhp.ma_hoc_phan,
                                        MaLop = lhp.ma_lop,
                                        TenLop = lhp.Lop_chinh.ten_lop,
                                        MaMon = lhp.ma_mon,
                                        TenMon = lhp.Mon_hoc.ten_mon,
                                        GiaoVien = lhp.Giao_vien.FirstOrDefault().ten_giao_vien,
                                        SoSinhVien = db.Lop_dang_ky.Count(ldk => ldk.ma_hoc_phan == lhp.ma_hoc_phan),
                                        Nganh = lhp.Mon_hoc.Nganh.ten_nganh,
                                        NienKhoa = lhp.Mon_hoc.Nganh.Nien_khoa.ten_nien_khoa,
                                        Khoa = lhp.Mon_hoc.Nganh.Nien_khoa.Khoa.ten_khoa
                                    })
                                    .OrderBy(lhp => lhp.MaHocPhan);
            int pageSize = (size ?? 5);
            int pageNumber = (page ?? 1);

            return View(lop_hoc_phan.ToPagedList(pageNumber, pageSize));
        }

        // GET: Lop_hoc_phan/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lop_hoc_phan lop_hoc_phan = db.Lop_hoc_phan.Find(id);
            if (lop_hoc_phan == null)
            {
                return HttpNotFound();
            }
            return View(lop_hoc_phan);
        }

        // GET: Lop_hoc_phan/Create
        public ActionResult Create()
        {
            ViewBag.ma_lop = new SelectList(db.Lop_chinh, "ma_lop", "ten_lop");
            ViewBag.ma_mon = new SelectList(db.Mon_hoc, "ma_mon", "ten_mon");
            return View();
        }

        // POST: Lop_hoc_phan/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ma_hoc_phan,ma_lop,ma_mon")] Lop_hoc_phan lop_hoc_phan)
        {
            if (ModelState.IsValid)
            {
                db.Lop_hoc_phan.Add(lop_hoc_phan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ma_lop = new SelectList(db.Lop_chinh, "ma_lop", "ten_lop", lop_hoc_phan.ma_lop);
            ViewBag.ma_mon = new SelectList(db.Mon_hoc, "ma_mon", "ten_mon", lop_hoc_phan.ma_mon);
            return View(lop_hoc_phan);
        }

        // GET: Lop_hoc_phan/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lop_hoc_phan lop_hoc_phan = db.Lop_hoc_phan.Find(id);
            if (lop_hoc_phan == null)
            {
                return HttpNotFound();
            }
            ViewBag.ma_lop = new SelectList(db.Lop_chinh, "ma_lop", "ten_lop", lop_hoc_phan.ma_lop);
            ViewBag.ma_mon = new SelectList(db.Mon_hoc, "ma_mon", "ten_mon", lop_hoc_phan.ma_mon);
            return View(lop_hoc_phan);
        }

        // POST: Lop_hoc_phan/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ma_hoc_phan,ma_lop,ma_mon")] Lop_hoc_phan lop_hoc_phan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lop_hoc_phan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ma_lop = new SelectList(db.Lop_chinh, "ma_lop", "ten_lop", lop_hoc_phan.ma_lop);
            ViewBag.ma_mon = new SelectList(db.Mon_hoc, "ma_mon", "ten_mon", lop_hoc_phan.ma_mon);
            return View(lop_hoc_phan);
        }

        // GET: Lop_hoc_phan/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lop_hoc_phan lop_hoc_phan = db.Lop_hoc_phan.Find(id);
            if (lop_hoc_phan == null)
            {
                return HttpNotFound();
            }
            return View(lop_hoc_phan);
        }

        // POST: Lop_hoc_phan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Lop_hoc_phan lop_hoc_phan = db.Lop_hoc_phan.Find(id);
            db.Lop_hoc_phan.Remove(lop_hoc_phan);
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
