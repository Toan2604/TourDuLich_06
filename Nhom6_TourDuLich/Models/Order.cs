namespace Nhom6_TourDuLich.Models
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Data.Entity.Spatial;

	public partial class Order
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public Order()
		{
			Bills = new HashSet<Bill>();
		}

		[Key]
		[Display(Name = "Mã Order")]
		public int IDOrder { get; set; }

		[Display(Name = "Mã người dùng")]
		public int IDUser { get; set; }

		[Display(Name = "Ngày đặt")]
		public DateTime BookingDate { get; set; }

		[Display(Name = "Tình trạng xóa")]
		public int StatusDelete { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<Bill> Bills { get; set; }

		public virtual User User { get; set; }
	}
}
