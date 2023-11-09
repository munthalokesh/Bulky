using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Bulky.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bulky.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM shoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == userId, IncludeProperties: "Product"),
                OrderHeader=new()
            };
            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                cart.price = getprice(cart);
                shoppingCartVM.OrderHeader.OrderTotal += (cart.price * cart.Count);
            }
            return View(shoppingCartVM);
        }
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == userId, IncludeProperties: "Product"),
                OrderHeader = new()
            };
            shoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUserRepository.Get(u => u.Id == userId);
            shoppingCartVM.OrderHeader.Name=shoppingCartVM.OrderHeader.ApplicationUser.Name;
            shoppingCartVM.OrderHeader.PhoneNumber = shoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            shoppingCartVM.OrderHeader.StreetAddress = shoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            shoppingCartVM.OrderHeader.City=shoppingCartVM.OrderHeader.ApplicationUser.City;
            shoppingCartVM.OrderHeader.State=shoppingCartVM.OrderHeader.ApplicationUser.State;
            shoppingCartVM.OrderHeader.PostalCode = shoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                cart.price = getprice(cart);
                shoppingCartVM.OrderHeader.OrderTotal += (cart.price * cart.Count);
            }
             return View(shoppingCartVM);
        }
        [HttpPost]
        [ActionName("Summary")]
		public IActionResult SummaryPOST()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == userId, IncludeProperties: "Product");
			shoppingCartVM.OrderHeader.OrderDate=DateTime.Now;
            shoppingCartVM.OrderHeader.ApplicationUserId=userId;
			
			shoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUserRepository.Get(u => u.Id == userId);
			

			foreach (var cart in shoppingCartVM.ShoppingCartList)
			{
				cart.price = getprice(cart);
				shoppingCartVM.OrderHeader.OrderTotal += (cart.price * cart.Count);
			}
            if(shoppingCartVM.OrderHeader.ApplicationUser.CompanyId.GetValueOrDefault()==0)
            {
                //regular account get paymnet
                shoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
                shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            }
            else
            {
                //company user so direct payment
                shoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
                shoppingCartVM.OrderHeader.PaymentStatus= SD.PaymentStatusDelayedPayment;
            }

            _unitOfWork.OrderHeaderRepository.Add(shoppingCartVM.OrderHeader);
            _unitOfWork.Save();
            foreach(var cart in shoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = shoppingCartVM.OrderHeader.Id,
                    Price = cart.price,
                    Count = cart.Count
                };
                _unitOfWork.OrderDetailRepository.Add(orderDetail);
                _unitOfWork.Save();
            }
			if (shoppingCartVM.OrderHeader.ApplicationUser.CompanyId.GetValueOrDefault() == 0)
			{
				//regular account get paymnet
				//add stripe
			}
			else
			{
				//company user so direct payment
				return RedirectToAction(nameof(OrderConfirmation),new {Id=shoppingCartVM.OrderHeader.Id});
			}
			return View(shoppingCartVM);
		}
        public IActionResult OrderConfirmation(int Id)
        {
            return View(Id);
        }
		public ActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCartRepository.Get(u => u.Id == cartId);
            cartFromDb.Count += 1;
            _unitOfWork.ShoppingCartRepository.Update(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCartRepository.Get(u => u.Id == cartId);
            if (cartFromDb.Count == 1)
            {
                _unitOfWork.ShoppingCartRepository.Remove(cartFromDb);
            }
            else
            {
                cartFromDb.Count -= 1;
                _unitOfWork.ShoppingCartRepository.Update(cartFromDb);
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public ActionResult Delete(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCartRepository.Get(u => u.Id == cartId);

            _unitOfWork.ShoppingCartRepository.Remove(cartFromDb);



            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        private double getprice(ShoppingCart cart)
        {
            if (cart.Count <= 50)
            {
                return cart.Product.Price;
            }
            else
            {
                if (cart.Count <= 100)
                {
                    return cart.Product.Price50;
                }
                else
                {
                    return cart.Product.Price100;
                }
            }
        }
    }
}
