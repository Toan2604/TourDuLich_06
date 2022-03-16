using Nhom6_TourDuLich.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nhom6_TourDuLich.Areas.Admin.Data
{
	public class BillTourModel
	{
		public Tour tour { get; set; }
		public Bill bill { get; set; }
	}
}