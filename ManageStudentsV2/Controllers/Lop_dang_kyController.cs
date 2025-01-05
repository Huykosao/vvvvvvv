using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ManageStudentsV2.Models;
using PagedList;

namespace ManageStudentsV2.Controllers
{
    //[RoleAuthorize("Admin")]
    public class Lop_dang_kyController : Controller
    {
        private Quan_Ly_Sinh_Vien_Entities db = new Quan_Ly_Sinh_Vien_Entities();

        // GET: Lop_dang_ky

        //[RoleAuthorize("Admin")]
        public ActionResult Index(String sortOrder, String currentFilter, String searchString, int? page)
        {
            ViewBag.currentSort = sortOrder;
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
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
                var lop_dang_ky = db.Lop_dang_ky
                                        .OrderBy(ldk => ldk.ngay_dk);

                var lop_dang_ky_list = lop_dang_ky.AsEnumerable();
                if (!String.IsNullOrEmpty(searchString))
                {   
                    lop_dang_ky_list = lop_dang_ky_list.Where(ldk => ldk.Hoc_sinh.ten_sinh_vien.Contains(searchString));
                }
                switch (sortOrder)
                {
                    case "name":
                        lop_dang_ky_list = lop_dang_ky_list.OrderBy(ldk => ldk.Hoc_sinh.ten_sinh_vien.Split(' ').LastOrDefault());
                        break;
                    case "name_desc":
                        lop_dang_ky_list = lop_dang_ky_list.OrderByDescending(ldk => ldk.Hoc_sinh.ten_sinh_vien.Split(' ').LastOrDefault());
                        break;
                    case "date_desc":
                        lop_dang_ky_list = lop_dang_ky_list.OrderByDescending(ldk => ldk.ngay_dk);
                        break;
                    default:
                        lop_dang_ky_list = lop_dang_ky_list.OrderBy(ldk => ldk.ngay_dk);
                        break;
                }
                int pageSize = 10;
                int pageNumber = (page ?? 1);

                return View(lop_dang_ky_list.ToPagedList(pageNumber, pageSize));
            }
            else if (Session["Role"].ToString() == "Student")
            {
                int userId = Convert.ToInt32(Session["UserID"]);
                var hoc_sinh = db.Hoc_sinh.FirstOrDefault(hs => hs.UserID == userId);
                if (hoc_sinh == null)
                    return View();

                var lop_dang_ky = db.Lop_dang_ky
                                    .Include(ldk => ldk.Lop_hoc_phan.Mon_hoc)
                                    .Include(ldk => ldk.Lop_hoc_phan.Lop_chinh)
                                    .Include(ldk => ldk.Lop_hoc_phan.Phan_cong)
                                    .Include(ldk => ldk.Hoc_sinh)
                                    .Where(ldk => ldk.Hoc_sinh.ma_sinh_vien == hoc_sinh.ma_sinh_vien)
                                    .ToList();
                var lop_dang_ky_list = lop_dang_ky.AsEnumerable();
                if (!String.IsNullOrEmpty(searchString))
                {
                    lop_dang_ky_list = lop_dang_ky_list.Where(ldk => ldk.Hoc_sinh.ten_sinh_vien.Contains(searchString));
                }
                switch (sortOrder)
                {
                    case "name":
                        lop_dang_ky_list = lop_dang_ky_list.OrderBy(ldk => ldk.Hoc_sinh.ten_sinh_vien.Split(' ').LastOrDefault());
                        break;
                    case "name_desc":
                        lop_dang_ky_list = lop_dang_ky_list.OrderByDescending(ldk => ldk.Hoc_sinh.ten_sinh_vien.Split(' ').LastOrDefault());
                        break;
                    case "date_desc":
                        lop_dang_ky_list = lop_dang_ky_list.OrderByDescending(ldk => ldk.ngay_dk);
                        break;
                    default:
                        lop_dang_ky_list = lop_dang_ky_list.OrderBy(ldk => ldk.ngay_dk);
                        break;
                }
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                return View(lop_dang_ky_list.ToPagedList(pageNumber, pageSize));
            }
            else if (Session["Role"].ToString() == "Teacher")
            {
                int userId = Convert.ToInt32(Session["UserID"]);

                return View();
            }

