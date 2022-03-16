using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nhom6_TourDuLich.Models;

namespace Nhom6_TourDuLich.Models.Custom
{
	public class Cart
	{
		CSDL_TOURDB db = new CSDL_TOURDB();
		public int iIDTour { get; set; }
		public string sTourName { get; set; }
		public string sImage { get; set; }
		public int iCost { get; set; }
		public int iQuantity { get; set; }
		public int iTotal
		{
			get { return iCost * iQuantity; }
		}
		public Cart(int id)
		{
			iIDTour = id;
			Tour tour = db.Tours.Find(id);
			sTourName = tour.TourName;
			sImage = tour.Images;
			iCost = int.Parse(tour.Cost.ToString());
			iQuantity = 1;
		}
	}
}