using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanDongHo.Models
{
    public class Comments
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string content { get; set; }

        public int star { get; set; }

        [Required]
        public string userComment { get; set; }

        public DateTime postTime { get; set; }

        //Tạo thuộc tính FK TH từ model
        [Required]
        public int products_id { get; set; }
        [ForeignKey("products_id")] 
        public Products Products { get; set; }

        //Tạo thuộc tính FK TH từ model
        [Required]
        public string users_id { get; set; }
        [ForeignKey("users_id")] 
        public ApplicationUsers ApplicationUsers { get; set; } 
    }
}
