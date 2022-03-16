using Nhom6_TourDuLich.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nhom6_TourDuLich.Areas.Admin.Controllers
{
	public class BaseController : Controller
	{
		// GET: Admin/Base
		protected override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			var sesion = (UserSession)Session[SessionConnection.ADMIN_SESSION];
			if (sesion == null)
			{
				filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { controller = "Users", action = "Login" }));
			}
			base.OnActionExecuted(filterContext);
		}

	}
}