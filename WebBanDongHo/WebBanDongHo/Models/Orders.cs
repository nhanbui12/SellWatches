using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanDongHo.Models
{
    public class Orders
    {
        [Key] // primary key
        public int id { get; set; }

        public DateTime orderDate { get; set; }

        public string? address { get; set; }
        //Tạo thuộc tính FK TH từ model
        [Required]
        public string users_id { get; set; }
        [ForeignKey("users_id")] 
        public ApplicationUsers ApplicationUsers { get; set; }
    }
}
