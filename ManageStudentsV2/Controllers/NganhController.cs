using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using ManageStudentsV2.Models;
using System.Text;


namespace ManageStudentsV2.Controllers
{
    [RoleAuthorize("Admin")]
    public class NganhController : Controller
    {
        private Quan_Ly_Sinh_Vien_Entities db = new Quan_Ly_Sinh_Vien_Entities();

        // GET: Nganh
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

            var nganhs = db.Nganhs.Include(n => n.Nien_khoa).OrderBy(n => n.ma_nganh);

            var nganh_list = nganhs.AsEnumerable();
            if (!String.IsNullOrEmpty(searchString))
            {
                nganh_list = nganh_list.Where(n => n.ten_nganh.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name":
                    nganh_list = nganh_list.OrderBy(n => n.ten_nganh.Split(' ').LastOrDefault());
                    break;
                case "name_desc":
                    nganh_list = nganh_list.OrderByDescending(n => n.ten_nganh.Split(' ').LastOrDefault());
                    break;
                default:
                    nganh_list = nganh_list.OrderBy(n => n.ma_nganh);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(nganh_list.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult ExportToExcel()
        {
            // Lấy danh sách ngành
            var nganh = db.Nganhs
                          .Include(n => n.Nien_khoa)
                          .OrderBy(n => n.ma_nganh)
                          .ToList();

            // Tạo nội dung file CSV
            StringBuilder sb = new StringBuilder();

            // Thêm tiêu đề cột (chấm phẩy làm dấu phân cách)
            sb.AppendLine("\"Mã Ngành\";\"Tên Ngành\";\"Mô Tả Ngành\";\"Niên Khóa\"");

            // Thêm dữ liệu vào file CSV
            foreach (var n in nganh)
            {
                sb.AppendLine($"\"{n.ma_nganh}\";" +
                              $"\"{n.ten_nganh}\";" +
                              $"\"{n.mo_ta_nganh}\";" +
                              $"\"{n.Nien_khoa?.ten_nien_khoa ?? "N/A"}\"");
            }

            // Chuyển StringBuilder thành byte array với UTF-8 BOM
            byte[] fileBytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
            string fileName = "DanhSachNganh.csv";

            // Trả về file CSV
            return File(fileBytes, "text/csv", fileName);
        }
        

        // GET: Nganh/Create
        public ActionResult Create()
        {
            ViewBag.ma_nien_khoa = new SelectList(db.Nien_khoa, "ma_nien_khoa", "ten_nien_khoa");
            return View();
        }

        // POST: Nganh/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ten_nganh,mo_ta_nganh,ma_nien_khoa")] Nganh nganh)
        {
            if (ModelState.IsValid)
            {
                db.Nganhs.Add(nganh);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ma_nien_khoa = new SelectList(db.Nien_khoa, "ma_nien_khoa", "ten_nien_khoa", nganh.ma_nien_khoa);
            return View(nganh);
        }

        // GET: Nganh/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nganh nganh = db.Nganhs.Find(id);
            if (nganh == null)
            {
                return HttpNotFound();
            }
            ViewBag.ma_nien_khoa = new SelectList(db.Nien_khoa, "ma_nien_khoa", "ten_nien_khoa", nganh.ma_nien_khoa);
            return View(nganh);
        }

        // POST: Nganh/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ma_nganh,ten_nganh,mo_ta_nganh,ma_nien_khoa")] Nganh nganh)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nganh).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ma_nien_khoa = new SelectList(db.Nien_khoa, "ma_nien_khoa", "ten_nien_khoa", nganh.ma_nien_khoa);
            return View(nganh);
        }

        // GET: Nganh/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nganh nganh = db.Nganhs.Find(id);
            if (nganh == null)
            {
                return HttpNotFound();
            }

            ViewBag.canh_bao = "Thao tác này sẽ xóa tất cả các lớp liên quan (Lớp chính, hoc sinh, môn học, ...) !!!";
            ViewBag.tips = "Chuyển các môn, lớp sang một ngành khác để giữ lại thông tin";

            return View(nganh);
        }

        // POST: Nganh/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    Nganh nganh = db.Nganhs.Find(id);

                    var nganh_lopchinh = db.Lop_chinh.Where(lc => lc.ma_nganh  == nganh.ma_nganh).ToList();
                    foreach( var lop_chinh in nganh_lopchinh)
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
                    foreach( var mon_hoc in nganh_monhoc)
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
