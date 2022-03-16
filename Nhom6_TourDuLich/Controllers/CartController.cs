using Nhom6_TourDuLich.Models;
using Nhom6_TourDuLich.Models.Custom;
using Nhom6_TourDuLich.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nhom6_TourDuLich.Controllers
{
	public class CartController : Controller
	{
		CSDL_TOURDB db = new CSDL_TOURDB();
		//Lấy giỏ hàng
		public List<Cart> GetListCart()
		{
			List<Cart> lstCart = Session["Cart"] as List<Cart>;
			if (lstCart == null)
			{
				//Nếu giỏ hàng chưa tồn tại thì khởi tạo lstGioHang
				lstCart = new List<Cart>();
				Session["Cart"] = lstCart;
			}
			return lstCart;
		}

		//Thêm hàng vào giỏ
		public ActionResult AddCart(int iIDTour)
		{
			//Lấy ra Session GioHang
			List<Cart> lstCart = GetListCart();
			//kiểm tra tour tồn tại trong Session GioHang chưa
			Cart sanpham = lstCart.Find(t => t.iIDTour == iIDTour);
			if (sanpham == null)
			{
				sanpham = new Cart(iIDTour);
				lstCart.Add(sanpham);
				return RedirectToAction("Cart", "Cart");
			}
			else
			{
				sanpham.iQuantity++;
				return RedirectToAction("Cart", "Cart");
			}
		}

		//Tính tổng số lượng
		private int TotalQuantity()
		{
			int iTotalQuantity = 0;
			List<Cart> lstCart = Session["Cart"] as List<Cart>;
			if (lstCart != null)
			{
				iTotalQuantity = lstCart.Sum(t => t.iQuantity);
			}
			return iTotalQuantity;
		}

		//Tính tổng tiền
		private int TotalCost()
		{
			int iTotalCost = 0;
			List<Cart> lstCart = Session["Cart"] as List<Cart>;
			if (lstCart != null)
			{
				iTotalCost = lstCart.Sum(t => t.iTotal);
			}
			return iTotalCost;
		}

		//Tạo trang giỏ hàng
		public ActionResult Cart()
		{
			List<Cart> lstCart = GetListCart();
			if (lstCart.Count == 0)
			{
				return RedirectToAction("Index", "VietTravel");
			}
			ViewBag.TotalQuantity = TotalQuantity();
			ViewBag.TotalCost = TotalCost();
			return View(lstCart);
		}

		//Xóa giỏ hàng
		public ActionResult DeleteCart(int iMaSP)
		{
			//Lấy giỏ hàng từ Session
			List<Cart> lstCart = GetListCart();
			Cart tour = lstCart.SingleOrDefault(t => t.iIDTour == iMaSP);
			if (tour != null)
			{
				lstCart.RemoveAll(t => t.iIDTour == iMaSP);
				return RedirectToAction("Cart");
			}
			if (lstCart.Count == 0)
			{
				return RedirectToAction("Index", "VietTravel");
			}
			return RedirectToAction("Cart");
		}

		//Cập nhật giỏ hàng
		public ActionResult UpdateCart(int iMaSP, FormCollection data)
		{
			List<Cart> lstCart = GetListCart();
			Cart tour = lstCart.SingleOrDefault(t => t.iIDTour == iMaSP);
			//Nếu tồn tại thì cho sửa số lượng
			if (tour != null)
			{
				tour.iQuantity = int.Parse(data["txtQuantity"].ToString());
			}
			return RedirectToAction("Cart");
		}

		//Xóa tất cả hàng trong giỏ
		public ActionResult DeleteAllCart()
		{
			List<Cart> lstCart = GetListCart();
			lstCart.Clear();
			return RedirectToAction("Index", "VietTravel");
		}

		//Hiển thị view Dathang để cập nhật thông tin cho đơn hàng
		[HttpGet]
		public ActionResult Order()
		{
			var user = (UserSession)Session[SessionConnection.CUSTOMER_SESSION];
			//Kiểm tra đăng nhập
			if (user == null)
			{
				return RedirectToAction("Login", "Users");
			}

			//Kiểm tra giỏ hàng trống
			if (Session["Cart"] == null)
			{
				return RedirectToAction("Index", "VietTravel");
			}
			//Lấy giỏ hàng từ Session
			List<Cart> lstCart = GetListCart();
			ViewBag.TotalQuantity = TotalQuantity();
			ViewBag.TotalCost = TotalCost();

			return View(lstCart);
			//return View(lstCart);
		}
		//[HttpPost]
		public ActionResult Order(FormCollection data)
		{
			//Lấy giỏ hàng từ Session
			List<Cart> lstCart = GetListCart();
			//Thêm order
			Order order = new Order();
			var  user = (UserSession)Session[SessionConnection.CUSTOMER_SESSION];
			order.IDUser = user.IDUser;
			order.BookingDate = DateTime.Now;
			order.StatusDelete = 1;
			db.Orders.Add(order);
			db.SaveChanges();
			//Thêm bill
			foreach (var item in lstCart)
			{
				Bill bill = new Bill();
				bill.IDOrder = order.IDOrder;
				bill.IDTour = item.iIDTour;
				var tour = db.Tours.Find(item.iIDTour);
				tour.NumberBooked++;
				bill.Quantity = item.iQuantity;
				bill.PaymentMethod = data["sPayMethod"];
				bill.Note = data["Note"];
				bill.StatusDelete = 1;
				db.Bills.Add(bill);
			}
			db.SaveChanges();
			Session["Cart"] = null;
			return RedirectToAction("AcceptOrder", "Cart");
		}
		public ActionResult AcceptOrder()
		{
			return View();
		}
	}
}