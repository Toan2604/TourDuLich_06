using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nhom6_TourDuLich.Areas.Admin.Data
{
	public class LoginModel
	{
		[Key]
		public int IDUser { get; set; }

		[Required(ErrorMessage = "Phải nhập Tên đăng nhập!")]
		[DisplayName("Tên đăng nhập")]
		public string UserName { get; set; }

		[Required(ErrorMessage = "Phải nhập Mật khẩu!")]
		[DisplayName("Mật khẩu")]
		public string Passwords { get; set; }
	}
}