using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Nhom6_TourDuLich.Models.Custom
{
	public class RegisterModel
	{
		[Key]
		public int IDUser { get; set; }

		[Required(ErrorMessage = "Phải nhập Tên đăng nhập!")]
		[DisplayName("Tên đăng nhập")]
		[StringLength(30, MinimumLength = 4, ErrorMessage = "Tên đăng nhập phải dài 4 đến 30 ký tự!")]
		public string UserName { get; set; }

		[Required(ErrorMessage = "Phải nhập Họ tên đầy đủ!")]
		[DisplayName("Họ và tên")]
		[StringLength(50, ErrorMessage = "Độ dài không vượt quá 50 ký tự!")]
		public string FullName { get; set; }

		[Required(ErrorMessage = "Phải nhập Email!")]
		[EmailAddress(ErrorMessage = "Sai định dạng Email!")]
		[StringLength(50, ErrorMessage = "Độ dài không vượt quá 50 ký tự!")]
		[DisplayName("Email")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Phải nhập Số điện thoại!")]
		[Phone(ErrorMessage = "Sai định dạng số điện thoại!")]
		[StringLength(15, ErrorMessage = "Độ dài không vượt quá 15 ký tự!")]
		[DisplayName("Số điện thoại")]
		public string PhoneNumber { get; set; }

		[Required(ErrorMessage = "Phải nhập Mật khẩu!")]
		[DisplayName("Mật khẩu")]
		[StringLength(50, MinimumLength = 4, ErrorMessage = "Mật khẩu phải dài ít nhất 4 ký tự và tối đa 50 ký tự")]
		public string Passwords { get; set; }

		[DisplayName("Phân quyền")]
		[Column(TypeName = "numeric")]
		public int Roles { get; set; }

		[DisplayName("Tình trạng xóa")]
		[Column(TypeName = "numeric")]
		public int StatusDelete { get; set; }
	}
}