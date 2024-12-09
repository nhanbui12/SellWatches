using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;
using WebBanDongHo.Data;
using WebBanDongHo.Models;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;


namespace WebBanDongHo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class ProductsController : Controller
    {
        private ApplicationDbContext db;
        private IHostingEnvironment host;

        public ProductsController(ApplicationDbContext data, IHostingEnvironment hosting)
        {
            db = data;
            host = hosting;
        }
        public IActionResult Index()
        {
            //lấy bao gồm chi tiết cả loại sản phẩm liên quan đến sp đó
            var listproduct = db.Products.Include(c=>c.Categories).OrderByDescending(p =>p.id).ToList();
            return View(listproduct);
        }

        // Get Create
        public IActionResult Create()
        {
            ViewData["categories_id"] = new SelectList(db.Categories.ToList(),"id","name");
            return View();
        }

        // Post Create
        [HttpPost]
        public async Task<IActionResult> Create(Products products, IFormFile image1, IFormFile image2, IFormFile image3)
        {
            
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var sp = db.Products.FirstOrDefault(p => p.name == products.name);
                if (sp != null)
                {
                    TempData["Error"] = "Sản phẩm đã có";
                    ViewData["categories_id"] = new SelectList(db.Categories.ToList(), "id", "name");
                    return View(products);
                }
                //image1
                if (image1 != null)
                {
                    var name = Path.Combine(host.WebRootPath + "/Images", Path.GetFileName(image1.FileName));
                    await image1.CopyToAsync(new FileStream(name, FileMode.Create));
                    products.image1 = "Images/" + image1.FileName;
                }
                if(image1==null)
                {
                    products.image1 = "Images/no_images.jpg";
                }

                //image2
                if (image2 != null)
                {
                    var name = Path.Combine(host.WebRootPath + "/Images", Path.GetFileName(image2.FileName));
                    await image2.CopyToAsync(new FileStream(name, FileMode.Create));
                    products.image2 = "Images/" + image2.FileName;
                }
                if (image2 == null)
                {
                    products.image2 = "Images/no_images.jpg";
                }

                //image3
                if (image3 != null)
                {
                    var name = Path.Combine(host.WebRootPath + "/Images", Path.GetFileName(image3.FileName));
                    await image3.CopyToAsync(new FileStream(name, FileMode.Create));
                    products.image3 = "Images/" + image3.FileName;
                }
                if (image3 == null)
                {
                    products.image3 = "Images/no_images.jpg";
                }

                db.Products.Add(products);
                await db.SaveChangesAsync();
                TempData["Create"] = "Thêm thành công!";
                return RedirectToAction(nameof(Index));

            } 
            return View(products);
        }

        //Get Edit
        public IActionResult Edit(int? id)
        {
            ViewData["categories_id"] = new SelectList(db.Categories.ToList(), "id", "name");
            if (id == null)
            {
                return NotFound();
            }
            var sp = db.Products.Include(c => c.Categories).FirstOrDefault(p=>p.id == id);
            if (sp == null)
            {
                return NotFound();
            }
            return View(sp);
        }

        //Post Edit
        [HttpPost]
        public async Task<IActionResult> Edit(Products products, IFormFile image1, IFormFile image2, IFormFile image3)
        {
            var sp = db.Products.Include(c => c.Categories).AsNoTracking().FirstOrDefault(p => p.id == products.id);
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                //image1
                if (image1 != null)
                {
                    var name = Path.Combine(host.WebRootPath + "/Images", Path.GetFileName(image1.FileName));
                    await image1.CopyToAsync(new FileStream(name, FileMode.Create));
                    products.image1 = "Images/" + image1.FileName;
                }
                if (image1 == null)
                {
                    products.image1 = sp.image1;
                }

                //image2
                if (image2 != null)
                {
                    var name = Path.Combine(host.WebRootPath + "/Images", Path.GetFileName(image2.FileName));
                    await image2.CopyToAsync(new FileStream(name, FileMode.Create));
                    products.image2 = "Images/" + image2.FileName;
                }
                if (image2 == null)
                {
                    products.image2 = sp.image2;
                }

                //image3
                if (image3 != null)
                {
                    var name = Path.Combine(host.WebRootPath + "/Images", Path.GetFileName(image3.FileName));
                    await image3.CopyToAsync(new FileStream(name, FileMode.Create));
                    products.image3 = "Images/" + image3.FileName;
                }
                if (image3 == null)
                {
                    products.image3 = sp.image3;
                }

                db.Products.Update(products);
                await db.SaveChangesAsync();
                TempData["Edit"] = "Sửa thành công!";
                return RedirectToAction(nameof(Index));

            }
            return View(products);
        }
        
        //Get Delete
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var sp = db.Products.Include(c => c.Categories).FirstOrDefault(p => p.id == id);
            if (sp == null)
            {
                return NotFound();
            }
            ViewBag.CategoriesName = db.Categories
                .Where(c => c.id == sp.categories_id)
                .Select(c=>new {c.name});
            return View(sp);
        }
        
        //Post Delete
        [HttpPost]
        public async Task<IActionResult> Delete(Products products)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                db.Products.Remove(products);
                await db.SaveChangesAsync();
                TempData["Delete"] = "Xóa thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(products);
        } 
    }
}
