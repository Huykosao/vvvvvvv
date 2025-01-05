using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using ManageStudentsV2.Models;
using PagedList;

namespace ManageStudentsV2.Controllers
{
    
    public class Lich_hocController : Controller
    {
        private Quan_Ly_Sinh_Vien_Entities db = new Quan_Ly_Sinh_Vien_Entities();

        // GET: Lich_hoc
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DateSortParm = string.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            if (Session["Role"] == null)
                return RedirectToAction("Index", "ManageStudentHome");

            IQueryable<LichHocViewModel> lichHocQuery = GetLichHocByRole(Session["Role"].ToString(), (int?)Session["UserID"]);

            if (lichHocQuery == null)
                return RedirectToAction("Index", "ManageStudentHome");

            if (!string.IsNullOrEmpty(searchString))
            {
                lichHocQuery = lichHocQuery.Where(lh => lh.TenMonHoc.Contains(searchString));
            }

            lichHocQuery = SortLichHoc(lichHocQuery, sortOrder);

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(lichHocQuery.ToPagedList(pageNumber, pageSize));
        }

        // Tách logic truy vấn theo vai trò
        private IQueryable<LichHocViewModel> GetLichHocByRole(string role, int? userId)
        {
            if (role == "Admin")
            {
                return db.Lich_hoc
                    .Include(lh => lh.Lop_hoc_phan.Mon_hoc)
                    .Select(lh => new LichHocViewModel
                    {
                        MaLich = lh.ma_lich,
                        ThoiGian = lh.thoi_gian ?? DateTime.MinValue,
                        PhongHoc = lh.phong_hoc,
                        TenLopChinh = lh.Lop_hoc_phan.Lop_chinh.ten_lop,
                        TenMonHoc = lh.Lop_hoc_phan.Mon_hoc.ten_mon
                    });
            }
            else if (role == "Student" && userId.HasValue)
            {
                var hocSinh = db.Hoc_sinh.FirstOrDefault(hs => hs.UserID == userId);
                if (hocSinh == null)
                    return null;

                return db.Lich_hoc
                    .Include(lh => lh.Lop_hoc_phan.Mon_hoc)
                    .Where(lh => lh.Lop_hoc_phan.Lop_chinh.Hoc_sinh.Any(hs => hs.ma_sinh_vien == hocSinh.ma_sinh_vien))
                    .Select(lh => new LichHocViewModel
                    {
                        MaLich = lh.ma_lich,
                        ThoiGian = lh.thoi_gian ?? DateTime.MinValue,
                        PhongHoc = lh.phong_hoc,
                        TenLopChinh = lh.Lop_hoc_phan.Lop_chinh.ten_lop,
                        TenMonHoc = lh.Lop_hoc_phan.Mon_hoc.ten_mon
                    });
            }
            else if (role == "Teacher" && userId.HasValue)
            {
                var giaoVien = db.Giao_vien.FirstOrDefault(gv => gv.UserID == userId);
                if (giaoVien == null)
                    return null;

                return db.Lich_hoc
                    .Include(lh => lh.Lop_hoc_phan.Mon_hoc)
                    .Where(lh => lh.Lop_hoc_phan.Phan_cong.Any(pc => pc.ma_giao_vien == giaoVien.ma_giao_vien))
                    .Select(lh => new LichHocViewModel
                    {
                        MaLich = lh.ma_lich,
                        ThoiGian = lh.thoi_gian ?? DateTime.MinValue,
                        PhongHoc = lh.phong_hoc,
                        TenLopChinh = lh.Lop_hoc_phan.Lop_chinh.ten_lop,
                        TenMonHoc = lh.Lop_hoc_phan.Mon_hoc.ten_mon
                    });
            }

            return null;
        }

