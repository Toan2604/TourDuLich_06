using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Nhom6_TourDuLich.Areas.Admin.Data;
using Nhom6_TourDuLich.Models;
using PagedList;

namespace Nhom6_TourDuLich.Areas.Admin.Controllers
{
	public class ToursController : Controller
	{
		private CSDL_TOURDB db = new CSDL_TOURDB();
		DataProcessing data = new DataProcessing();

		// GET: Tours
		private List<Tour> LayTour(int count)
		{
			//sx giảm dần theo lượt đặt tour
			return db.Tours.OrderByDescending(a => a.NumberBooked).Take(count).ToList(); ;
		}
		public ActionResult Index(string sortOrder, string sNameTour, string sStartCost,
		    string sEndCost, string sNameCate, string cStatus, string cNameCate, string sStatus,
		    string cNameTour, string cStartCost, string cEndCost, int? page)
		{
			//các biến sắp xếp
			ViewBag.CurrentSort = sortOrder; //Lấy bến yêu cầu sắp xếp hiện tại
			ViewBag.sxTheoTenTour = String.IsNullOrEmpty(sortOrder) ? "tentour_desc" : "";
			ViewBag.sxTheoTenCate = sortOrder == "tencate" ? "tencate_desc" : "tencate";
			ViewBag.sxTheoGia = sortOrder == "gia" ? "gia_desc" : "gia";
			ViewBag.sxTheoTinhTrang = sortOrder == "tinhtrang" ? "tinhtrang_desc" : "tinhtrang";
			//lấy giá trị của bộ lọc hiện tại
			if (sNameCate != null || sNameTour != null || sStartCost != null || sEndCost != null || sStatus != null)
			{
				page = 1;//trang đầu tiên
			}
			else
			{
				sNameCate = cNameCate;
				sNameTour = cNameTour;
				sStartCost = cStartCost;
				sEndCost = cEndCost;
				sStatus = cStatus;
			}
			ViewBag.cNameCate = sNameCate;
			ViewBag.cNameTour = sNameTour;
			ViewBag.cStartCost = sStartCost;
			ViewBag.cEndCost = sEndCost;
			ViewBag.cStatus = sStatus;

			var tours = db.Tours.Where(t => t.StatusDelete == 1);

			var model = db.Categories.Where(t => t.StatusDelete == 1);
			ViewBag.model = model;
			//ViewData["IDCategory"] = new SelectList(db.Categories.Where(x => x.StatusDelete == 1), "IDCategory", "CategoryName");

			bool flat = false;
			if (!String.IsNullOrEmpty(sNameCate))
			{
				tours = tours.Where(p => p.Category.CategoryName.Contains(sNameCate));
				flat = true;
			}
			else flat = true;

			//Lọc theo tên Tour
			if (!String.IsNullOrEmpty(sNameTour))
			{
				tours = tours.Where(p => p.TourName.Contains(sNameTour));
				if (tours.Count() > 0)
					flat = true;
				else flat = false;
			}
			if (!String.IsNullOrEmpty(sStartCost))
			{
				int min = int.Parse(sStartCost);
				tours = tours.Where(p => p.Cost >= min);
				if (tours.Count() > 0)
					flat = true;
				else flat = false;
			}
			if (!String.IsNullOrEmpty(sEndCost))
			{
				int max = int.Parse(sEndCost);
				tours = tours.Where(p => p.Cost <= max);
				if (tours.Count() > 0)
					flat = true;
				else flat = false;
			}
			if (!String.IsNullOrEmpty(sStatus))
			{
				tours = tours.Where(p => p.Statuss.Contains(sStatus));
				if (tours.Count() > 0)
					flat = true;
				else flat = false;
			}
			if (!flat)
				ViewBag.ErrorFind = "Không tìm thấy. Vui lòng thử lại!";
			//sắp xếp
			switch (sortOrder)
			{
				case "tentour_desc":
					tours = tours.OrderByDescending(s => s.TourName);
					break;
				case "tencate":
					tours = tours.OrderBy(s => s.Category.CategoryName);
					break;
				case "tencate_desc":
					tours = tours.OrderByDescending(s => s.Category.CategoryName);
					break;
				case "gia":
					tours = tours.OrderBy(s => s.Cost);
					break;
				case "gia_desc":
					tours = tours.OrderByDescending(s => s.Cost);
					break;
				case "tinhtrang":
					tours = tours.OrderBy(s => s.Statuss);
					break;
				case "tinhtrang_desc":
					tours = tours.OrderByDescending(s => s.Statuss);
					break;
				default:
					tours = tours.OrderBy(s => s.TourName);
					break;
			}

			if (TempData["result"] != null)
			{
				if (TempData["result"].ToString().Contains("thành công"))
					ViewBag.Message = TempData["result"];
				else ViewBag.ErrorDelete = TempData["result"];
			}

			int pageSize = 10;
			int pageNumber = (page ?? 1);
			return View(tours.ToPagedList(pageNumber, pageSize));
		}

