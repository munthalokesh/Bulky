using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Bulky.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _UnitOfWork;

        public HomeController(ILogger<HomeController> logger,IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _UnitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products=_UnitOfWork.ProductRepository.GetAll(IncludeProperties:"Category");
            return View(products);
        }
        public IActionResult Details(int ProductId)
        {
            var product = _UnitOfWork.ProductRepository.Get(u => u.Id == ProductId, IncludeProperties: "Category");
            ShoppingCart cart = new()
            {
                Product = product,
                ProductId = ProductId,
                Count = 1
            };
            
            return View(cart);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            if(ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                shoppingCart.ApplicationUserId = userId;
                ShoppingCart cardfromdb = _UnitOfWork.ShoppingCartRepository.Get(u => u.ApplicationUserId == userId && u.ProductId == shoppingCart.ProductId);
                if (cardfromdb != null)
                {
                    cardfromdb.Count += shoppingCart.Count;
                    _UnitOfWork.ShoppingCartRepository.Update(cardfromdb);//entity by default tracks thing to stop tracking changed the I repository
                }
                else
                {
                    _UnitOfWork.ShoppingCartRepository.Add(shoppingCart);
                }
                _UnitOfWork.Save();
                TempData["success"] = "Cart updated Successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(shoppingCart);
            }    
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