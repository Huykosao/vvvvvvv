using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ManageStudentsV2.Models;
using PagedList;

namespace ManageStudentsV2.Controllers
{
    public class Phan_congController : Controller
    {
        private Quan_Ly_Sinh_Vien_Entities db = new Quan_Ly_Sinh_Vien_Entities();
        [RoleAuthorize("Admin")]
        // GET: Phan_cong
        public ActionResult Index(String sortOrder, string currentFilter, String searchString,int? page)
        {
            ViewBag.currentSort = sortOrder;

            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.currentFilter = searchString;

            var phan_cong = db.Phan_cong
                                   .Include(p => p.Giao_vien)
                                   .Include(p => p.Lop_hoc_phan.Lop_chinh)
                                   .Include(p => p.Lop_hoc_phan.Mon_hoc)
                                   .OrderBy(p => p.ma_hoc_phan);
            var phan_cong_list = phan_cong.AsEnumerable();
            if (!String.IsNullOrEmpty(searchString))
            {
                phan_cong_list = phan_cong_list.Where(p => p.Giao_vien.ten_giao_vien.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name":
                    phan_cong_list = phan_cong_list.OrderByDescending(p => p.Giao_vien.ten_giao_vien.Split(' ').LastOrDefault());
                    break;
                case "name_desc":
                    phan_cong_list = phan_cong_list.OrderBy(p => p.Giao_vien.ten_giao_vien.Split(' ').LastOrDefault());
                    break;
                default:
                    phan_cong_list = phan_cong_list.OrderBy(p => p.ma_hoc_phan);
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(phan_cong_list.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult ExportToExcel()
        {
            var phan_cong = db.Phan_cong
                                   .Include(p => p.Giao_vien)
                                   .Include(p => p.Lop_hoc_phan.Lop_chinh)
                                   .Include(p => p.Lop_hoc_phan.Mon_hoc)
                                   .OrderBy(p => p.ma_hoc_phan)
                                   .ToList();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\"Tên Lớp\";\" Tên Môn\";\"Giáo Viên\";\"Số Sinh Viên\";\" Ngành\";\"Niên Khóa\";\"Khoa\"");
            foreach (var item in phan_cong)
            {
                sb.AppendLine($"\"{item.Lop_hoc_phan.Lop_chinh.ten_lop}\";" +
                              $"\"{item.Lop_hoc_phan.Mon_hoc.ten_mon ?? "N/A"}\";" +
                              $"\"{item.Giao_vien?.ten_giao_vien ?? "N/A"}\";" +
                              $"\"{db.Lop_dang_ky.Count(ldk => ldk.ma_hoc_phan == item.ma_hoc_phan)}\";" +
                              $"\"{item.Lop_hoc_phan.Mon_hoc.Nganh.ten_nganh ?? "N/A"}\";" +
                              $"\"{item.Lop_hoc_phan.Mon_hoc.Nganh.Nien_khoa.ten_nien_khoa ?? "N/A"}\";" +
                              $"\"{item.Lop_hoc_phan.Mon_hoc.Nganh.Nien_khoa.Khoa?.ten_khoa ?? "N/A"}\"");
            }

            // Chuyển StringBuilder thành byte array với UTF-8 BOM
            byte[] fileBytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
            string fileName = "DanhSachPhanCong.csv";   

            // Trả về file CSV
            return File(fileBytes, "text/csv", fileName);
        }

        // GET: Phan_cong/Create
        public ActionResult Create()
        {
            ViewBag.ma_giao_vien = new SelectList(db.Giao_vien, "ma_giao_vien", "ten_giao_vien");
            ViewBag.ma_hoc_phan = new SelectList(db.Lop_hoc_phan
                                            .Include(lhp => lhp.Mon_hoc)
                                            .Select(lhp => new
                                            {
                                                ma_hoc_phan = lhp.ma_hoc_phan,
                                                mon_hoc = lhp.Mon_hoc.ten_mon,
                                                giao_vien = lhp.Phan_cong.Any() ? lhp.Phan_cong.FirstOrDefault().Giao_vien.ten_giao_vien : "",
                                                mon_hoc_giao_vien = lhp.Mon_hoc.ten_mon + " - " + (lhp.Phan_cong.Any() ? lhp.Phan_cong.FirstOrDefault().Giao_vien.ten_giao_vien : "Chua phan cong")
                                            }), "ma_hoc_phan", "mon_hoc_giao_vien");
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
        public ActionResult Edit(int? ma_hoc_phan, int? ma_giao_vien)
        {
            if (ma_hoc_phan == null || ma_hoc_phan == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Phan_cong phan_cong = db.Phan_cong
                                        .Where(pc => pc.ma_hoc_phan == ma_hoc_phan)
                                        .Where(pc => pc.ma_giao_vien == ma_giao_vien)
                                        .FirstOrDefault();
            if (phan_cong == null)
            {
                return HttpNotFound();
            }
            ViewBag.ma_giao_vien = new SelectList(db.Giao_vien, "ma_giao_vien", "ten_giao_vien", phan_cong.ma_giao_vien);
            ViewBag.ma_hoc_phan = new SelectList(db.Lop_hoc_phan
                                             .Include(lhp => lhp.Mon_hoc)
                                             .Select(lhp => new
                                             {
                                                 ma_hoc_phan = lhp.ma_hoc_phan,
                                                 mon_hoc = lhp.Mon_hoc.ten_mon,
                                                 giao_vien = lhp.Phan_cong.Any() ? lhp.Phan_cong.FirstOrDefault().Giao_vien.ten_giao_vien : "",
                                                 mon_hoc_giao_vien = lhp.Mon_hoc.ten_mon + " - " + (lhp.Phan_cong.Any() ? lhp.Phan_cong.FirstOrDefault().Giao_vien.ten_giao_vien : "Chua phan cong")
                                             }), "ma_hoc_phan", "mon_hoc_giao_vien");
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
        public ActionResult Delete(int? ma_hoc_phan, int? ma_giao_vien)
        {
            if (ma_hoc_phan == null || ma_hoc_phan == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Phan_cong phan_cong = db.Phan_cong
                                        .Where(pc => pc.ma_hoc_phan == ma_hoc_phan)
                                        .Where(pc => pc.ma_giao_vien == ma_giao_vien)
                                        .FirstOrDefault();
            if (phan_cong == null)
            {
                return HttpNotFound();
            }
            return View(phan_cong);
        }

        // POST: Phan_cong/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? ma_hoc_phan, int? ma_giao_vien)
        {
            Phan_cong phan_cong = db.Phan_cong
                                       .Where(pc => pc.ma_hoc_phan == ma_hoc_phan)
                                       .Where(pc => pc.ma_giao_vien == ma_giao_vien)
                                       .FirstOrDefault();
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
