using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.CompilerServices;

namespace Bulky.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {

        private IUnitOfWork _UnitOfWork;
        private IWebHostEnvironment _WebHostEnvironment;
        public ProductController(IUnitOfWork UnitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _UnitOfWork = UnitOfWork;
            _WebHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _UnitOfWork.ProductRepository.GetAll().ToList();
            
            return View(objProductList);
        }

        public IActionResult Upsert(int ?id)  //update+insert
        {
            ProductVM productVM = new ProductVM();
            productVM.product = new Product();

            IEnumerable<SelectListItem> CategoryList = _UnitOfWork.CategoryRepository.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            productVM.CategoryList = CategoryList;
            ViewBag.CategoryList = CategoryList;
            if (id == null || id == 0)
            {
                //create
                return View(productVM);
            }
            else
            {
                //update
                productVM.product=_UnitOfWork.ProductRepository.Get(u=>u.Id==id);
                return View(productVM);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM obj,IFormFile? file)
        {
            
            if (ModelState.IsValid)
            {
                string wwwRootPath = _WebHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath= Path.Combine(wwwRootPath, @"Images\Products");
                    if(!string.IsNullOrEmpty(obj.product.ImgUrl))
                    {
                        var oldImgPath = Path.Combine(wwwRootPath, obj.product.ImgUrl.Trim('\\'));
                        if(System.IO.File.Exists(oldImgPath))
                        {
                            System.IO.File.Delete(oldImgPath);
                        }
                    }
                    using (var filestream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }
                    obj.product.ImgUrl= @"\Images\Products\"+fileName;
                }
                if (obj.product.Id == 0)
                {
                    _UnitOfWork.ProductRepository.Add(obj.product);
                    TempData["success"] = "Product created successfully";
                }
                else
                {
                    _UnitOfWork.ProductRepository.Update(obj.product);
                    TempData["success"] = "Product updated successfully";
                }
                
                _UnitOfWork.Save();
                
                return RedirectToAction("Index", "Product");

            }
            IEnumerable<SelectListItem> CategoryList = _UnitOfWork.CategoryRepository.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            obj.CategoryList = CategoryList;
            return View(obj);
        }
        
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? ProductFromDb = _UnitOfWork.ProductRepository.Get(u => u.Id == id);
            /*Product? ProductFromDb1 = _db.Categories.FirstOrDefault(u=>u.Id==id);
            Product? ProductFromDb2 = _db.Categories.Where(u=>u.Id==id).FirstOrDefault();*/
            if (ProductFromDb == null)

            {
                return NotFound();
            }
            return View(ProductFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Product obj = _UnitOfWork.ProductRepository.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _UnitOfWork.ProductRepository.Remove(obj);
            _UnitOfWork.Save();
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction("Index", "Product");

        }
    }
}
