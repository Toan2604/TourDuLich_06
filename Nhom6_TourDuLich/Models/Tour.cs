namespace Nhom6_TourDuLich.Models
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Entity.Spatial;

	[Table("Tour")]
	public partial class Tour
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public Tour()
		{
			Bills = new HashSet<Bill>();
		}

		[Key]
		[Display(Name = "Mã tour")]
		public int IDTour { get; set; }

		[Required(ErrorMessage ="Phải nhập Tên tour!")]
		[StringLength(200, ErrorMessage ="Độ dài Tên tour không vượt quá 200 ký tự!")]
		[Display(Name = "Tên tour")]
		public string TourName { get; set; }

		[Required(ErrorMessage ="Phải nhập địa chỉ!")]
		[StringLength(200, ErrorMessage ="Độ dài Địa chỉ không vượt quá 200 ký tự!")]
		[Display(Name = "Địa chỉ")]
		public string Addresss { get; set; }

		[Required(ErrorMessage ="Phải nhập Đơn giá!")]
		[Display(Name = "Đơn giá")]
		public int Cost { get; set; }

		[Column(TypeName = "text")]
		[Display(Name = "Hình ảnh")]
		public string Images { get; set; }

		[Required(ErrorMessage ="Phải nhập Tình trạng tour!")]
		[StringLength(30)]
		[Display(Name = "Tình trạng tour")]
		public string Statuss { get; set; }

		[Required(ErrorMessage ="Phải nhập Mô tả ngắn!")]
		[StringLength(500, ErrorMessage ="Độ dài Mô tả ngắn không vượt quá 500 ký tự!")]
		[Display(Name = "Mô tả ngắn")]
		public string ShortDescription { get; set; }

		[Required(ErrorMessage ="Phải nhập Mô tả chi tiết!")]
		[StringLength(1000, ErrorMessage ="Độ dài Mô tả chi tiết không vượt quá 1000 ký tự!")]
		[Display(Name = "Mô tả chi tiết")]
		public string DetailDescription { get; set; }

		[Display(Name = "Mã danh mục")]
		public int IDCategory { get; set; }

		[Display(Name = "Thời gian đi")]
		[Required(ErrorMessage ="Phải nhập Thời gian đi!")]
		[Column(TypeName = "numeric")]
		public int NumberDateTour { get; set; }

		[Display(Name = "Số lượng đã đặt")]
		[Column(TypeName = "numeric")]
		public int NumberBooked { get; set; }

		[Display(Name = "Tình trạng xóa")]
		public int StatusDelete { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<Bill> Bills { get; set; }

		public virtual Category Category { get; set; }
	}
}
