using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebBanDongHo.Data;
using WebBanDongHo.Models;

namespace WebBanDongHo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class StatisticalController : Controller
    {
        private ApplicationDbContext db;
        public StatisticalController(ApplicationDbContext data)
        {

            db = data;
        }
        public IActionResult Index()
        {
            return View(new List<dynamic>());
        }


        [HttpPost]
        public IActionResult Index(string option)
        {
            IEnumerable<dynamic> list = null;
            if (option == "1")
            {
                TempData["barChart"] = "Bảng dữ liệu và biểu đồ cột thống kê doanh thu bán theo tháng";
                list = RevenueByMonth();
            }
            else if(option == "2")
            {
                TempData["myChart"] = "Bảng dữ liệu và biểu đồ tròn thống kê số lượng sản phẩm theo loại sản phẩm";
                list = CountProductsInCategories();
            }
            return View("Index",list);
        }


        //[HttpPost]
        private IEnumerable<dynamic> RevenueByMonth()
        {
            var result = from o in db.Orders
                         join od in db.OrderDetails
                         on o.id equals od.orders_id
                         group od by new
                         {
                             month = o.orderDate.Month,
                             year = o.orderDate.Year,
                         }
                         into groupGetData
                         select new
                         {
                             MonthOrder = groupGetData.Key.month,
                             YearOrder = groupGetData.Key.year,
                             TotalForOrder = groupGetData.Sum(od=>od.total)
                         };
            var list = result.OrderBy(y => y.YearOrder)
                .ThenBy(m => m.MonthOrder)
                .ToList();

            return list;
        }


        //[HttpPost]
        private IEnumerable<dynamic> CountProductsInCategories()
        {
            var countProducts = from p in db.Products
                           join c in db.Categories
                           on p.categories_id equals c.id
                           group c by new
                           {
                               c.id,
                               c.name
                           }
                           into groupGetData
                           select new
                           {
                               CategoriesId = groupGetData.Key.id,
                               CategoriesName = groupGetData.Key.name,
                               CountProductsInThis = groupGetData.Count()
                           };


            var list = countProducts
                .OrderBy(c=>c.CategoriesId)
                .ToList();

            return list;
        }
    }
}
