using Nhom6_TourDuLich.Models;
using Nhom6_TourDuLich.Areas.Admin.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nhom6_TourDuLich.Areas.Admin.Data
{
	public class DataProcessing
	{
		private CSDL_TOURDB db = new CSDL_TOURDB();

		#region Đăng nhập
		public User Login(string username, string pass)
		{
			var user = db.User.SingleOrDefault(x => x.UserName == username && x.Passwords == pass && x.StatusDelete == 1);
			return user;
		}
		#endregion
		#region Quản lý Trang chủ
		public List<Tour> GetListTourMaxByBooked(int count)
		{
			return db.Tours.Where(x => x.StatusDelete == 1).OrderByDescending(a => a.NumberBooked).Take(count).ToList();
		}
		#endregion
		#region Quản lý Danh mục tour
		public List<Category> ViewCategory()
		{
			var listCategory = db.Categories.Where(x => x.StatusDelete == 1).ToList();
			foreach (var item in listCategory)
			{
				var result = listCategory.Where(x => x.IDCategory == item.IDCategory && x.StatusDelete == 1).Join(db.Tours, x => x.IDCategory, y => y.IDCategory, (x, y) => new { y.IDTour, y.StatusDelete }).Where(x => x.StatusDelete == 1).ToList();
				item.Quantity = result.Count();

				// Cập nhật số lượng trong Category
				var cate = db.Categories.SingleOrDefault(x => x.IDCategory == item.IDCategory && x.StatusDelete == 1);
				cate.Quantity = item.Quantity;
				db.SaveChanges();
			}
			var check = db.Categories.Join(db.Tours, x => x.IDCategory, y => y.IDCategory, (x, y) => new
			{
				x.IDCategory,
				x.CategoryName,
				y.IDTour
			}).GroupBy(x => new { x.IDCategory, x.CategoryName}).Select(x=>new { 
				IDCategory = x.Key.IDCategory, CategoryName = x.Key.CategoryName, Quantity = x.Count()
			});

			//var quantities = db.
			return listCategory;
		}
		public int InsertCategory(Category cate)
		{
			try
			{
				Category category = db.Categories.SingleOrDefault(x => x.CategoryName == cate.CategoryName);
				if (category == null)
				{
					cate.StatusDelete = 1;
					cate.Quantity = 0;
					db.Categories.Add(cate);
					db.SaveChanges();
					return 1;
				}
				return 0;
			}
			catch (Exception)
			{
				return -1;
			}
		}
		public int UpdateCategory(Category cate)
		{
			try
			{
				Category category = db.Categories.SingleOrDefault(x => x.IDCategory == cate.IDCategory);
				if (category != null)
				{
					category.CategoryName = cate.CategoryName;
					category.Images = cate.Images;
					db.SaveChanges();
					return 1;
				}
				return -1;
			}
			catch (Exception)
			{
				return -1;
			}
		}

		#endregion
		#region Quản lý Tour
		public int InsertTour(Tour tour)
		{
			try
			{
				Tour tours = db.Tours.SingleOrDefault(x => x.TourName == tour.TourName);
				if (tours == null)
				{
					tour.StatusDelete = 1;
					tour.NumberBooked = 0;
					db.Tours.Add(tour);

					var category = db.Categories.SingleOrDefault(x => x.IDCategory == tour.IDCategory);
					//var category = db.Categories.Where(x => db.Tours.Any(y => y.IDCategory == x.IDCategory)).SingleOrDefault();
					category.Quantity++;
					db.SaveChanges();
					return 1;
				}
				return 0;
			}
			catch (Exception)
			{
				return -1;
			}
		}
		public int UpdateTour(Tour tour)
		{
			try
			{
				Tour tours = db.Tours.SingleOrDefault(x => x.IDTour == tour.IDTour);
				if (tours != null)
				{
					tours.TourName = tour.TourName;
					tours.Addresss = tour.Addresss;
					tours.Cost = tour.Cost;
					tours.Images = tour.Images;
					tours.Statuss = tour.Statuss;
					tours.ShortDescription = tour.ShortDescription;
					tours.DetailDescription = tour.DetailDescription;
					tours.IDCategory = tour.IDCategory;
					tours.NumberDateTour = tour.NumberDateTour;

					var idCategory = db.Tours.SingleOrDefault(x => x.IDTour == tours.IDTour).IDCategory;
					var categoryOld = db.Categories.SingleOrDefault(x => x.IDCategory == idCategory);
					if (categoryOld != null)
					{
						categoryOld.Quantity--;
						if (categoryOld.Quantity < 0)
							categoryOld.Quantity = 0;
					}
					var categoryNew = db.Categories.SingleOrDefault(x => x.IDCategory == tours.IDCategory);
					if (categoryNew != null)
					{
						categoryNew.Quantity++;
					}
					//var category = db.Categories.Where(x => db.Tours.Any(y => y.IDCategory == x.IDCategory)).SingleOrDefault();
					db.SaveChanges();
					return 1;
				}
				return -1;
			}
			catch (Exception)
			{
				return -1;
			}
		}
		public int DeleteTour(int id)
		{
			try
			{
				var tour = db.Tours.Find(id);
				tour.StatusDelete = 0;
				var category = db.Categories.SingleOrDefault(x => x.IDCategory == tour.IDCategory);
				category.Quantity--;
				if (category.Quantity < 0)
				{
					category.Quantity = 0;
				}
				db.SaveChanges();
				return 1;
			}
			catch
			{
				return -1;
			}
		}

		#endregion
		#region Quản lý tài khoản
		public string InsertUser(User user)
		{
			try
			{
				var checkUsername = db.User.SingleOrDefault(x => x.UserName == user.UserName && x.StatusDelete == 1);
				if (checkUsername != null)
					return "Tên đăng nhập đã tồn tại! Vui lòng nhập lại Tên đăng nhập khác!";
				var checkEmail = db.User.SingleOrDefault(x => x.Email == user.Email && x.StatusDelete == 1);
				if (checkEmail != null)
					return "Email đã tồn tại. Vui lòng nhập lại Email khác";
				var checkPhone = db.User.SingleOrDefault(x => x.PhoneNumber == user.PhoneNumber && x.StatusDelete == 1);
				if (checkPhone != null)
					return "Số điện thoại đã tồn tại. Vui lòng nhập lại Số điện thoại khác!";

				user.StatusDelete = 1;
				db.User.Add(user);
				db.SaveChanges();
				return "Thêm mới Tài khoản thành công!";
			}
			catch (Exception ex)
			{
				return "Có lỗi xảy ra: " + ex.Message + " Vui lòng xem lại!";
			}
		}
		public string UpdateUser(User user)
		{
			try
			{
				var checkUser = db.User.Find(user.IDUser);
				var checkUsername = db.User.SingleOrDefault(x => x.UserName == user.UserName && x.UserName != checkUser.UserName);
				if (checkUsername != null)
					return "Tên đăng nhập đã tồn tại. Vui lòng nhập lại Tên đăng nhập khác!";

				var checkEmail = db.User.SingleOrDefault(x => x.Email == user.Email && x.Email != checkUser.Email);
				if (checkEmail != null)
					return "Email đã tồn tại. Vui lòng nhập lại Email khác!";

				var checkPhone = db.User.SingleOrDefault(x => x.PhoneNumber == user.PhoneNumber && x.PhoneNumber != checkUser.PhoneNumber);
				if (checkPhone != null)
					return "Số điện thoại đã tồn tại. Vui lòng nhập lại Số điện thoại khác!";

				checkUser.UserName = user.UserName;
				checkUser.FullName = user.FullName;
				checkUser.Email = user.Email;
				checkUser.PhoneNumber = user.PhoneNumber;
				checkUser.Passwords = user.Passwords;
				checkUser.Roles = user.Roles;
				checkUser.StatusDelete = 1;
				db.SaveChanges();
				return "Cập nhập Tài khoản thành công!";
			}
			catch (Exception ex)
			{
				return "Có lỗi xảy ra: " + ex.Message + " Vui lòng xem lại!";
			}
		}
		public string DeleteUser(int id)
		{
			try
			{
				var user = db.User.Find(id);
				/*if (user.Roles == 1)
				{
					return "Đây là tài khoản của một admin. Không thể xóa. Vui lòng chỉnh sửa lại quyền trước khi xóa!";
				}*/
				user.StatusDelete = 0;
				db.SaveChanges();
				return "Xóa Tài khoản thành công!";
			}
			catch (Exception ex)
			{
				return "Có lỗi xảy ra: " + ex.Message + " Vui lòng xem lại!";
			}
		}

		#endregion

		#region Quản lý Đơn hàng
		public ViewModel GetListData(Order order)
		{
			ViewModel getData = new ViewModel();
			List<Tour> lstTour = new List<Tour>();
			var user = db.User.SingleOrDefault(x => x.IDUser == order.IDUser && x.StatusDelete == 1);
			getData.order = order;
			getData.user = user;
			return getData;
		}
		public string DeleteOrder(int id)
		{
			try
			{
				var order = db.Orders.Find(id);
				order.StatusDelete = 0;
				var bills = db.Bills.Where(x => x.IDOrder == order.IDOrder).ToList();
				foreach (var item in bills)
				{
					item.StatusDelete = 0;
				}
				db.SaveChanges();
				return "Xóa order thành công!";
			}
			catch (Exception ex)
			{
				return "Có lỗi xảy ra: " + ex.Message + " Vui lòng xem lại!";
			}
		}
		/*public List<Order> FindOrders(string findText)
		{
			var finds = from user in db.User
					join order in db.Orders on user.IDUser equals order.IDUser
					join bill in db.Bills on order.IDOrder equals bill.IDOrder
					join tour in db.Tours on bill.IDTour equals tour.IDTour
					where (user.FullName.Contains(findText) || order.IDOrder.ToString() == findText || user.PhoneNumber.Contains(findText)) && (user.StatusDelete == 1 && order.StatusDelete == 1 && bill.StatusDelete == 1 && tour.StatusDelete == 1)
					select new Order
					{
						IDOrder = order.IDOrder,
						IDUser = order.IDUser,
						BookingDate = order.BookingDate,
						StatusDelete = order.StatusDelete
					};
			if (finds.Count() > 0)
				return new List<Order>(finds);
			return null;
		}*/
		#endregion
		#region Quản lý Tin tức
		public int InsertNews(News news)
		{
			try
			{
				var newss = db.News.SingleOrDefault(x => x.Title == news.Title);
				if (newss != null)
					return 0;
				db.News.Add(news);
				db.SaveChanges();
				return 1;
			}
			catch
			{
				return -1;
			}
		}
		public int UpdateNews(News newsOld)
		{
			try
			{
				var newsNew = db.News.Find(newsOld.IDNews);
				var checkNew = db.News.SingleOrDefault(x => x.Title == newsOld.Title && x.Title != newsNew.Title);
				if (checkNew != null)
					return 0;
				newsNew.Title = newsOld.Title;
				newsNew.Images = newsOld.Images;
				newsNew.Descriptions = newsOld.Descriptions;
				db.SaveChanges();
				return 1;
			}
			catch (Exception)
			{
				return -1;
			}
		}
		#endregion
	}
}