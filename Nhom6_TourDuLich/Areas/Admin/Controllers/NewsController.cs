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
	public class NewsController : Controller
	{
		private CSDL_TOURDB db = new CSDL_TOURDB();
		DataProcessing data = new DataProcessing();
		// GET: News
		public ActionResult Index(string sortOrder, string sSearch, string cSearch, int? page)
		{
			//các biến sắp xếp
			ViewBag.CurrentSort = sortOrder; //Lấy bến yêu cầu sắp xếp hiện tại
			ViewBag.sxTheoTitle = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";

			//lấy giá trị của bộ lọc hiện tại
			if (sSearch != null)
			{
				page = 1;//trang đầu tiên
			}
			else
			{
				sSearch = cSearch;
			}
			ViewBag.cSearch = sSearch;

			var news = db.News.Select(s => s);

			//Lọc theo tên Tour
			if (!String.IsNullOrEmpty(sSearch))
			{
				news = news.Where(p => p.Title.Contains(sSearch));
				if (news.Count() == 0)
				{
					ViewBag.ErrorFind = "Không tìm thấy. Vui lòng thử lại!";
				}
			}
			//sắp xếp
			switch (sortOrder)
			{
				case "title_desc":
					news = news.OrderByDescending(s => s.Title);
					break;
				default:
					news = news.OrderBy(s => s.Title);
					break;
			}
			int pageSize = 3;
			int pageNumber = (page ?? 1);

			if (TempData["result"] != null)
			{
				if (TempData["result"].ToString().Contains("thành công"))
					ViewBag.Message = TempData["result"];
				else ViewBag.ErrorDelete = TempData["result"];
			}
			return View(news.ToPagedList(pageNumber, pageSize));
		}

		// GET: News/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			News news = db.News.Find(id);
			if (news == null)
			{
				return HttpNotFound();
			}
			return View(news);
		}

		// GET: News/Create
		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "IDNews,Title,Images,Descriptions")] News news)
		{
			ViewBag.Image = "h0.png";
			if (ModelState.IsValid)
			{
				news.Images = "";
				var imgFile = Request.Files["ImageFile"];
				if (imgFile != null && imgFile.ContentLength > 0)
				{
					string FileName = System.IO.Path.GetFileName(imgFile.FileName);
					string UploadPath = Server.MapPath("~/wwwroot/images/imgNew/" + FileName);
					imgFile.SaveAs(UploadPath);
					news.Images = FileName;
				}
				else
				{
					news.Images = "h0.png";
				}
				int checkInsert = data.InsertNews(news);
				if (checkInsert == 1)
				{
					TempData["result"] = "Thêm tin tức thành công";
					return RedirectToAction("Index");
				}
				if (checkInsert == 0)
				{
					ModelState.AddModelError("Title", "Tiêu đề đã tồn tại. Vui lòng nhập lại Tiêu đề khác!");
					return View();
				}
				ViewBag.ErrorInsert = "Có lỗi xảy ra. Vui lòng xem lại!";
			}
			return View();
		}

		// GET: News/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			News news = db.News.Find(id);
			Session["Image"] = news.Images;
			if (news == null)
			{
				return HttpNotFound();
			}
			return View(news);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "IDNews,Title,Images,Descriptions")] News news)
		{
			if (ModelState.IsValid)
			{
				news.Images = Session["Image"].ToString();
				var imgFile = Request.Files["ImageFile"];
				if (imgFile != null && imgFile.ContentLength > 0)
				{
					string FileName = System.IO.Path.GetFileName(imgFile.FileName);
					string UploadPath = Server.MapPath("~/wwwroot/images/imgNew/" + FileName);
					imgFile.SaveAs(UploadPath);
					news.Images = FileName;
				}
				int checkUpdate = data.UpdateNews(news);
				if (checkUpdate == 1)
				{
					TempData["result"] = "Cập nhật tin tức thành công";
					return RedirectToAction("Index");
				}
					
				if (checkUpdate == 0)
				{
					ModelState.AddModelError("Title", "Tiêu đề đã tồn tại! Vui lòng nhập lại Tiêu đề khác!");
					return View();
				}
				ViewBag.ErrorUpdate = "Có lỗi xảy ra. Vui lòng xem lại!";
			}
			return View();
		}

		// GET: News/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			News news = db.News.Find(id);
			if (news == null)
			{
				return HttpNotFound();
			}
			try
			{
				db.News.Remove(news);
				db.SaveChanges();
				TempData["result"] = "Xóa tin tức thành công";
			}
			catch (Exception ex)
			{
				TempData["result"] = "Có lỗi xảy ra: " + ex.Message + " Vui lòng xem lại!";
			}
			return RedirectToAction("Index");

		}
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