            return View();
        }


        // GET: Lop_dang_ky/Create
        [RoleAuthorize("Admin", "Student")]

        public ActionResult Create()
        {
            if(Session["Role"] == null)
                return RedirectToAction("Index", "ManageStudentHome");


            if (Session["Role"].ToString() == "Admin")
            {
                ViewBag.ma_sinh_vien = new SelectList(db.Hoc_sinh, "ma_sinh_vien", "ten_sinh_vien");
                ViewBag.ma_hoc_phan = new SelectList(db.Lop_hoc_phan, "ma_hoc_phan", "ma_hoc_phan");
                return View();
            }
            else if (Session["Role"].ToString() == "Student")
            {
                int userId = Convert.ToInt32(Session["UserID"]);
                ViewBag.ma_sinh_vien = new SelectList(db.Hoc_sinh
                                                        .Where(hs => hs.UserID == userId)
                                                        .Select(hs => new
                                                        {
                                                            ma_sinh_vien = hs.ma_sinh_vien,
                                                            ten_sinh_vien = hs.ten_sinh_vien
                                                        }), "ma_sinh_vien", "ten_sinh_vien");

                var hoc_sinh = db.Hoc_sinh
                                    .Where(hs => hs.UserID == userId)
                                    .Select(hs => new
                                    {
                                        maSinhVien = hs.ma_sinh_vien,
                                        maNganh = hs.Lop_chinh.Nganh.ma_nganh
                                    })
                                    .FirstOrDefault();

                var maHocPhan = db.Lop_dang_ky
                                        .Where(ldk => ldk.ma_sinh_vien == hoc_sinh.maSinhVien)
                                        .Select(ldk => ldk.ma_hoc_phan)
                                        .ToList();
                                        

                ViewBag.ma_hoc_phan = new SelectList(db.Lop_hoc_phan
                                            .Include(lhp => lhp.Mon_hoc)
                                            .Where(lhp => lhp.Mon_hoc.Nganh.ma_nganh == hoc_sinh.maNganh)
                                            .Where(lhp => !maHocPhan.Contains(lhp.ma_hoc_phan)) 
                                            .Select(lhp => new
                                            {
                                                ma_hoc_phan = lhp.ma_hoc_phan,
                                                mon_hoc = lhp.Mon_hoc.ten_mon,
                                                giao_vien = lhp.Phan_cong.Any() ? lhp.Phan_cong.FirstOrDefault().Giao_vien.ten_giao_vien : "",
                                                mon_hoc_giao_vien = lhp.Mon_hoc.ten_mon + " - " + (lhp.Phan_cong.Any() ? lhp.Phan_cong.FirstOrDefault().Giao_vien.ten_giao_vien : "")
                                            }), "ma_hoc_phan", "mon_hoc_giao_vien");

                return View();
            }
            else if (Session["Role"].ToString() == "Teacher")
            {
                int userId = Convert.ToInt32(Session["UserID"]);
                return View();
            }

            return RedirectToAction("Index", "ManageStudentHome");
        }

