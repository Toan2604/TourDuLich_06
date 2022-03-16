using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Nhom6_TourDuLich.Models;

namespace Nhom6_TourDuLich.Areas.Admin.Data
{
    public class ViewModel
    {
        public User user { get; set; }
        public Bill bill { get; set; }
        public Order order { get; set; }

        public Tour tour { get; set; }
        public Category category { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.##0}")]
        public double Thanhtien { get; set; }

    }
}