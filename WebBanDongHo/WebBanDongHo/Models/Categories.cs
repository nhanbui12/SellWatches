using System.ComponentModel.DataAnnotations;

namespace WebBanDongHo.Models
{
    
    public class Categories
    {
        [Key] // primary key
        public int id { get; set; }

        [Required] // not null
        [StringLength(50)]
        public string name { get; set; }

        [StringLength(100)]
        public string? note { get; set; }
    }
}
