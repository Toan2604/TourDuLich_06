using Nhom6_TourDuLich.Areas.Admin.Data;
using Nhom6_TourDuLich.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nhom6_TourDuLich.Areas.Admin.Controllers
{
	public class HomeController : BaseController
	{
		private CSDL_TOURDB db = new CSDL_TOURDB();
		DataProcessing data = new DataProcessing();
		public ActionResult Index()
		{
			var cate = db.Categories.Where(x => x.StatusDelete==1);
			var tour = db.Tours.Where(x => x.StatusDelete==1);
			var user = db.User.Where(x => x.StatusDelete==1 && x.Roles==0);
			var order = db.Orders.Where(x => x.StatusDelete==1);

			ViewBag.cate = cate.Count();
			ViewBag.tour = tour.Count();
			ViewBag.user = user.Count();
			ViewBag.order = order.Count();

			var tours = data.GetListTourMaxByBooked(3);

			return View(tours);
		}

	}
}