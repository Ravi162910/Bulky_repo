using Bulky.DataAccess.Repository;
using BulkyWeb.Models;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitofWork _unitofwork;

        public HomeController(ILogger<HomeController> logger, IUnitofWork unitofWork)
        {
            _logger = logger;
            _unitofwork = unitofWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productlist = _unitofwork.Product.GetAll(includeproperties: "Category");
            return View(productlist);
        }

        public IActionResult Details(int productId)
        {
            ShoppingCart cart = new()
            {
                Product = _unitofwork.Product.Get(u => u.Id == productId, includeproperties: "Category"),
                Count = 1,
                ProductId = productId
            };
            return View(cart);

        }

        [HttpPost]
        public IActionResult Details(ShoppingCart cart)
        {
           var claimI = (ClaimsIdentity)User.Identity;
            var userId = claimI.FindFirst(ClaimTypes.NameIdentifier).Value;

            cart.ApplicationUserId = userId;

            ShoppingCart shoppingCart = _unitofwork.ShoppingCart.Get(u => u.ApplicationUserId == userId && u.ProductId == cart.ProductId);
            if (shoppingCart != null)
            {
                cart.Count += shoppingCart.Count;
                _unitofwork.ShoppingCart.Update(cart);
                _unitofwork.Save();
            }
            else
            {
                _unitofwork.ShoppingCart.Add(cart);
                _unitofwork.Save();
            }

            TempData["success"] = "Cart updated!";

            return RedirectToAction("Index");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
