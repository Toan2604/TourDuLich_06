using System.Web.Mvc;

namespace Nhom6_TourDuLich.Areas.Admin
{
	public class AdminAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "Admin";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
			    "Admin_default",
			    "Admin/{controller}/{action}/{id}",
			    new { action = "Index", controller = "Home", id = UrlParameter.Optional }
			);//.DataTokens.Add("area", "AdminHome");
		}
	}
}