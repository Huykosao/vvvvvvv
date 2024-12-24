using ManageStudentsV2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManageStudentsV2.Controllers
{
    public class StudentController : Controller
    {
        // GET: Student
        [HttpGet]
        [RoleAuthorize("Student")]
        public ActionResult Index()
        {
            ViewBag.Username = Session["Username"];
            return View();
        }
    }
}