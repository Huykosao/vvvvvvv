using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Text;
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
        public ActionResult Index(String sortOrder,String currentFilter,String searchString,int? page)
        {
            ViewBag.currentSort = sortOrder;
            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";
            if(searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.currentFilter = searchString;

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
            var lop_chinh_list = lop_chinh.AsEnumerable();
            if (!String.IsNullOrEmpty(searchString))
            {
                lop_chinh_list = lop_chinh_list.Where(l => l.TenLop.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name":
                    lop_chinh_list = lop_chinh_list.OrderBy(l => l.TenLop.Split(' ').LastOrDefault());
                    break;
                case "name_desc":
                    lop_chinh_list = lop_chinh_list.OrderByDescending(l => l.TenLop.Split(' ').LastOrDefault());
                    break;
                default:
                    lop_chinh_list = lop_chinh_list.OrderBy(l => l.MaLop);
                    break;
            }
            int pageSize =  10;
            int pageNumber = (page ?? 1);

            return View(lop_chinh_list.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult ExportToExcel()
        {
            // Lấy danh sách lớp chính
            var lop_chinh = db.Lop_chinh
                              .Include(l => l.Giao_vien)
                              .Include(l => l.Nganh.Nien_khoa.Khoa)
                              .OrderBy(l => l.ma_lop)
                              .ToList();

            // Tạo nội dung file CSV
            StringBuilder sb = new StringBuilder();

            // Thêm tiêu đề cột (chấm phẩy làm dấu phân cách)
            sb.AppendLine("\"Mã Lớp\";\"Tên Lớp\";\"Giáo Viên Chủ Nhiệm\";\"Ngành\";\"Niên Khóa\";\"Khoa\";\"Số Lượng Học Sinh\"");

            // Thêm dữ liệu vào file CSV
            foreach (var l in lop_chinh)
            {
                sb.AppendLine($"\"{l.ma_lop}\";" +
                              $"\"{l.ten_lop}\";" +
                              $"\"{l.Giao_vien?.ten_giao_vien ?? "N/A"}\";" +
                              $"\"{l.Nganh?.ten_nganh ?? "N/A"}\";" +
                              $"\"{l.Nganh?.Nien_khoa?.ten_nien_khoa ?? "N/A"}\";" +
                              $"\"{l.Nganh?.Nien_khoa?.Khoa?.ten_khoa ?? "N/A"}\";" +
                              $"\"{db.Hoc_sinh.Count(hs => hs.ma_lop == l.ma_lop)}\"");
            }

            // Chuyển StringBuilder thành byte array với UTF-8 BOM
            byte[] fileBytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
            string fileName = "DanhSachLopChinh.csv";

            // Trả về file CSV
            return File(fileBytes, "text/csv", fileName);
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
