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
    //[RoleAuthorize("Admin", "Teacher")]

    public class DiemController : Controller
    {
        private Quan_Ly_Sinh_Vien_Entities db = new Quan_Ly_Sinh_Vien_Entities();

        // GET: Diem
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
            if (Session["Role"] == null)
                return RedirectToAction("Index", "ManageStudentHome");


            if (Session["Role"].ToString() == "Admin")
            {
                var diems = db.Diems.Include(d => d.Hoc_sinh).Include(d => d.Mon_hoc).OrderBy(d => d.ma_sinh_vien).ToList();
                var diems_list = diems.AsEnumerable();
                if (!String.IsNullOrEmpty(searchString))// chi thuc thi co khi co gia tri tim kiem
                {
                    diems_list = diems_list.Where(d => d.Hoc_sinh.ten_sinh_vien.Contains(searchString));
                }
                switch (sortOrder)
                {
                    case "name":
                        diems_list = diems_list.OrderBy(d => d.Hoc_sinh.ten_sinh_vien.Split(' ').LastOrDefault());
                        break;
                    case "name_desc":
                        diems_list = diems_list.OrderByDescending(d => d.Hoc_sinh.ten_sinh_vien.Split(' ').LastOrDefault());
                        break;
                    default:
                        diems_list = diems_list.OrderBy(d => d.ma_sinh_vien);
                        break;
                }
                int pageSize = 10;
                int pageNumber = (page ?? 1);

                return View(diems_list.ToPagedList(pageNumber, pageSize));
            }else if (Session["Role"].ToString() == "Teacher")
            {
                
                return View();
            }
            else if (Session["Role"].ToString() == "Student")
            {
                int userId = (int)Session["UserID"];
                int ma_sinh_vien = db.Hoc_sinh
                                         .Where(hs  => hs.UserID == userId)
                                         .Select(hs => (int)hs.ma_sinh_vien)
                                         .FirstOrDefault();

                var diems = db.Diems
                                .Where(d => d.ma_sinh_vien == ma_sinh_vien)
                                .ToList();
                var diems_list = diems.AsEnumerable();
                int pageSize = 10;
                int pageNumber = (page ?? 1);

                return View(diems_list.ToPagedList(pageNumber, pageSize));
            }

            return RedirectToAction("Index", "ManageStudentHome");

        }
        public ActionResult ExportToExcel()
        {
            // Lấy danh sách điểm
            var diem = db.Diems
                          .Include(d => d.Hoc_sinh)
                          .Include(d => d.Mon_hoc)
                          .OrderBy(d => d.ma_sinh_vien)
                          .ToList();
            // Tạo nội dung file CSV
            StringBuilder sb = new StringBuilder();
            // Thêm tiêu đề cột (chấm phẩy làm dấu phân cách)
            sb.AppendLine("\"Mã Sinh Viên\";\"Mã Môn\";\"Điểm Số 1\";\"Điểm Số 2\";\"Điểm Cuối Kỳ\"");
            // Thêm dữ liệu vào file CSV
            foreach (var d in diem)
            {
                sb.AppendLine(string.Format("\"{0}\";\"{1}\";\"{2}\";\"{3}\";\"{4}\"",
                    d.ma_sinh_vien,
                    d.ma_mon,
                    d.diem_so_1,
                    d.diem_so_2,
                    d.diem_cuoi_ky));
            }
            // Return file CSV
            return File(new System.Text.UTF8Encoding().GetBytes(sb.ToString()), "text/csv", "Diem.csv");
        }
        

        // GET: Diem/Create
        public ActionResult Create()
        {
            ViewBag.ma_sinh_vien = new SelectList(db.Hoc_sinh, "ma_sinh_vien", "ten_sinh_vien");
            ViewBag.ma_mon = new SelectList(db.Mon_hoc, "ma_mon", "ten_mon");
            return View();
        }

        // POST: Diem/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ma_sinh_vien,ma_mon,diem_so_1,diem_so_2,diem_cuoi_ky")] Diem diem)
        {
            if (ModelState.IsValid)
            {
                db.Diems.Add(diem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ma_sinh_vien = new SelectList(db.Hoc_sinh, "ma_sinh_vien", "ten_sinh_vien", diem.ma_sinh_vien);
            ViewBag.ma_mon = new SelectList(db.Mon_hoc, "ma_mon", "ten_mon", diem.ma_mon);
            return View(diem);
        }

        // GET: Diem/Edit/5
        public ActionResult Edit(int? ma_sinh_vien, int? ma_mon)
        {
            if (ma_sinh_vien == null || ma_mon == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Diem diem = db.Diems
                               .Where( d => d.ma_sinh_vien == ma_sinh_vien)
                               .Where( d => d.ma_mon == ma_mon)
                               .FirstOrDefault();

            if (diem == null)
            {
                return HttpNotFound();
            }
            ViewBag.ma_sinh_vien = new SelectList(db.Hoc_sinh, "ma_sinh_vien", "ten_sinh_vien", diem.ma_sinh_vien);
            ViewBag.ma_mon = new SelectList(db.Mon_hoc, "ma_mon", "ten_mon", diem.ma_mon);
            return View(diem);
        }

        // POST: Diem/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ma_sinh_vien,ma_mon,diem_so_1,diem_so_2,diem_cuoi_ky")] Diem diem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(diem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("NhapDiem", "Diem", new {ma_hoc_phan = db.Lop_dang_ky
                                                                                      .Where(ldk => ldk.ma_sinh_vien == diem.ma_sinh_vien)
                                                                                      .Select(ldk => ldk.ma_hoc_phan)
                                                                                      .FirstOrDefault()});
            }
            ViewBag.ma_sinh_vien = new SelectList(db.Hoc_sinh, "ma_sinh_vien", "ten_sinh_vien", diem.ma_sinh_vien);
            ViewBag.ma_mon = new SelectList(db.Mon_hoc, "ma_mon", "ten_mon", diem.ma_mon);
            return View(diem);
        }

        // GET: Diem/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Diem diem = db.Diems.Find(id);
            if (diem == null)
            {
                return HttpNotFound();
            }
            return View(diem);
        }

        // POST: Diem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Diem diem = db.Diems.Find(id);
            db.Diems.Remove(diem);
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


        public ActionResult NhapDiem(int? ma_hoc_phan)
        {
            if (ma_hoc_phan == null)
            {
                return RedirectToAction("Index", "ManageStudentHome");
            }

            var ma_sinh_vien_list = db.Lop_dang_ky
                                       .Where(ldk => ldk.ma_hoc_phan == ma_hoc_phan)
                                       .Select(ldk => ldk.ma_sinh_vien)
                                       .ToList();

            var ma_mon = db.Lop_hoc_phan
                           .Where(lhp => lhp.ma_hoc_phan == ma_hoc_phan)
                           .Select(lhp => lhp.ma_mon)
                           .FirstOrDefault();

            if (ma_mon == 0)
            {
                return RedirectToAction("Index");
            }

            foreach (int ma_sinh_vien in ma_sinh_vien_list)
            {
                var diemExist = db.Diems
                                  .Any(d => d.ma_sinh_vien == ma_sinh_vien && d.ma_mon == ma_mon);

                if (!diemExist)
                {
                    Diem diem = new Diem
                    {
                        ma_sinh_vien = ma_sinh_vien,
                        ma_mon = ma_mon,
                        diem_so_1 = 0, 
                        diem_so_2 = 0, 
                        diem_cuoi_ky = 0
                    };

                    db.Diems.Add(diem);
                }
            }

            db.SaveChanges();

            var lophocphan_diem = db.Diems
                                       .Where(d => ma_sinh_vien_list.Contains(d.ma_sinh_vien) && d.ma_mon == ma_mon)
                                       .Select(d => new ManageStudentsV2.Models.DiemViewModel
                                       {
                                           MaSinhVien = d.ma_sinh_vien,
                                           TenSinhVien = d.Hoc_sinh.ten_sinh_vien,
                                           LopChinh = d.Hoc_sinh.Lop_chinh.ten_lop,
                                           MaMon = d.ma_mon,
                                           TenMon = d.Mon_hoc.ten_mon,
                                           LopHocPhan = db.Lop_hoc_phan
                                                            .Where(lhp => lhp.ma_hoc_phan == ma_hoc_phan)
                                                            .Select(lhp => lhp.Lop_chinh.ten_lop)
                                                            .FirstOrDefault(),
                                           MaHocPhan = ma_hoc_phan,
                                           DiemSo1 = (double)d.diem_so_1,
                                           DiemSo2 = (double)d.diem_so_2,
                                           DiemCuoiKy = (double)d.diem_cuoi_ky,
                                       })
                                       .ToList();

            return View(lophocphan_diem);
        }

        [HttpPost]
        public ActionResult SubmitScores(List<ManageStudentsV2.Models.DiemViewModel> diemViewModels)
        {
            int ma_hoc_phan = 0;

            if (diemViewModels == null || !diemViewModels.Any())
            {
                if (Request.Form.AllKeys.Any(key => key.StartsWith("diemViewModels[0]")))
                {
                    try
                    {
                        var singleDiemViewModel = new DiemViewModel
                        {
                            MaSinhVien = int.Parse(Request.Form["diemViewModels[0].MaSinhVien"] ?? "0"),
                            MaMon = int.Parse(Request.Form["diemViewModels[0].MaMon"] ?? "0"),
                            DiemSo1 = double.TryParse(Request.Form["diemViewModels[0].DiemSo1"], out var diemSo1) ? diemSo1 : 0,
                            DiemSo2 = double.TryParse(Request.Form["diemViewModels[0].DiemSo2"], out var diemSo2) ? diemSo2 : 0,
                            DiemCuoiKy = double.TryParse(Request.Form["diemViewModels[0].DiemCuoiKy"], out var diemCuoiKy) ? diemCuoiKy : 0,
                            MaHocPhan = int.Parse(Request.Form["diemViewModels[0].MaHocPhan"] ?? "0")
                        };

                        diemViewModels = new List<DiemViewModel> { singleDiemViewModel };
                    }
                    catch (Exception)
                    {
                        TempData["ErrorMessage"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                        return RedirectToAction("Index", "ManageStudentHome");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Không có dữ liệu điểm được gửi lên.";
                    return RedirectToAction("Index", "ManageStudentHome");
                }
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var diemViewModel in diemViewModels)
                    {
                        var diem = db.Diems
                                     .FirstOrDefault(d => d.ma_sinh_vien == diemViewModel.MaSinhVien && d.ma_mon == diemViewModel.MaMon);

                        ma_hoc_phan = (int)diemViewModel.MaHocPhan;

                        if (diem != null)
                        {
                            diem.diem_so_1 = diemViewModel.DiemSo1;
                            diem.diem_so_2 = diemViewModel.DiemSo2;
                            diem.diem_cuoi_ky = diemViewModel.DiemCuoiKy;
                        }
                        else
                        {
                            db.Diems.Add(new Diem
                            {
                                ma_sinh_vien = diemViewModel.MaSinhVien,
                                ma_mon = diemViewModel.MaMon,
                                diem_so_1 = diemViewModel.DiemSo1,
                                diem_so_2 = diemViewModel.DiemSo2,
                                diem_cuoi_ky = diemViewModel.DiemCuoiKy
                            });
                        }
                    }

                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    TempData["ErrorMessage"] = "Đã xảy ra lỗi trong quá trình cập nhật dữ liệu.";
                    return RedirectToAction("Index", "ManageStudentHome");
                }
            }

            return RedirectToAction("NhapDiem", "Diem", new { ma_hoc_phan });
        }


    }
}
