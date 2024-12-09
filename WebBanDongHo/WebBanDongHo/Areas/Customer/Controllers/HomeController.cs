using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using WebBanDongHo.Data;
using WebBanDongHo.Helper;
using WebBanDongHo.Models;
using WebBanDongHo.Utilities;
using X.PagedList;

namespace WebBanDongHo.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        UserManager<ApplicationUsers> userManager;
        private ApplicationDbContext db;
        private readonly IVnPayService vnPayservice;
        public HomeController(UserManager<ApplicationUsers> usermanager, ApplicationDbContext data, IVnPayService vnPayser)
        {
            userManager = usermanager;
            db = data;
            vnPayservice = vnPayser;
        }

        // Anybody
        // Get Index
        [AllowAnonymous]
        public IActionResult Index(int? page)
        {
            int pageSize = 12;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            //var list = db.Products.Include(p=>p.Categories).ToList();
            var list = db.Products.Include(p => p.Categories).AsNoTracking().OrderByDescending(x => x.id);
            PagedList<Products> listproduct = new PagedList<Products>(list,pageNumber,pageSize);
            return View(listproduct);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        //Post Index for Search
        [HttpPost]
        //Post Index action method
        public IActionResult Index(decimal? pricemin, decimal? pricemax, string? nameproduct, string? namecategory, int?page)
        {
            int pageSize = 12;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            // Price
            var listproduct = db.Products.Include(c => c.Categories).ToList();
            
            if (pricemin != null)
            {
                listproduct = db.Products.Include(c => c.Categories)
                .Where(s => s.price >= pricemin)
                .OrderBy(s=>s.price)
                .ToList();
            }
            if (pricemax != null)
            {
                listproduct = db.Products.Include(c => c.Categories)
                .Where(s => s.price <= pricemax)
                .OrderByDescending(s => s.price)
                .ToList();
            } 
            if(pricemin != null && pricemax != null)
            {
                listproduct = db.Products.Include(c => c.Categories)
                .Where(s => s.price >= pricemin && s.price <= pricemax)
                .OrderBy(s=>s.price)
                .ToList();
            }
            // Name Product
            if (nameproduct!=null)
            {
                listproduct = db.Products.Include(c => c.Categories)
                .Where(s => s.name.Contains(nameproduct)).ToList();
            }
            // Name Category
            if (namecategory != null)
            { 
                listproduct = db.Products.Include(c => c.Categories)
                .Where(s => s.Categories.name.Contains(namecategory)).ToList();
            }
            PagedList<Products> resultlist = new PagedList<Products>(listproduct, pageNumber, pageSize);
            return View(resultlist);
        }

        // login 
        [Authorize]
        //Get Detail about Product
        public async Task<IActionResult> Detail(int? id)
        {
            if(id == null)
            {
               return NotFound();
            }
            var pro = db.Products.Include(p => p.Categories).FirstOrDefault(t => t.id == id);
            if(pro == null)
            {
                return NotFound();
            }
            //get user current
            var user = await userManager.GetUserAsync(User);
            var listcomment = db.Comments.Where(c => c.products_id == id).ToList();
            if (listcomment.Count > 0)
            {
                ViewBag.listcomment = listcomment;
            }
            return View(pro);
        }

        
        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ActionName("Detail")]
        // Add product to Cart (Post detail Product)
        public IActionResult AddToCart(int? id, int quantity)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = db.Products.Include(c => c.Categories).FirstOrDefault(c => c.id == id);
            if (product == null)
            {
                return NotFound();
            }

            // gio hang truoc khi them sp
            var listCart = HttpContext.Session.Get<List<CartItem>>("listCart");
            //Neu chua co
            if (listCart == null)
            {
                listCart = new List<CartItem>();
            }
            // Tim trong gio hang co sp co ma id chua
            var item = listCart.FirstOrDefault(p => p.product_id == id);
            //co roi
            if(item != null)
            {
                item.quantity += quantity;
            }
            //chua co
            else
            {
                item = new CartItem
                {
                    product_id = product.id,
                    product_name = product.name,
                    product_image1 = product.image1,
                    product_price = product.price,
                    quantity = quantity,

                };
                //Them item vao gio hang
                listCart.Add(item);
            }
            
            //set lai session
            HttpContext.Session.Set("listCart", listCart);
            TempData["Add"] = "Giỏ hàng đã thêm sản phẩm";
            //hien thi lai detail sp
            return View(product);
        }

        [Authorize(Roles = "Customer")]
        // View Cart
        public IActionResult Cart()
        {
            var listCart = HttpContext.Session.Get<List<CartItem>>("listCart");
            
            if (listCart == null)
            {
                listCart = new List<CartItem>();
                if(listCart.Count == 0)
                {
                    TempData["Massage"] = "Không có sản phẩm trong giỏ hàng!";
                }
            }
            if (listCart.Count == 0)
            {
                TempData["Massage"] = "Không có sản phẩm trong giỏ hàng!";
            }
            return View(listCart);
        }

        // Get Delete product from Cart
        //if sl > 1 delete - 1/ 1 button delete, if sl = 1 outside Cart
        public IActionResult Delete(int? id)
        {
            var listCart = HttpContext.Session.Get<List<CartItem>>("listCart");
            if (listCart != null)
            {
                var pro = listCart.FirstOrDefault(p => p.product_id == id);
                if (pro != null)
                {
                    if (pro.quantity == 1)
                    {
                        listCart.Remove(pro);
                        TempData["Del"] = "Đã xóa sản phẩm khỏi giỏ hàng!";
                    }
                    else
                    {
                        pro.quantity--;
                        TempData["Desc"] = "Đã giảm 1 số lượng sản phẩm!";
                    }
                    HttpContext.Session.Set("listCart", listCart);
                    
                    if (listCart.Count == 0)
                    {
                        TempData["Massage"] = "Không có sản phẩm trong giỏ hàng!";
                    }
                }
            }
            return RedirectToAction(nameof(Cart));
        }

        
        [HttpPost]
        [ActionName("Delete")]
        // Post Delete product from cart
        public IActionResult DeleteFromCart(int? id)
        { 
            var listCart = HttpContext.Session.Get<List<CartItem>>("listCart");
            if (listCart != null)
            {
                var pro = listCart.FirstOrDefault(p => p.product_id == id);
                if (pro != null)
                {
                    if (pro.quantity == 1)
                    {
                        listCart.Remove(pro);
                        TempData["Del"] = "Đã xóa sản phẩm khỏi giỏ hàng!";
                    }
                    else
                    {
                        pro.quantity--;
                        TempData["Desc"] = "Đã giảm 1 số lượng sản phẩm!";
                    }
                    
                    HttpContext.Session.Set("listCart", listCart);
                    
                    if (listCart.Count == 0)
                    {
                        TempData["Massage"] = "Không có sản phẩm trong giỏ hàng!";
                    }
                }
            }
            return RedirectToAction(nameof(Cart));
        }

        // Comment
        [HttpPost]
        public async Task<IActionResult> Comment(int id, string content)
        {
            //get user current
            var user = await userManager.GetUserAsync(User);
            string usercomment;
            if (!string.IsNullOrEmpty(user.UserName))
            {
                usercomment = user.UserName;
            }
            else if (!string.IsNullOrEmpty(user.firstName) && !string.IsNullOrEmpty(user.lastName))
            {
                usercomment = $"{user.lastName} {user.firstName}";
            }
            else if (!string.IsNullOrEmpty(user.firstName))
            {
                usercomment = user.firstName;
            }
            else
            {
                usercomment = user.lastName;
            }

            // create object comment new
            var comment = new Comments
            {
                content = content,
                postTime = DateTime.Now,
                userComment = usercomment,
                users_id = user.Id,
                products_id = id,
                
            };

            // add comment to database
            db.Comments.Add(comment);
            db.SaveChanges();

            // result in View
            TempData["AddComment"] = "Bình luận thành công!";
            TempData["Post"] = true;

            return RedirectToAction("Detail", new { id = id });
        }

        [Authorize(Roles = "Customer")]
        // Get Info Order
        public async Task<IActionResult> OrderCart()
		{
			//get user current
			var user = await userManager.GetUserAsync(User);
			//if user login yet
			if (user == null)
			{
				return RedirectToAction("Login", "Account");
			}
            // user logined
            var order = new Orders
            {
                orderDate = DateTime.Now,
                users_id = user.Id,
                ApplicationUsers = user,
            };
			//get list cart
			var listCart = HttpContext.Session.Get<List<CartItem>>("listCart");

			var infoOrder = new InfoOrdes
			{
				orders = order,
				cartItems = listCart
			};
			return View(infoOrder);
		}
        
        
        [Authorize(Roles = "Customer")]
        [HttpPost]
		[ActionName("OrderCart")]
        // Checkout VNPay
		public async Task<IActionResult> Checkout(InfoOrdes infoOrdes,string payment, string dc)
		{
			//get user current
			var user = await userManager.GetUserAsync(User);
			//if user login yet
			if (user == null)
			{
				return RedirectToAction("Login", "Account");
			}

            TempData["address"] = dc;
            infoOrdes.orders.ApplicationUsers = user;

            // Get info Cart
			var listCart = HttpContext.Session.Get<List<CartItem>>("listCart");
            infoOrdes.cartItems = listCart;
            ModelState.Clear();
			if (ModelState.IsValid)
            {
                if (payment == "Thanh toán VNPay") // test action 
                {
                    var vnPaymodel = new VnPaymentRequestModel
                    {
                        OrderId = infoOrdes.orders.id + 1000,
                        Name = $"{infoOrdes.orders.ApplicationUsers.lastName} {infoOrdes.orders.ApplicationUsers.firstName}",
                        CreatedDate = DateTime.Now,
                        TotalMoney = (double)Math.Floor(infoOrdes.cartItems.Sum(c => c.quantity * c.product_price)),
                    };  
					return Redirect(vnPayservice.CreatePaymentUrl(HttpContext, vnPaymodel));
                }
            }
			return View();
		}

        // Result View when Customer checkout error, fail
        [Authorize]
        public IActionResult PaymentFail()
        {
            return View();
        }

        //Vnpay
        [Authorize]
		public async Task<IActionResult> PaymentCallBack(InfoOrdes infoOrdes)
		{
            var response = vnPayservice.PaymentExecute(Request.Query);
            if(response == null || response.VnPayResponseCode != "00")
            {
                TempData["PayError"] = "Thanh toán thất bại!";
                return RedirectToAction(nameof(PaymentFail));
            }
            string dc = TempData["address"] as string;
            //get user current
            var user = await userManager.GetUserAsync(User);
            var order = new Orders
            {
                orderDate = DateTime.Now,
                users_id = user.Id,
                ApplicationUsers = user,
                address = dc
            };
            
            // add table Orders and save to database
            db.Orders.Add(order);
            db.SaveChanges();

            var listCart = HttpContext.Session.Get<List<CartItem>>("listCart");
            var infoOrder = new InfoOrdes
            {
                orders = order,
                cartItems = listCart
            }; 

            // Save data with table OrderDetails
            foreach(var item in listCart)
            {
                var orderdetail = new OrderDetails
                {
                    orders_id = order.id,
                    products_id = item.product_id,
                    quantityOrder = item.quantity,
                    unitPrice = item.product_price,
                    total = item.product_price * item.quantity       
                };
                db.OrderDetails.Add(orderdetail);   
            }
            db.SaveChanges();
            TempData["PayMessage"] = "Thanh toán thành công!";
            //Delete product in cart after checkout success
            HttpContext.Session.Set("listCart", new List<CartItem>());
            return View();
		}

        [Authorize(Roles = "Admin")]
        // Get OrderDetails
        public IActionResult OrderDetails()
        {
			
			var list = db.OrderDetails
                .Include(od => od.Orders)
                .ThenInclude(o=>o.ApplicationUsers)
                .Include(od=>od.Products)
                .OrderByDescending(od=>od.Orders.orderDate)
                .ToList();
            return View(list);
        }

        //Compare price category together
        public IActionResult ComparePrice(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var pro = db.Products
                .Include(p => p.Categories)
                .FirstOrDefault(t => t.id == id);
            if (pro == null)
            {
                return NotFound();
            }
            var list = db.Products
                .Include(p => p.Categories)
                .Where(p => p.categories_id == pro.categories_id && p.id != pro.id && p.name != pro.name).ToList();

            var comparelist = list
                .Select(p => new
                {
                    Products = p,
                    distance = Distance2Products(pro, p)
                })
                .OrderBy(p => p.distance)
                .Select(s => s.Products)
                .Take(4)
                .ToList();
            return PartialView("_ComparePricePartialView", comparelist);
        }


        // Compare price between two products
        public double Distance2Products(Products product1, Products product2)
        {
            // Search max, min price
            var minprice = db.Products.Min(c => c.price);
            var maxprice = db.Products.Max(c => c.price);
            // Calculator distance with price is 0 or 1
            var y1 = (product1.price - minprice) / (maxprice - minprice);
            var y2 = (product2.price - minprice) / (maxprice - minprice);
            // Distance with price
            double disprice = (double)Math.Abs(y1 - y2);//manhattan
            return disprice;
        }
    }
    
}
