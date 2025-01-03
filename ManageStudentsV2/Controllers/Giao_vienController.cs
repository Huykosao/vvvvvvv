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

    public class Giao_vienController : Controller
    {
        private Quan_Ly_Sinh_Vien_Entities db = new Quan_Ly_Sinh_Vien_Entities();

        // GET: Giao_vien
        public ActionResult Index(String sortOrder, String currentFilter,String searchString,int? page)
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

            var giao_vien = db.Giao_vien
                                .Include(g => g.Khoa)
                                .Include(g => g.User)
                                .ToList();
            var giao_vien_list = giao_vien.AsEnumerable();
            if (!String.IsNullOrEmpty(searchString))
            {
                giao_vien_list = giao_vien_list.Where(g => g.ten_giao_vien.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name":
                    giao_vien_list = giao_vien_list.OrderByDescending(g => g.ten_giao_vien.Split(' ').LastOrDefault());
                    break;
                case "name_desc":
                    giao_vien_list = giao_vien_list.OrderBy(g => g.ten_giao_vien.Split(' ').LastOrDefault());
                    break;
                case "date_desc":
                    giao_vien_list = giao_vien_list.OrderByDescending(g => g.ma_giao_vien);
                    break;
                default:
                    giao_vien_list = giao_vien_list.OrderBy(g => g.ma_giao_vien);
                    break;
            }

            int pageSize =  10;
            int pageNumber = (page ?? 1);

            return View(giao_vien_list.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult ExportToExcel()
        {
            // Lấy danh sách giáo viên
            var giao_vien = db.Giao_vien
                              .Include(g => g.Khoa)
                              .OrderBy(g => g.ma_giao_vien)
                              .ToList();

            // Tạo nội dung file CSV
            StringBuilder sb = new StringBuilder();

            // Thêm tiêu đề cột (chấm phẩy làm dấu phân cách)
            sb.AppendLine("\"Mã GV\";\"Tên Giáo Viên\";\"Khoa\";\"Tên Tài Khoản\"");

            // Thêm dữ liệu vào file CSV
            foreach (var gv in giao_vien)
            {
                sb.AppendLine($"\"{gv.ma_giao_vien}\";" +
                              $"\"{gv.ten_giao_vien}\";" +
                              $"\"{gv.Khoa?.ten_khoa ?? "N/A"}\";" +
                              $"\"{gv.User?.Username ?? "N/A"}\"");
            }

            // Chuyển StringBuilder thành byte array với UTF-8 BOM
            byte[] fileBytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
            string fileName = "DanhSachGiaoVien.csv";

            // Trả về file CSV
            return File(fileBytes, "text/csv", fileName);
        }

        

        // GET: Giao_vien/Create

        public ActionResult Create()
        {
            ViewBag.ma_khoa = new SelectList(db.Khoas, "ma_khoa", "ten_khoa");
            ViewBag.UserID = new SelectList(db.Users, "ID", "Username");
            return View();
        }

        // POST: Giao_vien/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Create([Bind(Include = "ma_giao_vien,ten_giao_vien,ma_khoa,UserID")] Giao_vien giao_vien)
        {
            if (ModelState.IsValid)
            {
                db.Giao_vien.Add(giao_vien);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ma_khoa = new SelectList(db.Khoas, "ma_khoa", "ten_khoa", giao_vien.ma_khoa);
            ViewBag.UserID = new SelectList(db.Users, "ID", "Username", giao_vien.UserID);
            return View(giao_vien);
        }

        // GET: Giao_vien/Edit/5

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Giao_vien giao_vien = db.Giao_vien.Find(id);
            if (giao_vien == null)
            {
                return HttpNotFound();
            }
            ViewBag.ma_khoa = new SelectList(db.Khoas, "ma_khoa", "ten_khoa", giao_vien.ma_khoa);
            ViewBag.UserID = new SelectList(db.Users, "ID", "Username", giao_vien.UserID);
            return View(giao_vien);
        }

        // POST: Giao_vien/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ma_giao_vien,ten_giao_vien,ma_khoa,UserID")] Giao_vien giao_vien)
        {
            if (ModelState.IsValid)
            {
                db.Entry(giao_vien).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ma_khoa = new SelectList(db.Khoas, "ma_khoa", "ten_khoa", giao_vien.ma_khoa);
            ViewBag.UserID = new SelectList(db.Users, "ID", "Username", giao_vien.UserID);
            return View(giao_vien);
        }

        // GET: Giao_vien/Delete/5

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Giao_vien giao_vien = db.Giao_vien.Find(id);
            bool isGVCN = db.Lop_chinh.Where(lc => lc.giao_vien_chu_nhiem == giao_vien.ma_giao_vien).Any();
            if (isGVCN) {
                ViewBag.yeu_cau = "Vui lòng chọn giáo viên chủ nhiệm thay thế cho giáo viên : " + giao_vien.ten_giao_vien + " trước khi xóa !!!";
            }

            if (giao_vien == null)
            {
                return HttpNotFound();
            }
            return View(giao_vien);
        }

        // POST: Giao_vien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        

        public ActionResult DeleteConfirmed(int id)
        {
            Giao_vien giao_vien = db.Giao_vien.Find(id);
            var phanCongLienQuan = db.Phan_cong.Where(pc => pc.ma_giao_vien == giao_vien.ma_giao_vien).ToList();
            db.Phan_cong.RemoveRange(phanCongLienQuan);
            db.Giao_vien.Remove(giao_vien);
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
