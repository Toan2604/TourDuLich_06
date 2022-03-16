using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nhom6_TourDuLich.Models;
using Nhom6_TourDuLich.Models.Custom;

namespace Nhom6_TourDuLich.Controllers
{
	public class VietTravelController : Controller
	{

		//CSDL_TOURDB db = new CSDL_TOURDB();
		DataProcessing data = new DataProcessing();
		
		// GET: VietTravel
		public ActionResult Index()
		{
			ModelState.Clear();
			ViewBag.tour = data.GetListViewTour().OrderByDescending(x => x.NumberBooked).Take(12);
			var category = data.GetListCategory();
			return View(category);
		}
		
		
		// trang xem News - tin tức
		public ActionResult News()
		{
			var news = data.GetListNews();
			return View(news);
		}

		// trang xem AboutUs - giới thiệu
		public ActionResult AboutUs()
		{
			return View();
		}

	}
}