using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bulky.Areas.Admin.Controllers
{
    [Authorize(Roles =SD.Role_Admin)]
    [Area("Admin")]
    public class CategoryController : Controller
    {

        private IUnitOfWork _UnitOfWork;
        public CategoryController(IUnitOfWork UnitOfWork)
        {
            _UnitOfWork = UnitOfWork;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _UnitOfWork.CategoryRepository.GetAll().ToList();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "name and order cannot be same");
            }
            if (ModelState.IsValid)
            {
                _UnitOfWork.CategoryRepository.Add(obj);
                _UnitOfWork.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index", "Category");

            }
            return View();
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _UnitOfWork.CategoryRepository.Get(u => u.Id == id);
            /*Category? categoryFromDb1 = _db.Categories.FirstOrDefault(u=>u.Id==id);
            Category? categoryFromDb2 = _db.Categories.Where(u=>u.Id==id).FirstOrDefault();*/
            if (categoryFromDb == null)

            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "name and order cannot be same");
            }
            if (ModelState.IsValid)
            {
                _UnitOfWork.CategoryRepository.Update(obj);
                _UnitOfWork.Save();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index", "Category");

            }
            return View();
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _UnitOfWork.CategoryRepository.Get(u => u.Id == id);
            /*Category? categoryFromDb1 = _db.Categories.FirstOrDefault(u=>u.Id==id);
            Category? categoryFromDb2 = _db.Categories.Where(u=>u.Id==id).FirstOrDefault();*/
            if (categoryFromDb == null)

            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Category obj = _UnitOfWork.CategoryRepository.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _UnitOfWork.CategoryRepository.Remove(obj);
            _UnitOfWork.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index", "Category");

        }
    }
}
