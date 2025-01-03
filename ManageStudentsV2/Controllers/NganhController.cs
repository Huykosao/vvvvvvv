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
            return View(nganh);
        }

        // POST: Nganh/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Nganh nganh = db.Nganhs.Find(id);
            db.Nganhs.Remove(nganh);
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
