using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanDongHo.Models
{
    public class Products
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string name { get; set; }

        public string? description { get; set; }

        [Required]
        //[Column(TypeName ="Money")] //kiểu dl trong sql
        public decimal price { get; set; }

        [Required]
        public string color { get; set; }

        [Required]
        [Column(TypeName ="bit")]
        public bool isAvailable { get; set; }

        
        public string? image1 { get; set; }
        public string? image2 { get; set; }
        public string? image3 { get; set; }


        //Tạo thuộc tính FK TH từ model
        [Required]
        public int categories_id { get; set; }
        [ForeignKey("categories_id")] // Khóa ngoại tự đặt tham chiếu đến khóa chính id của bảng Categories
        public Categories Categories { get; set; } // thuộc tính điều hướng tham chiếu đến object class Categories
    }
}
