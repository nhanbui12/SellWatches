using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebBanDongHo.Models
{
    public class ApplicationUsers:IdentityUser
    {
        //Tên
        [Required]
        [StringLength(50)]
        public string firstName { get; set; }

        //Họ
        [Required]
        [StringLength(50)]
        public string lastName { get; set; }
    }
}
