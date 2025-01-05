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
    public class Nien_khoaController : Controller
    {
        private Quan_Ly_Sinh_Vien_Entities db = new Quan_Ly_Sinh_Vien_Entities();

        // GET: Nien_khoa
        [RoleAuthorize("Admin")]

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

            var nien_khoa = db.Nien_khoa
                                .Include(n => n.Khoa)
                                .Include(n => n.Nganhs.Select(ng => ng.Lop_chinh.Select(lc => lc.Hoc_sinh)))
                                .GroupBy(nk => new
                                {
                                    nk.ma_nien_khoa,
                                    nk.ten_nien_khoa,
                                    nk.nam_bat_dau,
                                    nk.nam_ket_thuc,
                                    nk.Khoa.ten_khoa
                                })
                                .Select(group => new ManageStudentsV2.Models.NienKhoaViewModel
                                {
                                    MaNienKhoa = group.Key.ma_nien_khoa,
                                    TenNienKhoa = group.Key.ten_nien_khoa,
                                    NamBatDau = group.Key.nam_bat_dau ?? DateTime.MinValue,
                                    NamKetThuc = group.Key.nam_ket_thuc ?? DateTime.MinValue,
                                    TenKhoa = group.Key.ten_khoa,
                                    SoLuongNganh = group.Count(),
                                    SoLuongHocSinh = group
                                        .SelectMany(g => g.Nganhs)
                                        .SelectMany(ng => ng.Lop_chinh)
                                        .SelectMany(lc => lc.Hoc_sinh)
                                        .Count()
                                })
                                .ToList();


            var nien_khoa_list = nien_khoa.AsEnumerable();
            if (!String.IsNullOrEmpty(searchString))
            {
                nien_khoa_list = nien_khoa_list.Where(nk => nk.TenNienKhoa.Contains(searchString));
            }
            switch(sortOrder)
            {
                case "name":
                    nien_khoa_list = nien_khoa_list.OrderBy(nk => nk.TenNienKhoa.Split(' ').LastOrDefault());
                    break;
                case "name_desc":
                    nien_khoa_list = nien_khoa_list.OrderByDescending(nk => nk.TenNienKhoa.Split(' ').LastOrDefault());
                    break;
                default:
                    nien_khoa_list = nien_khoa_list.OrderBy(nk => nk.MaNienKhoa);
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(nien_khoa_list.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult ExportToExcel()
        {
            // Lấy danh sách niên khóa
            var nien_khoa = db.Nien_khoa
                              .Include(n => n.Khoa)
                              .OrderBy(n => n.ma_nien_khoa)
                              .ToList();

            // Tạo nội dung file CSV
            StringBuilder sb = new StringBuilder();

            // Thêm tiêu đề cột (chấm phẩy làm dấu phân cách)
            sb.AppendLine("\"Mã Niên Khóa\";\"Tên Niên Khóa\";\"Năm Bắt Đầu\";\"Năm Kết Thúc\";\"Khoa\"");

            // Thêm dữ liệu vào file CSV
            foreach (var n in nien_khoa)
            {
                sb.AppendLine($"\"{n.ma_nien_khoa}\";" +
                              $"\"{n.ten_nien_khoa}\";" +
                              $"\"{n.nam_bat_dau?.ToString("yyyy") ?? "N/A"}\";" +
                              $"\"{n.nam_ket_thuc?.ToString("yyyy") ?? "N/A"}\";" +
                              $"\"{n.Khoa?.ten_khoa ?? "N/A"}\"");
            }

            // Chuyển StringBuilder thành byte array với UTF-8 BOM
            byte[] fileBytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
            string fileName = "DanhSachNienKhoa.csv";

            // Trả về file CSV
            return File(fileBytes, "text/csv", fileName);
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

            ViewBag.canh_bao = "Thao tác này sẽ xóa tất các các ngành và các lớp liên quan của niên khóa " + nien_khoa.ten_nien_khoa + " !!!";
            return View(nien_khoa);
        }

        // POST: Nien_khoa/Delete/5
        [RoleAuthorize("Admin")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    Nien_khoa nien_khoa = db.Nien_khoa.Find(id);

                    var nienkhoa_nganh = db.Nganhs.Where(n => n.ma_nien_khoa == nien_khoa.ma_nien_khoa).ToList();

                    foreach(var nganh in nienkhoa_nganh)
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
