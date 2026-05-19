using Bulky.DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Models;
using Models.Models.ViewModels;
using Stripe.Checkout;
using System.Security.Claims;
using Utilities;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitofWork _unitofwork;

        [BindProperty]
        public ShoppingCartVM shoppingCartVM { get; set; }

        public CartController(IUnitofWork unitofWork) 
        {
            _unitofwork = unitofWork;
        }

        public IActionResult Index()
        {
            var claimidentity = (ClaimsIdentity)User.Identity;
            var userId = claimidentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCartVM = new()
            {
                shoppingCartsList = _unitofwork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeproperties: "Product"),
                OrderHeader = new()
            };

            foreach (var cart in shoppingCartVM.shoppingCartsList) 
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(shoppingCartVM);
        }

        public double GetPriceBasedOnQuantity(ShoppingCart shoppingCart) 
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else if (shoppingCart.Product.Price <= 100)
            {
                return shoppingCart.Product.Price50;
            }
            else 
            {
                return shoppingCart.Product.Price100;
            }
        }

        public IActionResult Plus(int cartId) 
        {
            var cartfromdb = _unitofwork.ShoppingCart.Get(u => u.Id == cartId);
            cartfromdb.Count += 1;
            _unitofwork.ShoppingCart.Update(cartfromdb);
            _unitofwork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cartfromdb = _unitofwork.ShoppingCart.Get(u => u.Id == cartId);
            if (cartfromdb.Count <= 1)
            {
                _unitofwork.ShoppingCart.Remove(cartfromdb);
            }
            else
            {
                cartfromdb.Count -= 1;
                _unitofwork.ShoppingCart.Update(cartfromdb);
            }
            _unitofwork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId) 
        {
            var carfromdb = _unitofwork.ShoppingCart.Get(u => u.Id == cartId);
            _unitofwork.ShoppingCart.Remove(carfromdb);
            _unitofwork.Save();
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public IActionResult Summary() 
        {
            var claimidentity = (ClaimsIdentity)User.Identity;
            var userId = claimidentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCartVM = new()
            {
                shoppingCartsList = _unitofwork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeproperties: "Product"),
                OrderHeader = new()
            };

            shoppingCartVM.OrderHeader.ApplicationUser = _unitofwork.ApplicationUser.Get(u => u.Id == userId);

            shoppingCartVM.OrderHeader.Name = shoppingCartVM.OrderHeader.ApplicationUser.Name;
            shoppingCartVM.OrderHeader.PhoneNumber = shoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            shoppingCartVM.OrderHeader.StreetAddress = shoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            shoppingCartVM.OrderHeader.City = shoppingCartVM.OrderHeader.ApplicationUser.City;

            foreach (var cart in shoppingCartVM.shoppingCartsList) 
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(shoppingCartVM);
        }

        [ActionName("Summary")]
        [HttpPost]
        [Authorize]
        public IActionResult SummaryPOST() 
        {
            var claimidentity = (ClaimsIdentity)User.Identity;
            var userId = claimidentity.FindFirst(ClaimTypes.NameIdentifier).Value;


            shoppingCartVM.shoppingCartsList = _unitofwork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeproperties: "Product");

            shoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            shoppingCartVM.OrderHeader.ApplicationUserId = userId;

            ApplicationUser applicationUser = _unitofwork.ApplicationUser.Get(u => u.Id == userId);

            foreach (var cart in shoppingCartVM.shoppingCartsList) 
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                shoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else 
            {
                shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                shoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }

            _unitofwork.OrderHeader.Add(shoppingCartVM.OrderHeader);
            _unitofwork.Save();

            foreach (var cart in shoppingCartVM.shoppingCartsList) 
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = shoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count,
                };
                _unitofwork.OrderDetail.Add(orderDetail);
                _unitofwork.Save();
            }

            if (applicationUser.CompanyId.GetValueOrDefault() == 0) 
            {
                var domainlink = "https://localhost:7200/";
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = "https://example.com/success",
                    CancelUrl = domainlink + "customer/cart/index",
                    LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach (var item in shoppingCartVM.shoppingCartsList) 
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "nzd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }
                var service = new SessionService();
                Session session = service.Create(options);
                _unitofwork.OrderHeader.UpdateStripePaymentID(shoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitofwork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
            return RedirectToAction(nameof(OrderConfirmation), new { id = shoppingCartVM.OrderHeader.Id });
        }

        public IActionResult OrderConfirmation(int id) 
        {
            OrderHeader orderHeader = _unitofwork.OrderHeader.Get(u => u.Id == id, includeproperties: "ApplicationUser");
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment) 
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                if (session.PaymentStatus.ToLower() == "Paid") 
                {
                    _unitofwork.OrderHeader.UpdateStripePaymentID(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    _unitofwork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                }
                HttpContext.Session.Clear();
            }

            List<ShoppingCart> shoppingCarts = _unitofwork.ShoppingCart.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

            _unitofwork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitofwork.Save();
            return View(id);
        }

    }
}
