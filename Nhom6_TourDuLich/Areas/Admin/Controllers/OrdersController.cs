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
	public class OrdersController : Controller
	{
		private CSDL_TOURDB db = new CSDL_TOURDB();
		DataProcessing data = new DataProcessing();
		// GET: Orders
		public ActionResult Index(string sortOrder, string sTen, string cTen, int? page)
		{
			//các biến sắp xếp
			ViewBag.CurrentSort = sortOrder; //Lấy bến yêu cầu sắp xếp hiện tại
			ViewBag.sxTheoIDOrder = String.IsNullOrEmpty(sortOrder) ? "id" : "";
			//ViewBag.sxTheoTen = sortOrder == "ten" ? "ten_desc" : "ten";
			ViewBag.sxTheoNgay = sortOrder == "ngay" ? "ngay_desc" : "ngay";

			var orders = db.Orders.Where(o => o.StatusDelete==1);

			//lấy giá trị của bộ lọc hiện tại
			if (sTen != null)
			{
				page = 1;//trang đầu tiên
			}
			else
			{
				sTen = cTen;
			}
			ViewBag.cTen = sTen;

			if (!String.IsNullOrEmpty(sTen))
			{
				orders = orders.Where(p => p.User.FullName.Contains(sTen) || p.User.PhoneNumber.Contains(sTen) || p.IDOrder.ToString()==sTen.ToString());
				if (orders.Count() == 0)
				{
					ViewBag.ErrorFind = "Không tìm thấy. Vui lòng thử lại!";
				}
			}

			//sắp xếp
			switch (sortOrder)
			{
				case "id":
					orders = orders.OrderByDescending(s => s.IDOrder);
					break;
				case "ngay":
					orders = orders.OrderBy(s => s.BookingDate);
					break;
				case "ngay_desc":
					orders = orders.OrderByDescending(s => s.BookingDate);
					break;
				default:
					orders = orders.OrderBy(s => s.IDOrder);
					break;
			}
			int pageSize = 5;
			int pageNumber = (page ?? 1);

			if (TempData["result"] != null)
			{
				if (TempData["result"].ToString().Contains("thành công"))
					ViewBag.Message = TempData["result"];
				else ViewBag.Error = TempData["result"];
			}
			return View(orders.ToPagedList(pageNumber, pageSize));
		}

		// GET: Orders/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Order order = db.Orders.Find(id);
			if (order == null)
			{
				return HttpNotFound();
			}
			var getData = data.GetListData(order);
			var model = db.Bills.Where(x=>x.IDOrder == order.IDOrder && x.StatusDelete ==1).Join(db.Tours, x => x.IDTour, y => y.IDTour, (x, y) => new ViewModel
			{
				bill = x,
				tour = y,
				Thanhtien = y.Cost * x.Quantity
			});
			ViewBag.DataBill = model;
			return View(getData);
		}

		// GET: Orders/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Order order = db.Orders.Find(id);
			if (order == null)
			{
				return HttpNotFound();
			}
			var checkDelete = data.DeleteOrder(order.IDOrder);
			TempData["result"] = checkDelete;
			return RedirectToAction("Index");
		}

		// POST: Orders/Delete/5
		/*[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			return RedirectToAction("Index");
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
