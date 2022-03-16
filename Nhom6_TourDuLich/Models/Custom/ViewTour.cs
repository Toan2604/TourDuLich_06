using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nhom6_TourDuLich.Models.Custom
{
	public class ViewTour
	{
		//public int IDTour { get; set; }
		public int IDTour { get; set; }
		public string TourName { get; set; }
		public string Images { get; set; }
		public int Cost { get; set; }
		public string Statuss { get; set; }
		public string ShortDescription { get; set; }
		public int IDCategory { get; set; }

		public string CategoryName { get; set; }
		public int NumberDateTour { get; set; }
		public int NumberBooked { get; set; }
	}
}