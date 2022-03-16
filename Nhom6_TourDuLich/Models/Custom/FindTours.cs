using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Nhom6_TourDuLich.Models.Custom
{
	public class FindTours
	{
		public string IDCategory { get; set; }
		public string TourName { get; set; }
		[Column(TypeName = "numeric")]
		public long CostA { get; set; }
		[Column(TypeName = "numeric")]
		public long CostB { get; set; }
	}
}