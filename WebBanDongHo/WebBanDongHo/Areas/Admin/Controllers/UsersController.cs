using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using WebBanDongHo.Data;
using WebBanDongHo.Models;

namespace WebBanDongHo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class UsersController : Controller
    {
        RoleManager<IdentityRole> roleManager;
        UserManager<ApplicationUsers> userManager;
        ApplicationDbContext db;

        public UsersController(RoleManager<IdentityRole> rolemanager, UserManager<ApplicationUsers> usermanager, ApplicationDbContext data)
        {
            roleManager = rolemanager;
            userManager = usermanager;
            db = data;
        }

        public IActionResult Index()
        {
           var list = db.ApplicationUsers.OrderByDescending(c=>c.Id).ToList();
            return View(list);
        }


        // Get Lock
        public IActionResult Lock(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = db.ApplicationUsers.FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // Post Lock
        [HttpPost]
        public async Task<IActionResult> Lock(ApplicationUsers users)
        {
            if (ModelState.IsValid)
            {
                var user = db.ApplicationUsers.FirstOrDefault(u => u.Id == users.Id);
                if (user == null)
                {
                    return NotFound();
                }
                user.LockoutEnd = DateTime.Now.AddYears(1000);
                await db.SaveChangesAsync();
                TempData["Lock"] = "Khóa tài khoản thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(users);
        }

        // Get Open
        public IActionResult Open(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = db.ApplicationUsers.FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // Post Open
        [HttpPost]
        public async Task<IActionResult> Open(ApplicationUsers users)
        {
            if (ModelState.IsValid)
            {
                var user = db.ApplicationUsers.FirstOrDefault(u => u.Id == users.Id);
                if (user == null)
                {
                    return NotFound();
                }
                user.LockoutEnd = null;
                await db.SaveChangesAsync();
                TempData["Open"] = "Mở khóa tài khoản thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(users);
        }


        // Get Delete
        public IActionResult Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = db.ApplicationUsers.FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // Post Delete
        [HttpPost]
        public async Task<IActionResult> Delete(ApplicationUsers users)
        {
            if (ModelState.IsValid)
            {
                var user = db.ApplicationUsers.FirstOrDefault(u => u.Id == users.Id);
                if (user == null)
                {
                    return NotFound();
                }
                db.ApplicationUsers.Remove(user);
                await db.SaveChangesAsync();
                TempData["Del"] = "Xóa tài khoản thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(users);
        }

        // Get EditUserRole
        public IActionResult EditUserRole()
        {
            ViewData["users_id"] = new SelectList(db.ApplicationUsers.Where(u => u.LockoutEnd < DateTime.Now || u.LockoutEnd == null).ToList(), "Id", "UserName");
            ViewData["roles_id"] = new SelectList(roleManager.Roles.ToList(), "Name", "Name");
            return View();
        }

        //Post EditUserRole
        [HttpPost]
        public async Task<IActionResult> EditUserRole(ApplicationUserRoles ur)
        {
            if (ModelState.IsValid)
            {
                // get user current
                var user = db.ApplicationUsers.FirstOrDefault(u => u.Id == ur.UserId);
                // Test user and role together isavailable

                var exist = await userManager.IsInRoleAsync(user, ur.RoleId);
                if (exist)
                {
                    TempData["warning"] = "Tài khoản đã có vai trò này!";
                    ViewData["users_id"] = new SelectList(db.ApplicationUsers.Where(u => u.LockoutEnd < DateTime.Now || u.LockoutEnd == null).ToList(), "Id", "UserName");
                    ViewData["roles_id"] = new SelectList(roleManager.Roles.ToList(), "Name", "Name");
                    return View();
                }

                // get data in database about user and role current

                var getisavailabledata = db.ApplicationUserRoles
                    .AsNoTracking()
                    .Where(app => app.UserId == user.Id)
                    .FirstOrDefault();
                // if exist remove role this after set new roleo and save time
                if (getisavailabledata != null)
                {
                    var getnext = await db.Roles
                                .AsNoTracking()
                                .FirstOrDefaultAsync(get => get.Id == getisavailabledata.RoleId);
                   await userManager.RemoveFromRoleAsync(user, getnext.Name);    
                }
                
                // Test data about user and role saved?
                var thisuserrole = await db.ApplicationUserRoles
                                  .FirstOrDefaultAsync(get => get.UserId == user.Id && get.RoleId == ur.RoleId);
                
                // Had data, update timeActive
                if (thisuserrole != null)
                {
                    thisuserrole.timeActive = DateTime.Now.AddMinutes(-1);
                    //update database
                    db.ApplicationUserRoles.Update(thisuserrole); 
                }
                else
                {
                    // search role in form select
                    var thisrole = await db.Roles
                        .AsNoTracking()
                        .FirstOrDefaultAsync(r => r.Name == ur.RoleId);
                    if(thisrole != null)
                    { 
                        // create new
                        var newuserrole = new ApplicationUserRoles
                        {
                            UserId = user.Id,
                            RoleId = thisrole.Id,
                            timeActive = DateTime.Now.AddMinutes(-1)
                        };

                        db.ApplicationUserRoles.Add(newuserrole);
                    }
                }
                await db.SaveChangesAsync();
                TempData["save"] = "Phân quyền thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
    }
}
