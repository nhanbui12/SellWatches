using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace WebBanDongHo.Models
{
    [PrimaryKey(nameof(orders_id), nameof(products_id))]
    public class OrderDetails
    {   
        [Required]
        public int orders_id { get; set; }
        [ForeignKey("orders_id")] // Khóa ngoại tự đặt tham chiếu đến khóa chính id của bảng Orders
        public Orders Orders { get; set; } // thuộc tính điều hướng tham chiếu đến object class Orders

        [Required]
        public int products_id { get; set; }
        [ForeignKey("products_id")]
        public Products Products { get; set; }

        [Required]
        public int quantityOrder { get; set; }

        [Required]
        public decimal unitPrice { get; set; }

        public decimal total {  get; set; }
    }   

}
