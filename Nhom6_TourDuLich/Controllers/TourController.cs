using Nhom6_TourDuLich.Models;
using Nhom6_TourDuLich.Models.Custom;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nhom6_TourDuLich.Controllers
{
	public class TourController : Controller
	{
		CSDL_TOURDB db = new CSDL_TOURDB();
		DataProcessing data = new DataProcessing();

		int pageSize = 9;
		// trang xem Tour
		public ActionResult Tour(int? page)
		{
			var category = new List<Category>(data.GetListCategory().OrderBy(x => x.CategoryName));
			var tour = data.GetListViewTour().OrderBy(x => x.TourName);
			ViewData["IDCategory"] = new SelectList(db.Categories.Where(x => x.StatusDelete == 1 && x.Quantity > 0), "IDCategory", "CategoryName");

			int pageNumber = page ?? 1;
			var pagedTour = new PagedListTour();
			pagedTour.PageTour = tour.ToPagedList(pageNumber, pageSize);
			pagedTour.Categories = category;

			//Tuple<List<Category>, List<ViewTour>, IPagedList<PagedListTour>> tuple = new Tuple<List<Category>, List<ViewTour>, IPagedList<PagedListTour>>(category,new List<ViewTour>(tour), (IPagedList < PagedListTour >)pagedTour);

			return View(pagedTour);
		}
		public ActionResult Details(int id)
		{
			var tour = db.Tours.Find(id);
			return View(tour);
		}
		public ActionResult TourByCategory(int idCategory, int? page)
		{
			var category = new List<Category>(data.GetListCategory().OrderBy(x => x.CategoryName));
			var tour = data.GetListTourByIDCategory(idCategory);
			ViewData["IDCategory"] = new SelectList(db.Categories.Where(x => x.StatusDelete == 1 && x.Quantity > 0), "IDCategory", "CategoryName");//db.Categories.Where(x=>x.StatusDelete==1 && x.Quantity>0)

			int pageNumber = page ?? 1;
			var pagedTour = new PagedListTour();
			pagedTour.PageTour = tour.ToPagedList(pageNumber, pageSize);
			pagedTour.Categories = category;
			//Tuple<List<Category>, List<ViewTour>> tuple = new Tuple<List<Category>, List<ViewTour>>(category, new List<ViewTour>(tour));

			return View("Tour", pagedTour);
		}
		public ActionResult FindListTour(int? page, FindTours finds)
		{
			bool flat = false;
			var category = new List<Category>(data.GetListCategory().OrderBy(x => x.CategoryName));
			var listVTour = data.GetListViewTour();
			if (!String.IsNullOrEmpty(finds.IDCategory))
			{
				int id = int.Parse(finds.IDCategory);
				var categorynames = listVTour.Where(x => x.IDCategory == id);
				if (categorynames.Count() > 0)
				{
					listVTour = new List<ViewTour>(categorynames);
					ViewBag.CategoryName = id;
					flat = true;
				}
				else
				{
					page = 1;
				}
			}
			else { flat = true; }
			if (!String.IsNullOrEmpty(finds.TourName))
			{
				var tournames = listVTour.Where(x => x.TourName.ToLower().Contains(finds.TourName.ToLower()));
				if (tournames.Count() > 0)
				{
					listVTour = new List<ViewTour>(tournames);
					ViewBag.tournames = finds.TourName;
					flat = true;
				}
				else
				{
					flat = false;
					page = 1;
				}
			}

			if (finds.CostA > 0)
			{
				var costAs = listVTour.Where(x => x.Cost >= finds.CostA);
				if (costAs.Count() > 0)
				{
					listVTour = new List<ViewTour>(costAs);
					ViewBag.costA = finds.CostA;
					flat = true;
				}
				else
				{
					flat = false;
					page = 1;
				}
			}
			if (finds.CostB > 0)
			{
				var costBs = listVTour.Where(x => x.Cost <= finds.CostB);
				if (costBs.Count() > 0)
				{
					listVTour = new List<ViewTour>(costBs);
					ViewBag.costB = finds.CostB;
					flat = true;
				}
				else
				{
					flat = false;
					page = 1;
				}
			}
			if (!flat)
			{
				ViewBag.Error = "Không tìm thấy. Vui lòng nhập lại!";
			}

			SelectList items = new SelectList(db.Categories.Where(x => x.StatusDelete == 1 && x.Quantity > 0), "IDCategory", "CategoryName");
			foreach (var item in items)
			{
				if (item.Value == finds.IDCategory) item.Selected = true;
			}

			int pageNumber = page ?? 1;
			var pagedTour = new PagedListTour();
			pagedTour.PageTour = listVTour.ToPagedList(pageNumber, pageSize);
			pagedTour.Categories = category;
			ViewData["IDCategory"] = items;

			//Tuple<List<Category>, List<ViewTour>> tuple = new Tuple<List<Category>, List<ViewTour>>(category, new List<ViewTour>(listVTour));

			return View("Tour", pagedTour);
		}
	}
}