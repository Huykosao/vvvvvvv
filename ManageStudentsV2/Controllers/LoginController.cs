using ManageStudentsV2.Models;
using System.Linq;
using System.Web.Mvc;

namespace ManageStudentsV2.Controllers
{
    public class LoginController : Controller
    {
        private readonly Quan_Ly_Sinh_Vien_Entities db = new Quan_Ly_Sinh_Vien_Entities();

        // GET: Login
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public ActionResult Index(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Tìm User theo Username
                var user = db.Users.FirstOrDefault(u => u.Username == model.Username);

                if (user != null)
                {
                    // Kiểm tra Password
                    if (user.PasswordHash == model.Password)
                    {
                        var role = db.Roles.FirstOrDefault(r => r.RoleID == user.RoleID);

                        // Lưu thông tin User vào Session
                        Session["UserID"] = user.ID;
                        Session["Username"] = user.Username;
                        Session["Role"] = role.RoleName;

                        //if (role != null)
                        //{
                        //    if (role.RoleName == "Student")
                        //    {
                        //        var hocSinh = db.Hoc_sinh
                        //                            .Include(hs => hs.User)
                        //                            .Include(hs => hs.Lop_chinh.Nganh.Nien_khoa.Khoa)
                        //                            .ToList();
                        //        TempData["HocSinh"] = hocSinh;
                        //        return RedirectToAction("Index", "ManageStudentHome");

                        //    }
                        //}

                        return RedirectToAction("Index", "ManageStudentHome");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Tài Khoản Hoặc Mật Khẩu Không Đúng.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Tên đăng nhập không tồn tại.");
                }
            }

            return View(model);
        }


        // Đăng Xuất
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
