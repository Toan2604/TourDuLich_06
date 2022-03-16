namespace Nhom6_TourDuLich.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Category")]
    public partial class Category
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Category()
        {
            Tours = new HashSet<Tour>();
        }

        [Key]
        [Display(Name ="Mã danh mục")]
        public int IDCategory { get; set; }

        [Required(ErrorMessage ="Phải nhập Tên danh mục!")]
        [StringLength(200, ErrorMessage ="Độ dài Tên danh mục không vượt quá 200 ký tự!")]
        [Display(Name ="Tên danh mục")]
        public string CategoryName { get; set; }

        [Display(Name ="Số lượng tour")]
        public int Quantity { get; set; }

        [Column(TypeName = "text")]
        [Display(Name ="Hình ảnh")]
        public string Images { get; set; }

        [Display(Name ="Tình trạng xóa")]
        public int StatusDelete { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tour> Tours { get; set; }
    }
}
