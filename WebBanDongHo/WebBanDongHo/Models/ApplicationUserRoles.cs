using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanDongHo.Models
{
    public class ApplicationUserRoles : IdentityUserRole<string>
    {
        public DateTime timeActive { get; set; }
    }
}
