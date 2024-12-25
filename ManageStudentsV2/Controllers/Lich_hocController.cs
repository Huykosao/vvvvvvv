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
    [RoleAuthorize("Admin")]
    public class Lich_hocController : Controller
    {
        private Quan_Ly_Sinh_Vien_Entities db = new Quan_Ly_Sinh_Vien_Entities();

        // GET: Lich_hoc
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

            var lich_hoc = db.Lich_hoc
                                .Include(lh => lh.Lop_hoc_phan.Mon_hoc) // Bao gồm Mon_hoc
                                .Select(lh => new ManageStudentsV2.Models.LichHocViewModel
                                {
                                    MaLich = lh.ma_lich,
                                    ThoiGian = (DateTime)lh.thoi_gian,
                                    PhongHoc = lh.phong_hoc,
                                    TenLopChinh = db.Lop_chinh
                                                    .Where(lc => lc.ma_lop == lh.Lop_hoc_phan.ma_lop) 
                                                    .Select(lc => lc.ten_lop)
                                                    .FirstOrDefault(), 
                                    TenMonHoc = lh.Lop_hoc_phan.Mon_hoc.ten_mon
                                })
                                .OrderBy(lh => lh.MaLich);

            //var lich_hoc = db.Lich_hoc
            //                    .Include(l => l.Lop_hoc_phan.Mon_hoc)
            //                    .OrderBy(l => l.ma_lich);

            int pageSize = (size ?? 5);

            int pageNumber = (page ?? 1);

            
            return View(lich_hoc.ToPagedList(pageNumber, pageSize));
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
        // GET: Lich_hoc/Details/5
        public ActionResult Details(int? id)
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
