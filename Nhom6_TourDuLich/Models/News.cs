namespace Nhom6_TourDuLich.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class News
    {
        [Key]
        [Display(Name ="Mã tin tức")]
        public int IDNews { get; set; }

        [Required(ErrorMessage ="Phải nhập Tiêu đề!")]
        [StringLength(200, ErrorMessage = "Độ dài Tiêu đề không vượt qúa 200 ký tự!")]
        [Display(Name ="Tiêu đề")]
        public string Title { get; set; }

        [Column(TypeName = "text")]
        [Display(Name ="Hình ảnh")]
        public string Images { get; set; }

        [Column(TypeName = "ntext")]
        [Required(ErrorMessage ="Phải nhập Mô tả!")]
        [Display(Name ="Mô tả")]
        public string Descriptions { get; set; }
    }
}