        // Tách logic sắp xếp
        private IQueryable<LichHocViewModel> SortLichHoc(IQueryable<LichHocViewModel> lichHocQuery, string sortOrder)
        {
            switch (sortOrder)
            {
                case "name":
                    return lichHocQuery.OrderBy(lh => lh.TenMonHoc.Split(' ').LastOrDefault());
                case "name_desc":
                    return lichHocQuery.OrderByDescending(lh => lh.TenMonHoc.Split(' ').LastOrDefault());
                case "date_desc":
                    return lichHocQuery.OrderByDescending(lh => lh.ThoiGian);
                default:
                    return lichHocQuery.OrderBy(lh => lh.ThoiGian);
            }
        }
        public ActionResult ExportToExcel()
        {
            // Lấy danh sách lịch học
            var lich_hoc = db.Lich_hoc
                             .Include(l => l.Lop_hoc_phan)
                             .OrderBy(l => l.ma_lich)
                             .ToList();

            // Tạo nội dung file CSV
            StringBuilder sb = new StringBuilder();

            // Thêm tiêu đề cột (chấm phẩy làm dấu phân cách)
            sb.AppendLine("\"Mã Lịch\";\"Thời Gian\";\"Phòng Học\";\"Tên Lớp\";\"Tên Môn\";\"Giáo Viên\";\"Ngành\";\"Niên Khóa\";\"Khoa\"");

            // Thêm dữ liệu vào file CSV
            foreach (var l in lich_hoc)
            {
                sb.AppendLine($"\"{l.ma_lich}\";" +
                              $"\"{l.thoi_gian?.ToString("dd/MM/yyyy HH:mm") ?? "N/A"}\";" +
                              $"\"{l.phong_hoc}\";" +
                              $"\"{l.Lop_hoc_phan?.Lop_chinh?.ten_lop ?? "N/A"}\";" +
                              $"\"{l.Lop_hoc_phan?.Mon_hoc?.ten_mon ?? "N/A"}\";" +
                              $"\"{l.Lop_hoc_phan?.Phan_cong.FirstOrDefault()?.Giao_vien.ten_giao_vien ?? "N/A"}\";" +
                              $"\"{l.Lop_hoc_phan?.Mon_hoc?.Nganh?.ten_nganh ?? "N/A"}\";" +
                              $"\"{l.Lop_hoc_phan?.Mon_hoc?.Nganh?.Nien_khoa?.ten_nien_khoa ?? "N/A"}\";" +
                              $"\"{l.Lop_hoc_phan?.Mon_hoc?.Nganh?.Nien_khoa?.Khoa?.ten_khoa ?? "N/A"}\"");
            }

            // Chuyển StringBuilder thành byte array với UTF-8 BOM
            byte[] fileBytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
            string fileName = "DanhSachLichHoc.csv";

            // Trả về file CSV
            return File(fileBytes, "text/csv", fileName);
        }
        
        // GET: Lich_hoc/Create
        public ActionResult Create()
        {
            ViewBag.ma_hoc_phan = new SelectList(db.Lop_hoc_phan
                                                        .Include(lh => lh.Mon_hoc) // Bao gồm Mon_hoc để lấy tên môn học
                                                        .Include(lh => lh.Lop_chinh) // Bao gồm Lop_chinh để lấy tên lớp
                                                        .Select(lh => new
                                                        {
                                                            ma_hoc_phan = lh.ma_hoc_phan,
                                                            ten_mon = lh.Mon_hoc.ten_mon, // Lấy tên môn học
                                                            ten_lop = lh.Lop_chinh.ten_lop,
                                                            ten_lop_mon = lh.Lop_chinh.ten_lop + " - " + lh.Mon_hoc.ten_mon
                                                        }),
                                                        "ma_hoc_phan", "ten_lop_mon"); // Sử dụng "ma_hoc_phan" làm giá trị và "ten_lop" làm tên hiển thị
            return View();
        }

        // POST: Lich_hoc/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ma_lich,thoi_gian,phong_hoc,ma_hoc_phan")] Lich_hoc lich_hoc)
        {
            if (ModelState.IsValid)
            {
                db.Lich_hoc.Add(lich_hoc);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ma_hoc_phan = new SelectList(db.Lop_hoc_phan, "ma_hoc_phan", "ma_hoc_phan", lich_hoc.ma_hoc_phan);
            return View(lich_hoc);
        }

        // GET: Lich_hoc/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Lich_hoc lich_hoc = db.Lich_hoc.Find(id);
            if (lich_hoc == null)
            {
                return HttpNotFound();
            }

            // Tạo SelectList với danh sách lớp học phần bao gồm tên môn và tên lớp
            ViewBag.ma_hoc_phan = new SelectList(
                db.Lop_hoc_phan
                    .Include(lh => lh.Mon_hoc) // Bao gồm Mon_hoc để lấy tên môn học
                    .Include(lh => lh.Lop_chinh) // Bao gồm Lop_chinh để lấy tên lớp
                    .Select(lh => new
                    {
                        ma_hoc_phan = lh.ma_hoc_phan, // Giá trị của dropdown
                        ten_lop_mon = lh.Lop_chinh.ten_lop + " - " + lh.Mon_hoc.ten_mon // Tên lớp và môn học kết hợp
                    }).ToList(),
                "ma_hoc_phan", // Trường chứa giá trị (value) cho dropdown
                "ten_lop_mon", // Trường chứa giá trị hiển thị cho dropdown
                lich_hoc.ma_hoc_phan // Giá trị mặc định được chọn trong dropdown
            );

            return View(lich_hoc);
        }


        // POST: Lich_hoc/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ma_lich,thoi_gian,phong_hoc,ma_hoc_phan")] Lich_hoc lich_hoc)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lich_hoc).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ma_hoc_phan = new SelectList(db.Lop_hoc_phan, "ma_hoc_phan", "ma_hoc_phan", lich_hoc.ma_hoc_phan);
            return View(lich_hoc);
        }

        // GET: Lich_hoc/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lich_hoc lich_hoc = db.Lich_hoc.Find(id);
            if (lich_hoc == null)
            {
                return HttpNotFound();
            }
            return View(lich_hoc);
        }

        // POST: Lich_hoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Lich_hoc lich_hoc = db.Lich_hoc.Find(id);
            db.Lich_hoc.Remove(lich_hoc);
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
