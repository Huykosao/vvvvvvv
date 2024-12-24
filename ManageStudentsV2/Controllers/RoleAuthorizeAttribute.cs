using System.Linq;
using System.Web.Mvc;

public class RoleAuthorizeAttribute : ActionFilterAttribute
{
    private readonly string[] _roles;

    public RoleAuthorizeAttribute(params string[] roles)
    {
        _roles = roles;
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var session = filterContext.HttpContext.Session;

        // Kiểm tra đăng nhập
        if (session["UserID"] == null)
        {
            filterContext.Result = new RedirectResult("~/Login");
            return;
        }

        // Kiểm tra quyền
        var userRole = session["Role"].ToString();
        if (!_roles.Contains(userRole))
        {
            filterContext.Result = new RedirectResult("~/");
            return;
        }

        base.OnActionExecuting(filterContext);
    }
}
