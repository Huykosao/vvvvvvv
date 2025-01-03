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
    public class Mon_hocController : Controller
    {
        private Quan_Ly_Sinh_Vien_Entities db = new Quan_Ly_Sinh_Vien_Entities();

        // GET: Mon_hoc
        public ActionResult Index(String sortOrder,String currterFilter,String searchString,int? page)
        {
            
            ViewBag.currentSort = sortOrder;
            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currterFilter;
            }
            ViewBag.currentFilter = searchString;


            var mon_hoc = db.Mon_hoc
                            .Include(mh => mh.Lop_hoc_phan.Select(lhp => lhp.Lop_chinh))
                            .Include(mh => mh.Nganh)
                            .OrderBy(mh => mh.ma_mon)
                            .ToList();

            if (Session["Role"] != null && Session["Role"].ToString() == "Student")
            {
                int userId = Convert.ToInt32(Session["UserID"]);
                int student_majoring = db.Hoc_sinh
                                            .Include(hs => hs.Lop_chinh.Nganh)
                                            .Select(hs => hs.Lop_chinh.Nganh.ma_nganh)
                                            .FirstOrDefault();
                mon_hoc = db.Mon_hoc
                                    .Include(mh => mh.Lop_hoc_phan.Select(lhp => lhp.Lop_chinh))
                                    .Include(mh => mh.Nganh)
                                    .OrderBy(mh => mh.ma_mon)
                                    .Where(mh => mh.ma_nganh == student_majoring)
                                    .ToList();

            }
           
               
            

            var mon_hoc_list = mon_hoc.AsEnumerable();
            if (!String.IsNullOrEmpty(searchString))
            {
                mon_hoc_list = mon_hoc_list.Where(mh => mh.ten_mon.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name":
                    mon_hoc_list = mon_hoc_list.OrderBy(mh => mh.ten_mon.Split(' ').LastOrDefault());
                    break;
                case "name_desc":
                    mon_hoc_list = mon_hoc_list.OrderByDescending(mh => mh.ten_mon.Split(' ').LastOrDefault());
                    break;
                default:
                    mon_hoc_list = mon_hoc_list.OrderBy(mh => mh.ma_mon);
                    break;
            }
            int pageSize = 10;

            int pageNumber = (page ?? 1);

            return View(mon_hoc_list.ToPagedList(pageNumber, pageSize));
        }
        [RoleAuthorize("Admin")]
        public ActionResult ExportToExcel()
        {
            // Lấy danh sách môn học
            var mon_hoc = db.Mon_hoc
                            .Include(m => m.Nganh)
                            .OrderBy(m => m.ma_mon)
                            .ToList();

            // Tạo nội dung file CSV
            StringBuilder sb = new StringBuilder();

            // Thêm tiêu đề cột (chấm phẩy làm dấu phân cách)
            sb.AppendLine("\"Mã Môn\";\"Tên Môn\";\"Mô Tả Môn\";\"Ngành\"");

            // Thêm dữ liệu vào file CSV
            foreach (var m in mon_hoc)
            {
                sb.AppendLine($"\"{m.ma_mon}\";" +
                              $"\"{m.ten_mon}\";" +
                              $"\"{m.mo_ta_mon}\";" +
                              $"\"{m.Nganh?.ten_nganh ?? "N/A"}\"");
            }

            // Chuyển StringBuilder thành byte array với UTF-8 BOM
            byte[] fileBytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
            string fileName = "DanhSachMonHoc.csv";

            // Trả về file CSV
            return File(fileBytes, "text/csv", fileName);
        }


        // GET: Mon_hoc/Create
        [RoleAuthorize("Admin")]
        public ActionResult Create()
        {
            ViewBag.ma_nganh = new SelectList(db.Nganhs, "ma_nganh", "ten_nganh");
            return View();
        }

        // POST: Mon_hoc/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [RoleAuthorize("Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ten_mon,mo_ta_mon, ma_nganh")] Mon_hoc mon_hoc)
        {
            if (ModelState.IsValid)
            {
                db.Mon_hoc.Add(mon_hoc);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mon_hoc);
        }
        [RoleAuthorize("Admin")]
        // GET: Mon_hoc/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mon_hoc mon_hoc = db.Mon_hoc.Find(id);
            if (mon_hoc == null)
            {
                return HttpNotFound();
            }
            return View(mon_hoc);
        }
        [RoleAuthorize("Admin")]

        // POST: Mon_hoc/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ma_mon,ten_mon,mo_ta_mon")] Mon_hoc mon_hoc)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mon_hoc).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mon_hoc);
        }
        [RoleAuthorize("Admin")]
        // GET: Mon_hoc/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mon_hoc mon_hoc = db.Mon_hoc.Find(id);
            if (mon_hoc == null)
            {
                return HttpNotFound();
            }

            ViewBag.canh_bao = "Thao tác này sẽ xóa các lớp liên quan (Lớp học phần, Điểm, Phân công, Lịch Học, Lớp đăng ký) !!!";
            return View(mon_hoc);
        }
        [RoleAuthorize("Admin")]
        // POST: Mon_hoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {

                    Mon_hoc mon_hoc = db.Mon_hoc.Find(id);
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
