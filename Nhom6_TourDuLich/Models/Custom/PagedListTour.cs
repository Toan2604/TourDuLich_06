using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nhom6_TourDuLich.Models.Custom
{
	public class PagedListTour
	{
		public IPagedList<ViewTour> PageTour { set; get; }
		public List<Category> Categories { get; set; }
		//public List<ViewTour> ViewTours { get; set; }
		
	}
}