        [RoleAuthorize("Admin", "Student")]
        // POST: Lop_dang_ky/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ma_sinh_vien,ma_hoc_phan")] Lop_dang_ky lop_dang_ky)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    lop_dang_ky.ngay_dk = DateTime.Now; // Automatically set the registration date
                    db.Lop_dang_ky.Add(lop_dang_ky);
                    db.SaveChanges();
                }
                catch (Exception error)
                {
                    Console.WriteLine("Hello World!" + error);
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }

            ViewBag.ma_sinh_vien = new SelectList(db.Hoc_sinh, "ma_sinh_vien", "ten_sinh_vien", lop_dang_ky.ma_sinh_vien);
            ViewBag.ma_hoc_phan = new SelectList(db.Lop_hoc_phan, "ma_hoc_phan", "ma_hoc_phan", lop_dang_ky.ma_hoc_phan);
            return View(lop_dang_ky);
        }
        // GET: Lop_dang_ky/Edit/5
        //[RoleAuthorize("Admin")]
        public ActionResult Edit(int? ma_sinh_vien, int? ma_hoc_phan)
        {
            if (ma_sinh_vien == null || ma_hoc_phan == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lop_dang_ky lop_dang_ky = db.Lop_dang_ky
                                              .Where(ldk => ldk.ma_sinh_vien == ma_sinh_vien)
                                              .Where(ldk => ldk.ma_hoc_phan == ma_hoc_phan)
                                              .FirstOrDefault();

            if (lop_dang_ky == null)
            {
                return HttpNotFound();
            }
            ViewBag.ma_sinh_vien = new SelectList(db.Hoc_sinh, "ma_sinh_vien", "ten_sinh_vien", lop_dang_ky.ma_sinh_vien);
            ViewBag.ma_hoc_phan = new SelectList(db.Lop_hoc_phan, "ma_hoc_phan", "ma_hoc_phan", lop_dang_ky.ma_hoc_phan);
            return View(lop_dang_ky);
        }
        
        // POST: Lop_dang_ky/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ma_sinh_vien,ma_hoc_phan,ngay_dk")] Lop_dang_ky lop_dang_ky)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lop_dang_ky).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ma_sinh_vien = new SelectList(db.Hoc_sinh, "ma_sinh_vien", "ten_sinh_vien", lop_dang_ky.ma_sinh_vien);
            ViewBag.ma_hoc_phan = new SelectList(db.Lop_hoc_phan, "ma_hoc_phan", "ma_hoc_phan", lop_dang_ky.ma_hoc_phan);
            return View(lop_dang_ky);
        }
        
        // GET: Lop_dang_ky/Delete/5
        [RoleAuthorize("Admin","Student")]
        public ActionResult Delete(int? ma_sinh_vien, int? ma_hoc_phan)
        {
            if (Session["Role"].ToString() == "Student")
            {
                int userId = Convert.ToInt32(Session["UserID"]);
                int? MaSinhVien = db.Hoc_sinh.Where(hs => hs.UserID == userId).Select(hs => hs.ma_sinh_vien).FirstOrDefault();
                if(MaSinhVien != ma_sinh_vien)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (ma_sinh_vien == null || ma_hoc_phan == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lop_dang_ky lop_dang_ky = db.Lop_dang_ky
                                              .Where(ldk => ldk.ma_sinh_vien == ma_sinh_vien)
                                              .Where(ldk => ldk.ma_hoc_phan == ma_hoc_phan)
                                              .FirstOrDefault();

            if (lop_dang_ky == null)
            {
                return HttpNotFound();
            }
            return View(lop_dang_ky);
        }
        [RoleAuthorize("Admin", "Student")]
        // POST: Lop_dang_ky/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? ma_sinh_vien, int? ma_hoc_phan)
        {
            Lop_dang_ky lop_dang_ky = db.Lop_dang_ky
                                               .Where(ldk => ldk.ma_sinh_vien == ma_sinh_vien)
                                               .Where(ldk => ldk.ma_hoc_phan == ma_hoc_phan)
                                               .FirstOrDefault();
            db.Lop_dang_ky.Remove(lop_dang_ky);
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

        public ActionResult Get_lopdangky_hocsinh()
        {
            //var ma_sinh_vien = 0;
            var lop_dang_ky = db.Lop_dang_ky
                                    .Include(ldk => ldk.Hoc_sinh)
                                    .ToList();

            return View(lop_dang_ky);
        }
    }
}
