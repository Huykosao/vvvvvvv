using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ManageStudentsV2.Models;
using PagedList;


namespace ManageStudentsV2.Controllers
{
    [RoleAuthorize("Admin")]

    public class Lop_chinhController : Controller
    {
        private Quan_Ly_Sinh_Vien_Entities db = new Quan_Ly_Sinh_Vien_Entities();

        // GET: Lop_chinh
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
            var lop_chinh = db.Lop_chinh
                                    .Include(l => l.Giao_vien)
                                    .Include(l => l.Nganh.Nien_khoa.Khoa)
                                    .Select(l => new ManageStudentsV2.Models.LopChinhViewModel 
                                    {
                                        MaLop = l.ma_lop,
                                        TenLop = l.ten_lop,
                                        SoLuongHocSinh = db.Hoc_sinh.Count(hs => hs.ma_lop == l.ma_lop),
                                        GiaoVien = l.Giao_vien.ten_giao_vien,
                                        Nganh = l.Nganh.ten_nganh,
                                        NienKhoa = l.Nganh.Nien_khoa.ten_nien_khoa,
                                        Khoa = l.Nganh.Nien_khoa.Khoa.ten_khoa
                                    }).ToList();

            int pageSize = (size ?? 5);
            int pageNumber = (page ?? 1);

            return View(lop_chinh.ToPagedList(pageNumber, pageSize));
        }

        // GET: Lop_chinh/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lop_chinh lop_chinh = db.Lop_chinh.Find(id);
            if (lop_chinh == null)
            {
                return HttpNotFound();
            }
            return View(lop_chinh);
        }

        // GET: Lop_chinh/Create
        public ActionResult Create()
        {
            ViewBag.giao_vien_chu_nhiem = new SelectList(db.Giao_vien, "ma_giao_vien", "ten_giao_vien");
            ViewBag.ma_nganh = new SelectList(db.Nganhs, "ma_nganh", "ten_nganh");
            return View();
        }

        // POST: Lop_chinh/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ten_lop,giao_vien_chu_nhiem,ma_nganh")] Lop_chinh lop_chinh)
        {
            if (ModelState.IsValid)
            {
                db.Lop_chinh.Add(lop_chinh);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.giao_vien_chu_nhiem = new SelectList(db.Giao_vien, "ma_giao_vien", "ten_giao_vien", lop_chinh.giao_vien_chu_nhiem);
            ViewBag.ma_nganh = new SelectList(db.Nganhs, "ma_nganh", "ten_nganh", lop_chinh.ma_nganh);
            return View(lop_chinh);
        }

        // GET: Lop_chinh/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lop_chinh lop_chinh = db.Lop_chinh.Find(id);
            if (lop_chinh == null)
            {
                return HttpNotFound();
            }
            ViewBag.giao_vien_chu_nhiem = new SelectList(db.Giao_vien, "ma_giao_vien", "ten_giao_vien", lop_chinh.giao_vien_chu_nhiem);
            ViewBag.ma_nganh = new SelectList(db.Nganhs, "ma_nganh", "ten_nganh", lop_chinh.ma_nganh);
            return View(lop_chinh);
        }

        // POST: Lop_chinh/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ma_lop,ten_lop,giao_vien_chu_nhiem,ma_nganh")] Lop_chinh lop_chinh)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lop_chinh).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.giao_vien_chu_nhiem = new SelectList(db.Giao_vien, "ma_giao_vien", "ten_giao_vien", lop_chinh.giao_vien_chu_nhiem);
            ViewBag.ma_nganh = new SelectList(db.Nganhs, "ma_nganh", "ten_nganh", lop_chinh.ma_nganh);
            return View(lop_chinh);
        }

        // GET: Lop_chinh/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lop_chinh lop_chinh = db.Lop_chinh.Find(id);
            if (lop_chinh == null)
            {
                return HttpNotFound();
            }
            return View(lop_chinh);
        }

        // POST: Lop_chinh/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Lop_chinh lop_chinh = db.Lop_chinh.Find(id);
            db.Lop_chinh.Remove(lop_chinh);
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