		// GET: Tours/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Tour tour = db.Tours.Find(id);
			if (tour == null)
			{
				return HttpNotFound();
			}
			return View(tour);
		}

		// GET: Tours/Create
		public ActionResult Create()
		{
			ViewBag.IDCategory = new SelectList(db.Categories.Where(x => x.StatusDelete == 1), "IDCategory", "CategoryName");
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "IDTour,TourName,Addresss,Cost,Images,Statuss,ShortDescription,DetailDescription,IDCategory,NumberDateTour,NumberBooked,StatusDelete")] Tour tour)
		{
			ViewBag.Image = "h0.png";
			//ViewData["IDCategory"] = new SelectList(db.Categories.Where(x => x.StatusDelete == 1), "IDCategory", "CategoryName");

			if (ModelState.IsValid)
			{
				tour.Images = "";
				var imgFile = Request.Files["ImageFile"];
				if (imgFile != null && imgFile.ContentLength > 0)
				{
					string FileName = System.IO.Path.GetFileName(imgFile.FileName);
					string UploadPath = Server.MapPath("~/wwwroot/images/imgTour/" + FileName);
					imgFile.SaveAs(UploadPath);
					ViewBag.Image = FileName;
					tour.Images = FileName;
				}
				else
				{
					tour.Images = "h0.png";
				}
				int checkAddTour = data.InsertTour(tour);
				if (checkAddTour == 1)
				{
					TempData["result"] = "Thêm Tour thành công!";
					return RedirectToAction("Index");
				}
				else if (checkAddTour == 0)
					ModelState.AddModelError("TourName", "Tên tour đã tồn tại!");
				else
					ViewBag.Error = "Có lỗi xảy ra. Vui lòng thử lại!";
				db.SaveChanges();
			}
			return View(tour);
		}

		// GET: Tours/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Tour tour = db.Tours.Find(id);
			Session["ImageTour"] = tour.Images;
			if (tour == null)
			{
				return HttpNotFound();
			}
			ViewBag.IDCategory = new SelectList(db.Categories, "IDCategory", "CategoryName", tour.IDCategory);
			return View(tour);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "IDTour,TourName,Addresss,Cost,Images,Statuss,ShortDescription,DetailDescription,IDCategory,NumberDateTour,NumberBooked,StatusDelete")] Tour tour)
		{
			ViewBag.IDCategory = new SelectList(db.Categories, "IDCategory", "CategoryName", tour.IDCategory);

			if (ModelState.IsValid)
			{
				tour.Images = Session["ImageTour"].ToString();
				var imgFile = Request.Files["ImageFile"];
				if (imgFile != null && imgFile.ContentLength > 0)
				{
					string FileName = System.IO.Path.GetFileName(imgFile.FileName);
					string UploadPath = Server.MapPath("~/wwwroot/images/imgTour/" + FileName);
					imgFile.SaveAs(UploadPath);
					ViewBag.Image = FileName;
					tour.Images = FileName;
				}
				int checkUpdateTour = data.UpdateTour(tour);
				if (checkUpdateTour == 1)
				{
					TempData["result"] = "Cập nhập Tour thành công!";
					return RedirectToAction("Index");
				}
				else
					ViewBag.Error = "Có lỗi xảy ra. Vui lòng thử lại!";
			}
			return View();
		}

		// GET: Tours/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Tour tour = db.Tours.Find(id);
			if (tour == null)
			{
				return HttpNotFound();
			}
			int checkDelete = data.DeleteTour(tour.IDTour);
			if (checkDelete == 1)
			{
				TempData["result"] = "Xóa Tour thành công!";
			}
			else TempData["result"] = "Có lỗi xảy ra! Vui lòng kiểm tra lại!";
			return RedirectToAction("Index");
		}
/*
		// POST: Tours/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Tour tour = db.Tours.Find(id);
			int checkDelete = data.DeleteTour(tour.IDTour);
			if (checkDelete == 1)
				return RedirectToAction("Index");

			ViewBag.Error = "Có lỗi xảy ra! Vui lòng kiểm tra lại!";
			return View();

		}
*/
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
