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
using WebGrease.Activities;

namespace ManageStudentsV2.Controllers
{
    [RoleAuthorize("Admin")]

    public class Hoc_sinhController : Controller
    {
        private Quan_Ly_Sinh_Vien_Entities db = new Quan_Ly_Sinh_Vien_Entities();

        // GET: Hoc_sinh
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

            var hoc_sinh = db.Hoc_sinh
                                .Include(h => h.Lop_chinh.Nganh.Nien_khoa.Khoa) // Gồm các liên kết bảng
                                .Include(h => h.User) // Bao gồm thông tin tài khoản người dùng
                                .OrderBy(h => h.ma_sinh_vien) // Thêm sắp xếp để đảm bảo thứ tự nhất quán
                                .ToList();
            var hoc_sinh_list = hoc_sinh.AsEnumerable();
            if (!String.IsNullOrEmpty(searchString))
            {
                hoc_sinh_list = hoc_sinh_list.Where(h => h.ten_sinh_vien.Contains(searchString));
            }
            switch(sortOrder)
            {
                case "name":
                    hoc_sinh_list = hoc_sinh_list.OrderBy(h => h.ten_sinh_vien.Split(' ').LastOrDefault());
                    break;
                case "name_desc":
                    hoc_sinh_list = hoc_sinh_list.OrderByDescending(h => h.ten_sinh_vien.Split(' ').LastOrDefault());
                    break;
                default:
                    hoc_sinh_list = hoc_sinh_list.OrderBy(h => h.ma_sinh_vien);
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(hoc_sinh_list.ToPagedList(pageNumber, pageSize));

            //var Hoc_sinh = from hs in db.Hoc_sinh
            //               join lh in db.Lop_chinh on hs.ma_lop equals lh.ma_lop
            //               join nganh in db.Nganhs on lh.ma_nganh equals nganh.ma_nganh
            //               join nien_khoa in db.Nien_khoa on nganh.ma_nien_khoa equals nien_khoa.ma_nien_khoa
            //               join khoa in db.Khoas on nien_khoa.ma_khoa equals khoa.ma_khoa
            //               select new
            //               {
            //                   HocSinh = hs,
            //                   Lop_chinh = lh,
            //                   Nganh = nganh,
            //                   Nien_khoa = nien_khoa,
            //                   Khoa = khoa,
            //               };
        }
        //in excel
        public ActionResult ExportToExcel()
        {
            // Lấy danh sách sinh viên
            var hoc_sinh = db.Hoc_sinh
                             .Include(h => h.Lop_chinh.Nganh.Nien_khoa.Khoa)
                             .OrderBy(h => h.ma_sinh_vien)
                             .ToList();

            // Tạo nội dung file CSV
            StringBuilder sb = new StringBuilder();

            // Thêm tiêu đề cột (chấm phẩy làm dấu phân cách)
            sb.AppendLine("\"MSSV\";\"Tên Sinh Viên\";\"Lớp\";\"Ngành\";\"Niên Khóa\";\"Khoa\"");

            // Thêm dữ liệu vào file CSV
            foreach (var hs in hoc_sinh)
            {
                sb.AppendLine($"\"{hs.ma_sinh_vien}\";" +
                              $"\"{hs.ten_sinh_vien}\";" +
                              $"\"{hs.Lop_chinh?.ten_lop ?? "N/A"}\";" +
                              $"\"{hs.Lop_chinh?.Nganh?.ten_nganh ?? "N/A"}\";" +
                              $"\"{hs.Lop_chinh?.Nganh?.Nien_khoa?.ten_nien_khoa ?? "N/A"}\";" +
                              $"\"{hs.Lop_chinh?.Nganh?.Nien_khoa?.Khoa?.ten_khoa ?? "N/A"}\"");
            }

            // Chuyển StringBuilder thành byte array với UTF-8 BOM
            byte[] fileBytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
            string fileName = "DanhSachSinhVien.csv";

            // Trả về file CSV
            return File(fileBytes, "text/csv", fileName);
        }

        // GET: Hoc_sinh/Create
        public ActionResult Create()
        {
            ViewBag.ma_lop = new SelectList(db.Lop_chinh, "ma_lop", "ten_lop");
            var usersNotReferenced = db.Users
                                      .Where(user => !db.Hoc_sinh.Any(hs => hs.UserID == user.ID))
                                      .Where(user => !db.Giao_vien.Any(gv => gv.UserID == user.ID))
                                      .ToList();

            // Truyền vào ViewBag cho dropdown list
            ViewBag.UserID = new SelectList(usersNotReferenced, "ID", "Username");
            return View();
        }

        // POST: Hoc_sinh/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ma_sinh_vien,ten_sinh_vien,ma_lop,UserID")] Hoc_sinh hoc_sinh)
        {
            if (ModelState.IsValid)
            {
                db.Hoc_sinh.Add(hoc_sinh);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ma_lop = new SelectList(db.Lop_chinh, "ma_lop", "ten_lop", hoc_sinh.ma_lop);
            ViewBag.UserID = new SelectList(db.Users, "ID", "Username", hoc_sinh.UserID);
            return View(hoc_sinh);
        }

        // GET: Hoc_sinh/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hoc_sinh hoc_sinh = db.Hoc_sinh.Find(id);
            if (hoc_sinh == null)
            {
                return HttpNotFound();
            }
            ViewBag.ma_lop = new SelectList(db.Lop_chinh, "ma_lop", "ten_lop", hoc_sinh.ma_lop);
            ViewBag.UserID = new SelectList(db.Users, "ID", "Username", hoc_sinh.UserID);
            return View(hoc_sinh);
        }

        // POST: Hoc_sinh/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ma_sinh_vien,ten_sinh_vien,ma_lop,UserID")] Hoc_sinh hoc_sinh)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hoc_sinh).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ma_lop = new SelectList(db.Lop_chinh, "ma_lop", "ten_lop", hoc_sinh.ma_lop);
            ViewBag.UserID = new SelectList(db.Users, "ID", "Username", hoc_sinh.UserID);
            return View(hoc_sinh);
        }

        // GET: Hoc_sinh/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hoc_sinh hoc_sinh = db.Hoc_sinh.Find(id);
            if (hoc_sinh == null)
            {
                return HttpNotFound();
            }
            ViewBag.canh_bao = "Thao tác này sẽ xóa hoc sinh: " + hoc_sinh.ten_sinh_vien + " khỏi các lớp liên quan !!!";
            return View(hoc_sinh);
        }

        // POST: Hoc_sinh/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    
                    Hoc_sinh hoc_sinh = db.Hoc_sinh.Find(id);
                    if (hoc_sinh == null)
                    {
                        return HttpNotFound();
                    }

                    var hocsinh_lopdangky = db.Lop_dang_ky.Where(ldk => ldk.ma_sinh_vien == hoc_sinh.ma_sinh_vien).ToList();
                    var hocsinh_diem = db.Diems.Where(d => d.ma_sinh_vien == hoc_sinh.ma_sinh_vien).ToList();
                    db.Diems.RemoveRange(hocsinh_diem);
                    db.Lop_dang_ky.RemoveRange(hocsinh_lopdangky);
                    db.Hoc_sinh.Remove(hoc_sinh);
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
            //Hoc_sinh hoc_sinh = db.Hoc_sinh.Find(id);
            //db.Hoc_sinh.Remove(hoc_sinh);
            //db.SaveChanges();
            //return RedirectToAction("Index");
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
