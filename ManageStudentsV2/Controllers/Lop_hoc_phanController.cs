using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ManageStudentsV2.Models;
using PagedList;

namespace ManageStudentsV2.Controllers
{
    //[RoleAuthorize("Admin")]

    public class Lop_hoc_phanController : Controller
    {
        private Quan_Ly_Sinh_Vien_Entities db = new Quan_Ly_Sinh_Vien_Entities();

        // GET: Lop_hoc_phan
        //[RoleAuthorize("Admin")]
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
            if (Session["Role"] == null)
                return RedirectToAction("Index", "ManageStudentHome");
            if (Session["Role"].ToString() == "Admin")
            {
                //sql
                var lop_hoc_phan = db.Lop_hoc_phan
                                        .Include(lhp => lhp.Lop_chinh)
                                        .Include(lhp => lhp.Mon_hoc)
                                        .Include(lhp => lhp.Phan_cong.Select(pc => pc.Giao_vien))
                                        .Select(lhp => new ManageStudentsV2.Models.LopHocPhanViewModel
                                        {
                                            MaHocPhan = lhp.ma_hoc_phan,
                                            MaLop = lhp.ma_lop,
                                            TenLop = lhp.Lop_chinh.ten_lop,
                                            MaMon = lhp.ma_mon,
                                            TenMon = lhp.Mon_hoc.ten_mon,
                                            GiaoVien = lhp.Phan_cong.FirstOrDefault().Giao_vien.ten_giao_vien,
                                            SoSinhVien = db.Lop_dang_ky.Count(ldk => ldk.ma_hoc_phan == lhp.ma_hoc_phan),
                                            Nganh = lhp.Mon_hoc.Nganh.ten_nganh,
                                            NienKhoa = lhp.Mon_hoc.Nganh.Nien_khoa.ten_nien_khoa,
                                            Khoa = lhp.Mon_hoc.Nganh.Nien_khoa.Khoa.ten_khoa
                                        })
                                        .OrderBy(lhp => lhp.MaHocPhan);
                var lop_hoc_phan_list = lop_hoc_phan.AsEnumerable();
                if (!String.IsNullOrEmpty(searchString))
                {
                    lop_hoc_phan_list = lop_hoc_phan_list.Where(lhp => lhp.TenLop.Contains(searchString));
                }
                switch (sortOrder)
                {
                    case "name":
                        lop_hoc_phan_list = lop_hoc_phan_list.OrderBy(lhp => lhp.TenLop.Split(' ').LastOrDefault());
                        break;
                    case "name_desc":
                        lop_hoc_phan_list = lop_hoc_phan_list.OrderByDescending(lhp => lhp.TenLop.Split(' ').LastOrDefault());
                        break;
                    default:
                        lop_hoc_phan_list = lop_hoc_phan_list.OrderBy(lhp => lhp.MaHocPhan);
                        break;
                }
                int pageSize = 10;
                int pageNumber = (page ?? 1);

                return View(lop_hoc_phan_list.ToPagedList(pageNumber, pageSize));
            }else if (Session["Role"].ToString() == "Teacher")
            {

                int userId = Convert.ToInt32(Session["UserID"]);
                var giao_vien = db.Giao_vien.Where(gv => gv.UserID == userId).FirstOrDefault();

                if (giao_vien == null)
                    return View();

                //Tên Môn Giáo Viên   Số Sinh Viên Ngành   Niên Khóa   Khoa
                var lop_hoc_phan = db.Phan_cong
                                        .Where(pc => pc.ma_giao_vien == giao_vien.ma_giao_vien)
                                        .Select(pc => new ManageStudentsV2.Models.LopHocPhanViewModel
                                        {
                                            MaHocPhan = pc.Lop_hoc_phan.ma_hoc_phan,
                                            MaLop = pc.Lop_hoc_phan.ma_lop,
                                            MaMon = pc.Lop_hoc_phan.Mon_hoc.ma_mon,
                                            TenMon = pc.Lop_hoc_phan.Mon_hoc.ten_mon,
                                            GiaoVien = pc.Giao_vien.ten_giao_vien,
                                            TenLop = pc.Lop_hoc_phan.Lop_chinh.ten_lop,
                                            SoSinhVien = pc.Lop_hoc_phan.Lop_dang_ky.Count(),
                                            Nganh = pc.Lop_hoc_phan.Mon_hoc.Nganh.ten_nganh,
                                            NienKhoa = pc.Lop_hoc_phan.Mon_hoc.Nganh.Nien_khoa.ten_nien_khoa,
                                            Khoa = pc.Lop_hoc_phan.Mon_hoc.Nganh.Nien_khoa.Khoa.ten_khoa
                                        }).OrderBy(lhp => lhp.MaHocPhan);
                var lop_hoc_phan_list = lop_hoc_phan.AsEnumerable();
                if(!String.IsNullOrEmpty(searchString))
                {
                    lop_hoc_phan_list = lop_hoc_phan_list.Where(lhp => lhp.TenLop.Contains(searchString));
                }
                switch (sortOrder)
                {
                    case "name":
                        lop_hoc_phan_list = lop_hoc_phan_list.OrderBy(lhp => lhp.TenLop.Split(' ').LastOrDefault());
                        break;
                    case "name_desc":
                        lop_hoc_phan_list = lop_hoc_phan_list.OrderByDescending(lhp => lhp.TenLop.Split(' ').LastOrDefault());
                        break;
                    default:
                        lop_hoc_phan_list = lop_hoc_phan_list.OrderBy(lhp => lhp.MaHocPhan);
                        break;
                }
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                return View(lop_hoc_phan_list.ToPagedList(pageNumber, pageSize));

            }
            else if (Session["Role"].ToString() == "Student")
            {
                return View();
            }

