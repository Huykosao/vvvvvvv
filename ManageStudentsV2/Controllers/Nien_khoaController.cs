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
    public class Nien_khoaController : Controller
    {
        private Quan_Ly_Sinh_Vien_Entities db = new Quan_Ly_Sinh_Vien_Entities();

        // GET: Nien_khoa
        [RoleAuthorize("Admin")]

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

            var nien_khoa = db.Nganhs
                                .Include(n => n.Nien_khoa)
                                .Include(n => n.Lop_chinh.Select(l => l.Hoc_sinh))
                                .GroupBy(nganh => new
                                {
                                    nganh.Nien_khoa.ten_nien_khoa,
                                    nganh.Nien_khoa.nam_bat_dau,
                                    nganh.Nien_khoa.nam_ket_thuc,
                                    nganh.Nien_khoa.Khoa.ten_khoa
                                })
                                .Select(group => new NienKhoaViewModel
                                {
                                    TenNienKhoa = group.Key.ten_nien_khoa,
                                    NamBatDau = (DateTime)group.Key.nam_bat_dau,
                                    NamKetThuc = (DateTime)group.Key.nam_ket_thuc,
                                    TenKhoa = group.Key.ten_khoa,
                                    SoLuongNganh = group.Count(),
                                    SoLuongHocSinh = group
                                        .SelectMany(g => g.Lop_chinh)
                                        .SelectMany(l => l.Hoc_sinh)
                                        .Count()
                                })
                                .OrderBy(nk => nk.TenNienKhoa)
                                .ToList();


            int pageSize = (size ?? 5);
            int pageNumber = (page ?? 1);

            return View(nien_khoa.ToPagedList(pageNumber, pageSize));
        }

        // GET: Nien_khoa/Details/5
        [RoleAuthorize("Admin")]

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nien_khoa nien_khoa = db.Nien_khoa.Find(id);
            if (nien_khoa == null)
            {
                return HttpNotFound();
            }
            return View(nien_khoa);
        }

        // GET: Nien_khoa/Create
        [RoleAuthorize("Admin")]

        public ActionResult Create()
        {
            ViewBag.ma_khoa = new SelectList(db.Khoas, "ma_khoa", "ten_khoa");
            return View();
        }

        // POST: Nien_khoa/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Admin")]

        public ActionResult Create([Bind(Include = "ten_nien_khoa,nam_bat_dau,nam_ket_thuc,ma_khoa")] Nien_khoa nien_khoa)
        {
            if (ModelState.IsValid)
            {
                db.Nien_khoa.Add(nien_khoa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ma_khoa = new SelectList(db.Khoas, "ma_khoa", "ten_khoa", nien_khoa.ma_khoa);
            return View(nien_khoa);
        }

        // GET: Nien_khoa/Edit/5
        [RoleAuthorize("Admin")]

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nien_khoa nien_khoa = db.Nien_khoa.Find(id);
            if (nien_khoa == null)
            {
                return HttpNotFound();
            }
            ViewBag.ma_khoa = new SelectList(db.Khoas, "ma_khoa", "ten_khoa", nien_khoa.ma_khoa);
            return View(nien_khoa);
        }

        // POST: Nien_khoa/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Admin")]

        public ActionResult Edit([Bind(Include = "ma_nien_khoa,ten_nien_khoa,nam_bat_dau,nam_ket_thuc,ma_khoa")] Nien_khoa nien_khoa)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nien_khoa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ma_khoa = new SelectList(db.Khoas, "ma_khoa", "ten_khoa", nien_khoa.ma_khoa);
            return View(nien_khoa);
        }


        // GET: Nien_khoa/Delete/5
        [RoleAuthorize("Admin")]

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nien_khoa nien_khoa = db.Nien_khoa.Find(id);
            if (nien_khoa == null)
            {
                return HttpNotFound();
            }
            return View(nien_khoa);
        }

        // POST: Nien_khoa/Delete/5
        [RoleAuthorize("Admin")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Nien_khoa nien_khoa = db.Nien_khoa.Find(id);
            db.Nien_khoa.Remove(nien_khoa);
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
