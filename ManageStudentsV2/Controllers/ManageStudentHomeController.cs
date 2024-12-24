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

namespace ManageStudentsV2.Controllers
{
    public class ManageStudentHomeController : Controller
    {
        private Quan_Ly_Sinh_Vien_Entities db = new Quan_Ly_Sinh_Vien_Entities();

        // GET: ManageStudentHome
        public ActionResult Index()
        {
            if (Session["Role"] != null)
            {
                if (Session["Role"] != null && Session["Role"].ToString() == "Student")
                {
                    if (Session["UserID"] != null)
                    {
                        int userId = Convert.ToInt32(Session["UserID"]);

                        var hoc_sinh = db.Hoc_sinh
                                         .Include(h => h.Lop_chinh.Nganh.Nien_khoa.Khoa)
                                         .Include(h => h.User)
                                         .FirstOrDefault(hs => hs.User != null && hs.User.ID == userId);

                        if (hoc_sinh != null)
                        {
                            return View(hoc_sinh);
                        }
                    }
                    return RedirectToAction("Login", "Account");
                }
            }

            return View();
        }

        public ActionResult ClassView()
        {
            ViewBag.Message = "Class manage";
            return View();
        }
    }
}