            return RedirectToAction("Index", "ManageStudentHome");

        }
        [RoleAuthorize("Admin")]
        public ActionResult ExportToExcel()
        {
            // Lấy danh sách lớp học phần
            var lop_hoc_phan = db.Lop_hoc_phan
                                 .Include(lhp => lhp.Lop_chinh)
                                 .Include(lhp => lhp.Mon_hoc)
                                 .Include(lhp => lhp.Phan_cong.Select(pc => pc.Giao_vien))
                                 .OrderBy(lhp => lhp.ma_hoc_phan)
                                 .ToList();

            // Tạo nội dung file CSV
            StringBuilder sb = new StringBuilder();

            // Thêm tiêu đề cột (chấm phẩy làm dấu phân cách)
            sb.AppendLine("\"Mã Học Phần\";\"Tên Lớp\";\"Tên Môn\";\"Giáo Viên\";\"Số Sinh Viên\";\"Ngành\";\"Niên Khóa\";\"Khoa\"");

            // Thêm dữ liệu vào file    
            foreach (var lhp in lop_hoc_phan)
            {
                sb.AppendLine($"\"{lhp.ma_hoc_phan}\";" +
                              $"\"{lhp.Lop_chinh?.ten_lop ?? "N/A"}\";" +
                              $"\"{lhp.Mon_hoc?.ten_mon ?? "N/A"}\";" +
                              $"\"{lhp.Phan_cong.FirstOrDefault()?.Giao_vien.ten_giao_vien ?? "N/A"}\";" +
                              $"\"{db.Lop_dang_ky.Count(ldk => ldk.ma_hoc_phan == lhp.ma_hoc_phan)}\";" +
                              $"\"{lhp.Mon_hoc?.Nganh?.ten_nganh ?? "N/A"}\";" +
                              $"\"{lhp.Mon_hoc?.Nganh?.Nien_khoa?.ten_nien_khoa ?? "N/A"}\";" +
                              $"\"{lhp.Mon_hoc?.Nganh?.Nien_khoa?.Khoa?.ten_khoa ?? "N/A"}\"");
            }

            // Chuyển StringBuilder thành byte array với UTF-8 BOM
            byte[] fileBytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
            string fileName = "DanhSachLopHocPhan.csv";

            // Trả về file CSV
            return File(fileBytes, "text/csv", fileName);
        }
        [RoleAuthorize("Admin")]
        // GET: Lop_hoc_phan/Create
        public ActionResult Create()
        {
            ViewBag.ma_lop = new SelectList(db.Lop_chinh, "ma_lop", "ten_lop");
            ViewBag.ma_mon = new SelectList(db.Mon_hoc, "ma_mon", "ten_mon");
            return View();
        }
        [RoleAuthorize("Admin")]
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
        [RoleAuthorize("Admin")]
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
        [RoleAuthorize("Admin")]
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
        [RoleAuthorize("Admin")]
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
            ViewBag.canh_bao = "Thao tác này sẽ xóa lớp học phần và các lớp liên quan (Phân công, Lịch hoc) !!!";
            return View(lop_hoc_phan);
        }
        [RoleAuthorize("Admin")]
        // POST: Lop_hoc_phan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    Lop_hoc_phan lop_hoc_phan = db.Lop_hoc_phan.Find(id);
                    var lophocphan_lichhoc = db.Lich_hoc.Where(l => l.ma_hoc_phan == lop_hoc_phan.ma_hoc_phan).ToList();
                    var lophocphan_phancong = db.Phan_cong.Where(pc => pc.ma_hoc_phan == lop_hoc_phan.ma_hoc_phan).ToList();
                    var lophocphan_lopdangky = db.Lop_dang_ky.Where(ldk => ldk.ma_hoc_phan == lop_hoc_phan.ma_hoc_phan).ToList();
                    db.Lop_dang_ky.RemoveRange(lophocphan_lopdangky);
                    db.Lich_hoc.RemoveRange(lophocphan_lichhoc);
                    db.Phan_cong.RemoveRange(lophocphan_phancong);
                    db.Lop_hoc_phan.Remove(lop_hoc_phan);
                    db.SaveChanges();
                    transaction.Commit();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Transaction failed: " + ex.Message);
                    return RedirectToAction("Index");
                }
            }
            
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
