using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanDongHo.Data;
using WebBanDongHo.Models;

namespace WebBanDongHo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class CategoriesController : Controller
    {
        private ApplicationDbContext db;
        public CategoriesController(ApplicationDbContext data)
        {

            db = data; 
        }
        //Home
        public IActionResult Index()
        {
            var listcategories = db.Categories.OrderByDescending(c=>c.id).ToList();
            return View(listcategories);
        }

        //Get Create
        public IActionResult Create()
        {
            return View();
        }

        // Post Create
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Categories categories)
        {
            // Clear all state model if it has bug
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var lsp = db.Categories.FirstOrDefault(c => c.name == categories.name);
                if (lsp != null)
                {
                    TempData["Error"] = "Loại sản phẩm đã có";
                   
                    return View(categories);
                }
                db.Categories.Add(categories);
                await db.SaveChangesAsync();
                TempData["Cre"] = "Thêm thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(categories);
        }

        //Get Edit
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var lsp = db.Categories.FirstOrDefault(c => c.id == id);
            if (lsp == null)
            {
                return NotFound();
            }
            return View(lsp);
        }

        // Post Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Categories categories)
        {   
            if (ModelState.IsValid)
            {
                db.Categories.Update(categories);
                await db.SaveChangesAsync();
                TempData["Ed"] = "Sửa thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(categories);
        }

        //Get Delete
        public IActionResult Delete(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var lsp = db.Categories.FirstOrDefault(c => c.id == id);
            if (lsp == null)
            {
                return NotFound();
            }
            return View(lsp);
        }

        // Post Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Categories categories)
        {
            var p = await db.Products.AnyAsync(p => p.categories_id == categories.id);
            if (p)
            {
                TempData["Exist"] = "Có sản phẩm tồn tại trong loại sản phẩm!";
                return View(categories);
            }
            if (ModelState.IsValid)
            {
                db.Categories.Remove(categories);
                await db.SaveChangesAsync();
                TempData["Del"] = "Xóa thành công!";
                return RedirectToAction(nameof(Index));    
            }
            return View(categories);
        }
    }
}
