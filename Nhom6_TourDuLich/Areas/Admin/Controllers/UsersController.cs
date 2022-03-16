using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Nhom6_TourDuLich.Models;
using Nhom6_TourDuLich.Sessions;
using Nhom6_TourDuLich.Areas.Admin.Data;

namespace Nhom6_TourDuLich.Areas.Admin.Controllers
{
	public class UsersController : Controller
	{
		private CSDL_TOURDB db = new CSDL_TOURDB();
		DataProcessing data = new DataProcessing();

		// GET: Users
		public ActionResult Index(string sortOrder, string sSearch, string cSearch, string sRole, string cRole, int? page)
		{
			//các biến sắp xếp
			ViewBag.CurrentSort = sortOrder; //Lấy bến yêu cầu sắp xếp hiện tại
			ViewBag.sxTheoTen = String.IsNullOrEmpty(sortOrder) ? "ten_desc" : "";
			ViewBag.sxTheoUserName = sortOrder == "ten_username" ? "ten_username_desc" : "ten_username";
			ViewBag.sxTheoEmail = sortOrder == "email" ? "email_desc" : "email";
			ViewBag.sxTheoRole = sortOrder == "role" ? "role_desc" : "role";

			if (sSearch != null || sRole != null)
			{
				page = 1;//trang đầu tiên
			}
			else
			{
				sSearch = cSearch;
				sRole = cRole;
			}
			ViewBag.cSearch = sSearch;
			ViewBag.cRole = sRole;

			var users = db.User.Where(x => x.StatusDelete == 1);
			//lọc
			if (!String.IsNullOrEmpty(sSearch))
			{
				users = users.Where(p => p.FullName.Contains(sSearch) || p.Email.Contains(sSearch) || p.PhoneNumber.Contains(sSearch));
				if (users.Count() == 0)
					ViewBag.ErrorFind = "Không tìm thấy. Vui lòng thử lại!";
			}
			if (!String.IsNullOrEmpty(sRole))
			{
				users = users.Where(p => p.Roles.ToString().Contains(sRole));
				if (users.Count() == 0)
					ViewBag.ErrorFind = "Không tìm thấy. Vui lòng thử lại!";
			}
			//sắp xếp
			switch (sortOrder)
			{
				case "ten_desc":
					users = users.OrderByDescending(s => s.FullName);
					break;
				case "ten_username":
					users = users.OrderBy(s => s.UserName);
					break;
				case "ten_username_desc":
					users = users.OrderByDescending(s => s.UserName);
					break;
				case "email":
					users = users.OrderBy(s => s.Email);
					break;
				case "email_desc":
					users = users.OrderByDescending(s => s.Email);
					break;
				case "role":
					users = users.OrderBy(s => s.Roles);
					break;
				case "role_desc":
					users = users.OrderByDescending(s => s.Roles);
					break;
				default:
					users = users.OrderBy(s => s.FullName); ;
					break;
			}

			if (TempData["result"] != null)
			{
				if (TempData["result"].ToString().Contains("thành công"))
					ViewBag.Message = TempData["result"];
				else ViewBag.ErrorDelete = TempData["result"];
			}

			int pageSize = 5;
			int pageNumber = (page ?? 1);
			return View(users.ToPagedList(pageNumber, pageSize));
		}

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
					if (dat.Roles == 1)
					{
						#region Add ADMIN_CUSTOM_SESSION
						var adminsession = new UserSession();
						adminsession.IDUser = dat.IDUser;
						adminsession.FullName = dat.FullName;
						adminsession.UserName = dat.UserName;
						adminsession.Passwords = dat.Passwords;
						adminsession.Email = dat.Email;
						adminsession.Roles = dat.Roles;
						adminsession.PhoneNumber = dat.PhoneNumber;
						Session.Add(SessionConnection.ADMIN_SESSION, adminsession);
						Session.Add(SessionConnection.CUSTOMER_SESSION, adminsession);
						#endregion
						return RedirectToAction("Index", "Home");
					}
					else ViewBag.Error = "Có lỗi xảy ra. Vui lòng thử lại!";
				}
				else
				{
					var d1 = db.User.SingleOrDefault(s => s.UserName.Equals(user.UserName)) as Models.User;
					var d2 = db.User.SingleOrDefault(s => s.Passwords.Equals(user.Passwords)) as Models.User;

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

		// GET: Users/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Models.User user = db.User.Find(id);
			if (user == null)
			{
				return HttpNotFound();
			}
			return View(user);
		}

		// GET: Users/Create
		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "IDUser,UserName,FullName,Email,PhoneNumber,Passwords,Roles,StatusDelete")] User user)
		{
			if (ModelState.IsValid)
			{
				string checkInsert = data.InsertUser(user);
				if (checkInsert.Contains("thành công!"))
				{
					TempData["result"] = checkInsert;
					return RedirectToAction("Index");
				}
				if (checkInsert.Contains("Tên đăng nhập"))
				{
					ModelState.AddModelError("UserName", checkInsert);
					return View();
				}
				if (checkInsert.Contains("Email"))
				{
					ModelState.AddModelError("Email", checkInsert);
					return View();
				}
				if (checkInsert.Contains("Số điện thoại"))
				{
					ModelState.AddModelError("PhoneNumber", checkInsert);
					return View();
				}
				ViewBag.ErrorInsert = checkInsert;
			}
			return View(user);
		}

		// GET: Users/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Models.User user = db.User.Find(id);

			if (user == null)
			{
				return HttpNotFound();
			}
			return View(user);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(User user)
		{
			if (ModelState.IsValid)
			{
				string checkUpdate = data.UpdateUser(user);
				if (checkUpdate.Contains("thành công"))
				{
					TempData["result"] = checkUpdate;
					return RedirectToAction("Index");
				}
				if (checkUpdate.Contains("Tên đăng nhập"))
				{
					ModelState.AddModelError("UserName", checkUpdate);
					return View();
				}
				if (checkUpdate.Contains("Email"))
				{
					ModelState.AddModelError("Email", checkUpdate);
					return View();
				}
				if (checkUpdate.Contains("Số điện thoại"))
				{
					ModelState.AddModelError("PhoneNumber", checkUpdate);
					return View();
				}
				ViewBag.ErrorInsert = checkUpdate;
			}
			return View();
		}

		// GET: Users/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Models.User user = db.User.Find(id);
			if (user == null)
			{
				return HttpNotFound();
			}
			string checkDelete = data.DeleteUser(user.IDUser);
			if (checkDelete.Contains("admin"))
			{
				var user1 = db.User.Find(id);
				return View("Edit", user);
			}
			TempData["result"] = checkDelete;
			return RedirectToAction("Index");
		}
/*
		// POST: Users/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			string checkDelete = data.DeleteUser(id);
			if (checkDelete.Contains("admin"))
			{
				var user = db.User.Find(id);
				return View("Edit", user);
			}
			if (checkDelete.Contains("Thành công"))
				return RedirectToAction("Index");
			ViewBag.ErrorDelete = checkDelete;
			return View();
		}*/
		public ActionResult Logout()
		{
			Session[SessionConnection.CUSTOMER_SESSION] = null;
			Session[SessionConnection.ADMIN_SESSION] = null;
			return RedirectToAction("Login");
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
