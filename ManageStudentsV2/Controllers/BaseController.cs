using System.Web.Mvc;

public class BaseController : Controller
{
    // Hàm kiểm tra đăng nhập
    protected bool IsLoggedIn()
    {
        return Session["UserID"] != null;
    }

    // Hàm kiểm tra quyền
    protected bool HasRole(string requiredRole)
    {
        return Session["Role"].ToString() == requiredRole;
    }

    // Hàm kiểm tra và chuyển hướng nếu không đủ quyền
    protected ActionResult AuthorizeRole(string requiredRole)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Index", "Login");
        }

        if (!HasRole(requiredRole))
        {
            return RedirectToAction("Index", "ManageStudentHome");
        }

        return null;
    }
}
