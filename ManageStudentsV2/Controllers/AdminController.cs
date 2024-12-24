using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManageStudentsV2.Controllers
{
    public class AdminController : BaseController
    {
        // GET: Admin
        [RoleAuthorize("Admin")]

        public ActionResult Index()
        {
            ViewBag.Username = Session["Username"];

            return View();
        }
    }
}