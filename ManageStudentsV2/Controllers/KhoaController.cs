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
    [RoleAuthorize("Admin")]
    public class KhoaController : Controller
    {
        private Quan_Ly_Sinh_Vien_Entities db = new Quan_Ly_Sinh_Vien_Entities();

        // GET: Khoa
        public ActionResult Index(String sortOrder,String currentFilter,String searchString,int? page)
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
            var khoa = db.Khoas
                            .ToList();
            var khoa_list = khoa.AsEnumerable();
            if (!String.IsNullOrEmpty(searchString))
            {
                khoa_list = khoa_list.Where(k => k.ten_khoa.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name":
                    khoa_list = khoa_list.OrderBy(k => k.ten_khoa.Split(' ').LastOrDefault());
                    break;
                case "name_desc":
                    khoa_list = khoa_list.OrderByDescending(k => k.ten_khoa.Split(' ').LastOrDefault());
                    break;  
                default:
                    khoa_list = khoa_list.OrderBy(k => k.ma_khoa);
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(khoa_list.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult ExportToExcel()
        {
            // Lấy danh sách khoa
            var khoa = db.Khoas
                         .OrderBy(k => k.ma_khoa)
                         .ToList();

            // Tạo nội dung file CSV
            StringBuilder sb = new StringBuilder();

            // Thêm tiêu đề cột (chấm phẩy làm dấu phân cách)
            sb.AppendLine("\"Mã Khoa\";\"Tên Khoa\";\"Mô Tả Khoa\"");

            // Thêm dữ liệu vào file CSV
            foreach (var k in khoa)
            {
                sb.AppendLine($"\"{k.ma_khoa}\";" +
                              $"\"{k.ten_khoa}\";" +
                              $"\"{k.mo_ta_khoa}\"");
            }

            // Chuyển StringBuilder thành byte array với UTF-8 BOM
            byte[] fileBytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
            string fileName = "DanhSachKhoa.csv";

            // Trả về file CSV
            return File(fileBytes, "text/csv", fileName);
        }
        
        // GET: Khoa/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Khoa/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ten_khoa,mo_ta_khoa")] Khoa khoa)
        {
            if (ModelState.IsValid)
            {
                db.Khoas.Add(khoa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(khoa);
        }

        // GET: Khoa/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Khoa khoa = db.Khoas.Find(id);
            if (khoa == null)
            {
                return HttpNotFound();
            }
            return View(khoa);
        }

        // POST: Khoa/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ma_khoa,ten_khoa,mo_ta_khoa")] Khoa khoa)
        {
            if (ModelState.IsValid)
            {
                db.Entry(khoa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(khoa);
        }

        // GET: Khoa/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Khoa khoa = db.Khoas.Find(id);
            if (khoa == null)
            {
                return HttpNotFound();
            }

            ViewBag.canh_bao = "Thao tác này sẽ xóa các lớp liên quan của khoa " + khoa.ten_khoa + " (Niên khoa, ngành, giáo viên, lớp, môn ,...) !!!";
            return View(khoa);
        }

        // POST: Khoa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    Khoa khoa = db.Khoas.Find(id);

                    var khoa_nienkhoa = db.Nien_khoa.Where(nk => nk.ma_khoa == khoa.ma_khoa).ToList();

                    foreach( var nien_khoa in khoa_nienkhoa)
                    {
                        var nienkhoa_nganh = db.Nganhs.Where(n => n.ma_nien_khoa == nien_khoa.ma_nien_khoa).ToList();

                        foreach (var nganh in nienkhoa_nganh)
                        {
                            var nganh_lopchinh = db.Lop_chinh.Where(lc => lc.ma_nganh == nganh.ma_nganh).ToList();
                            foreach (var lop_chinh in nganh_lopchinh)
                            {
                                var lopchinh_hocsinh = db.Hoc_sinh.Where(hs => hs.ma_lop == lop_chinh.ma_lop).ToList();
                                foreach (var hoc_sinh in lopchinh_hocsinh)
                                {
                                    var hocsinh_lopdangky = db.Lop_dang_ky.Where(ldk => ldk.ma_sinh_vien == hoc_sinh.ma_sinh_vien).ToList();
                                    var hocsinh_diem = db.Diems.Where(d => d.ma_sinh_vien == hoc_sinh.ma_sinh_vien).ToList();
                                    db.Diems.RemoveRange(hocsinh_diem);
                                    db.Lop_dang_ky.RemoveRange(hocsinh_lopdangky);
                                    db.Hoc_sinh.Remove(hoc_sinh);
                                }

                                var lopchinh_lophocphan = db.Lop_hoc_phan.Where(lhp => lhp.ma_lop == lop_chinh.ma_lop).ToList();
                                foreach (var lop_hoc_phan in lopchinh_lophocphan)
                                {
                                    var lophocphan_lichhoc = db.Lich_hoc.Where(l => l.ma_hoc_phan == lop_hoc_phan.ma_hoc_phan).ToList();
                                    var lophocphan_phancong = db.Phan_cong.Where(pc => pc.ma_hoc_phan == lop_hoc_phan.ma_hoc_phan).ToList();
                                    var lophocphan_lopdangky = db.Lop_dang_ky.Where(ldk => ldk.ma_hoc_phan == lop_hoc_phan.ma_hoc_phan).ToList();
                                    db.Lop_dang_ky.RemoveRange(lophocphan_lopdangky);
                                    db.Lich_hoc.RemoveRange(lophocphan_lichhoc);
                                    db.Phan_cong.RemoveRange(lophocphan_phancong);
                                    db.Lop_hoc_phan.Remove(lop_hoc_phan);
                                }

                                db.Lop_chinh.Remove(lop_chinh);
                            }

                            var nganh_monhoc = db.Mon_hoc.Where(mh => mh.ma_nganh == nganh.ma_nganh).ToList();
                            foreach (var mon_hoc in nganh_monhoc)
                            {
                                var monhoc_diem = db.Diems.Where(d => d.ma_mon == mon_hoc.ma_mon).ToList();
                                var monhoc_lophocphan = db.Lop_hoc_phan.Where(lhp => lhp.ma_mon == mon_hoc.ma_mon).ToList();
                                foreach (var lophocphan in monhoc_lophocphan)
                                {
                                    var lichhoc = db.Lich_hoc.Where(lh => lh.ma_hoc_phan == lophocphan.ma_hoc_phan).ToList();
                                    var phancong = db.Phan_cong.Where(pc => pc.ma_hoc_phan == lophocphan.ma_hoc_phan).ToList();
                                    var lopdangky = db.Lop_dang_ky.Where(ldk => ldk.ma_hoc_phan == lophocphan.ma_hoc_phan).ToList();
                                    db.Lop_dang_ky.RemoveRange(lopdangky);
                                    db.Lich_hoc.RemoveRange(lichhoc);
                                    db.Phan_cong.RemoveRange(phancong);
                                }

                                db.Diems.RemoveRange(monhoc_diem);
                                db.Lop_hoc_phan.RemoveRange(monhoc_lophocphan);
                                db.Mon_hoc.Remove(mon_hoc);
                            }

                            db.Nganhs.Remove(nganh);
                        }

                        db.Nien_khoa.Remove(nien_khoa);
                    }

                    var khoa_giaovien = db.Giao_vien.Where(gv => gv.ma_khoa  == khoa.ma_khoa).ToList();
                    foreach(var giao_vien  in khoa_giaovien)
                    {
                        var phanCongLienQuan = db.Phan_cong.Where(pc => pc.ma_giao_vien == giao_vien.ma_giao_vien).ToList();
                        db.Phan_cong.RemoveRange(phanCongLienQuan);
                        db.Giao_vien.Remove(giao_vien);
                    }

                    db.Khoas.Remove(khoa);
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
