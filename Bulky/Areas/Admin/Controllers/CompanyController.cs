using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Bulky.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.CompilerServices;

namespace Bulky.Areas.Admin.Controllers
{
    //[Authorize(Roles = SD.Role_Admin)]
    [Area("Admin")]
    public class CompanyController : Controller
    {

        private IUnitOfWork _UnitOfWork;
        
        public CompanyController(IUnitOfWork UnitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _UnitOfWork = UnitOfWork;
            
        }
        public IActionResult Index()
        {
            List<Company> objCompanyList = _UnitOfWork.CompanyRepository.GetAll().ToList();
            
            return View(objCompanyList);
        }

        public IActionResult Upsert(int ?id)  //update+insert
        {
            
            
            if (id == null || id == 0)
            {
                //create
                return View(new Company());
            }
            else
            {
                //update
               Company companyobj=_UnitOfWork.CompanyRepository.Get(u=>u.Id==id);
                return View(companyobj);
            }
        }

        [HttpPost]
        public IActionResult Upsert(Company obj)
        {
            
            if (ModelState.IsValid)
            {
                
                if (obj.Id == 0)
                {
                    _UnitOfWork.CompanyRepository.Add(obj);
                    TempData["success"] = "Company created successfully";
                }
                else
                {
                    _UnitOfWork.CompanyRepository.Update(obj);
                    TempData["success"] = "Company updated successfully";
                }
                
                _UnitOfWork.Save();
                
                return RedirectToAction("Index", "Company");

            }
            
            
            return View(obj);
        }
        
        /*public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Company? CompanyFromDb = _UnitOfWork.CompanyRepository.Get(u => u.Id == id);
            *//*Company? CompanyFromDb1 = _db.Categories.FirstOrDefault(u=>u.Id==id);
            Company? CompanyFromDb2 = _db.Categories.Where(u=>u.Id==id).FirstOrDefault();*//*
            if (CompanyFromDb == null)

            {
                return NotFound();
            }
            return View(CompanyFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Company obj = _UnitOfWork.CompanyRepository.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _UnitOfWork.CompanyRepository.Remove(obj);
            _UnitOfWork.Save();
            TempData["success"] = "Company deleted successfully";
            return RedirectToAction("Index", "Company");

        }*/
        #region APi_CALLS
        public IActionResult GetAll()
        {
            List<Company> objCompanyList = _UnitOfWork.CompanyRepository.GetAll().ToList();
            return Json(objCompanyList);
        }
        [HttpDelete]
        public IActionResult Delete(int ?id)
        {
            var CompanyToBeDeleted = _UnitOfWork.CompanyRepository.Get(u => u.Id == id);
            if (CompanyToBeDeleted == null) 
            {
                return Json(new { success = false, message = "Error While deleting" });
            }
            
            _UnitOfWork.CompanyRepository.Remove(CompanyToBeDeleted);
            _UnitOfWork.Save();
            return Json(new { success = true, message = "Deleted Successfully" });
        }
        #endregion
    }
}
