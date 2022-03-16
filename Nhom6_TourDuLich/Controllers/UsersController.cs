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
	public class UsersController : Controller
	{
		CSDL_TOURDB db = new CSDL_TOURDB();
		DataProcessing data = new DataProcessing();
		// đăng ký
		[HttpGet]
		public ActionResult Register()
		{
			return View();
		}

		// đăng nhập
		[HttpGet]
		public ActionResult Login()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Login(LoginModel user)
		{
			if (ModelState.IsValid)
			{
				var dat = data.Login(user.UserName, user.Passwords);
				if (dat != null)
				{
					if (dat.Roles == -1)
					{
						ViewBag.Error = " Tài khoản của bạn đã bị vô hiệu hóa. Vui lòng liên hệ với quản trị viên để biết thêm thông tin!";
						return View();
					}
					#region Add ADMIN_CUSTOMER_SESSION
					var usersession = new UserSession();
					usersession.IDUser = dat.IDUser;
					usersession.FullName = dat.FullName;
					usersession.UserName = dat.UserName;
					usersession.Passwords = dat.Passwords;
					usersession.Email = dat.Email;
					usersession.Roles = dat.Roles;
					usersession.PhoneNumber = dat.PhoneNumber;
					#endregion
					Session.Add(SessionConnection.CUSTOMER_SESSION, usersession);
					if (dat.Roles == 1)
					{
						Session.Add(SessionConnection.ADMIN_SESSION, usersession);
					}
					if (Session["Cart"] != null)
						return RedirectToAction("Order", "Cart");
					return RedirectToAction("Index", "VietTravel");
				}
				else
				{
					var d1 = db.User.SingleOrDefault(s => s.UserName.Equals(user.UserName)) as User;
					var d2 = db.User.SingleOrDefault(s => s.Passwords.Equals(user.Passwords)) as User;

					if (d1 == null && d2 == null)
					{
						ModelState.AddModelError("UserName", "Tên đăng nhập không tồn tại!");
						ModelState.AddModelError("Passwords", "Mật khẩu không tồn tại!");
					}
					else
					{
						if (d1 == null)
							ModelState.AddModelError("UserName", "Tên đăng nhập không tồn tại!");
						else if (d2 == null)
							ModelState.AddModelError("Passwords", "Mật khẩu không tồn tại!");
					}
				}
			}
			return View(user);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Register(RegisterModel user)
		{
			var checkUser = db.User.Where(x => x.StatusDelete == 1);
			if (ModelState.IsValid)
			{
				var RePasswords = Request["RePasswords"];
				if (RePasswords == user.Passwords)
				{
					var CheckEmail = checkUser.SingleOrDefault(x => x.Email == user.Email);
					if (CheckEmail == null)
					{
						var CheckPhone = checkUser.SingleOrDefault(x => x.PhoneNumber == user.PhoneNumber);
						if (CheckPhone == null)
						{
							int checkInsert = data.InsertUser(user);
							if (checkInsert == 1)
								return RedirectToAction("Login");
							else if (checkInsert == 0)
								ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại!");
							else
							{
								ViewBag.ErrorInsert = "Có lỗi xảy ra. Vui lòng thử lại!";
							}
							return View();
						}
						else
						{
							ModelState.AddModelError("PhoneNumber", "Số điện thoại đã tồn tại! Vui lòng nhập lại Số điện thoại khác!");
							return View(user);
						}
					}
					else
					{
						ModelState.AddModelError("Email", "Email đã tồn tại! Vui lòng nhập lại email khác!");
						return View(user);
					}
				}
				else
				{
					ModelState.AddModelError("RePasswords", "Xác nhận mật khẩu thất bại!");
					return View(user);
				}
			}
			return View();
		}
		public ActionResult Logout()
		{
			Session[SessionConnection.CUSTOMER_SESSION] = null;
			Session[SessionConnection.ADMIN_SESSION] = null;
			return RedirectToAction("Login");
		}
	}
}