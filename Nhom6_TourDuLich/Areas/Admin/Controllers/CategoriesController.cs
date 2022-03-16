using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Nhom6_TourDuLich.Areas.Admin.Data;
using Nhom6_TourDuLich.Models;
using PagedList;

namespace Nhom6_TourDuLich.Areas.Admin.Controllers
{
	public class CategoriesController : Controller
	{
		private CSDL_TOURDB db = new CSDL_TOURDB();
		DataProcessing data = new DataProcessing();
		// GET: Categories
		public ActionResult Index(string sortOrder, string searchName, string currentName, int? page)
		{
			//các biến sắp xếp
			ViewBag.CurrentSort = sortOrder; //Lấy bến yêu cầu sắp xếp hiện tại
			ViewBag.sxTheoTen = String.IsNullOrEmpty(sortOrder) ? "ten_desc" : "";
			ViewBag.sxTheoSL = sortOrder == "sl" ? "sl_desc" : "sl";
			//Lấy giá tị của bộ lọc hiện tại
			if (searchName != null)
			{
				page = 1;//trang đầu tiên
			}
			else
			{
				searchName = currentName;
			}
			ViewBag.currentName = searchName;
			var category = data.ViewCategory();
			
			if (!String.IsNullOrEmpty(searchName))
			{
				category = new List<Category>(category.Where(p => p.CategoryName.ToLower().Contains(searchName.ToLower())));
				if (category.Count == 0)
				{
					ViewBag.ErrorFind = "Không tìm thấy. Vui lòng xem lại!";
				}
			}
			//sắp xếp
			switch (sortOrder)
			{
				case "ten_desc":
					category = new List<Category>(category.OrderByDescending(s => s.CategoryName));
					break;
				default:
					category = new List<Category>(category.OrderBy(s => s.CategoryName));
					break;
			}

			if (TempData["result"] != null)
			{
				if (TempData["result"].ToString().Contains("thành công"))
					ViewBag.Message = TempData["result"];
				else ViewBag.ErrorDelete = TempData["result"];
			}

			int pageSize = 5;//Kích thước trang
			int pageaNumber = (page ?? 1);//nếu page bằng null thì trả về 1
			return View(category.ToPagedList(pageaNumber, pageSize));
		}

		// GET: Categories/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Category category = db.Categories.Find(id);

			var result = from c in db.Categories
					 join t in db.Tours on c.IDCategory equals t.IDCategory
					 where c.IDCategory == id
					 //int.Parse(IDCategory)
					 select new
					 {
						 t.IDTour
					 };
			category.Quantity = result.Count();

			if (category == null)
			{
				return HttpNotFound();
			}
			return View(category);
		}

		// GET: Categories/Create
		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "IDCategory,CategoryName,Quantity,Image")] Category cate)
		{
			ViewBag.Image = "h0.png";
			if (ModelState.IsValid)
			{
				cate.Images = "";
				var imgFile = Request.Files["ImageFile"];
				if (imgFile != null && imgFile.ContentLength > 0)
				{
					string FileName = System.IO.Path.GetFileName(imgFile.FileName);
					string UploadPath = Server.MapPath("~/wwwroot/images/imgCategory/" + FileName);
					imgFile.SaveAs(UploadPath);
					ViewBag.Image = FileName;
					cate.Images = FileName;
				}
				else
				{
					cate.Images = "h0.png";
				}
				int checkAddCate = data.InsertCategory(cate);
				if (checkAddCate == 1)
				{
					TempData["result"] = "Thêm mới Danh mục thành công!";
					return RedirectToAction("Index");
				}
				else if (checkAddCate == 0)
					ModelState.AddModelError("CategoryName", "Tên danh mục đã tồn tại!");
				else
					ViewBag.Error = "Có lỗi xảy ra. Vui lòng thử lại!";
			}
			return View();
		}

		// GET: Categories/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Category category = db.Categories.Find(id);
			Session["ImageCategory"] = category.Images;
			if (category == null)
			{
				return HttpNotFound();
			}
			return View(category);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "IDCategory,CategoryName,Quantity,Image")] Category category)
		{
			if (ModelState.IsValid)
			{
				category.Images = Session["ImageCategory"].ToString();
				var imgFile = Request.Files["ImageFile"];
				if (imgFile != null && imgFile.ContentLength > 0)
				{
					string FileName = System.IO.Path.GetFileName(imgFile.FileName);
					string UploadPath = Server.MapPath("~/wwwroot/images/imgCategory/" + FileName);
					imgFile.SaveAs(UploadPath);
					category.Images = FileName;
				}
				int checkUpdate = data.UpdateCategory(category);
				if (checkUpdate == 1)
				{
					TempData["result"] = "Cập nhật Danh mục thành công!";
					return RedirectToAction("Index");
				}
				else
					ViewBag.Error = "Có lỗi xảy ra. Vui lòng thử lại!";
			}
			return View();
		}

		// GET: Categories/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Category category = db.Categories.Find(id);
			if (category == null)
			{
				return HttpNotFound();
			}
			category.StatusDelete = 0;
			db.SaveChanges();
			TempData["result"] = "Xóa Danh mục thành công!";
			return RedirectToAction("Index");
		}
/*
		// POST: Categories/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Category category = db.Categories.Find(id);
			try
			{
				category.StatusDelete = 0;
				//db.Categories.Remove(category);
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			catch (Exception ex)
			{
				ViewBag.Error = "Lỗi nhập dữ liệu! " + ex.Message;
				return View("Delete", category);
			}
		}*/

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
