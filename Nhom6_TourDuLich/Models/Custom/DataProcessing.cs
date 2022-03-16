using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nhom6_TourDuLich.Models.Custom
{
	public class DataProcessing
	{
		CSDL_TOURDB db = new CSDL_TOURDB();
		public int InsertUser(RegisterModel user)
		{
			try
			{
				User checkUser = new User();
				checkUser.FullName = user.FullName;
				checkUser.UserName = user.UserName;
				checkUser.Email = user.Email;
				checkUser.Passwords = user.Passwords;
				checkUser.PhoneNumber = user.PhoneNumber;

				var user1 = db.User.SingleOrDefault(x => x.UserName == user.UserName && x.StatusDelete==1);
				if (user1 == null)
				{
					checkUser.Roles = 0;
					checkUser.StatusDelete = 1;
					db.User.Add(checkUser);
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
		public User Login(string username, string pass)
		{
			var user = db.User.SingleOrDefault(x => x.UserName == username && x.Passwords == pass && x.StatusDelete==1);
			return user;
		}
		public List<ViewTour> GetListViewTour()
		{
			var lstTour = db.Tours.Join(db.Categories, x => x.IDCategory, y => y.IDCategory, (x, y) => new
			{
				x.IDTour,
				x.TourName,
				x.Images,
				x.Cost,
				x.Statuss,
				x.ShortDescription,
				x.IDCategory,
				y.CategoryName,
				x.NumberDateTour,
				x.NumberBooked,
				x.StatusDelete,
				CateDelete = y.StatusDelete
			}).Where(x => x.StatusDelete == 1 && x.CateDelete == 1).ToList();

			List<ViewTour> lstViewTour = new List<ViewTour>();
			for (int index = 0; index < lstTour.Count; index++)
			{
				ViewTour viewTour = new ViewTour();
				viewTour.IDTour = lstTour[index].IDTour;
				viewTour.TourName = lstTour[index].TourName;
				viewTour.Images = lstTour[index].Images;
				viewTour.Cost = lstTour[index].Cost;
				viewTour.Statuss = lstTour[index].Statuss;
				viewTour.ShortDescription = lstTour[index].ShortDescription;
				viewTour.IDCategory = lstTour[index].IDCategory;
				if (lstTour[index].CategoryName.Contains("Tour Du lịch"))
				{
					viewTour.CategoryName = lstTour[index].CategoryName.Substring(13);
				}
				else
				{
					viewTour.CategoryName = lstTour[index].CategoryName;
				}
				viewTour.NumberDateTour = lstTour[index].NumberDateTour;
				viewTour.NumberBooked = lstTour[index].NumberBooked;
				lstViewTour.Add(viewTour);
			}
			return lstViewTour;
		}
		public List<Category> GetListCategory()
		{
			#region Cập nhật số lượng trong Category
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
			#endregion
			return db.Categories.Where(x => x.StatusDelete == 1 && x.Quantity>0).ToList();
		}

		public List<Tour> GetListTour()
		{
			return db.Tours.Where(a => a.StatusDelete == 1).ToList();
		}
		public List<ViewTour> GetListTourByIDCategory(int idCategory)
		{
			var lstTour = GetListViewTour();
			return lstTour.Where(x => x.IDCategory == idCategory).ToList();
		}

		public List<News> GetListNews()
		{
			return db.News.Select(a => a).ToList();
		}

	}
}