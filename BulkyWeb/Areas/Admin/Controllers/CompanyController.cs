using BulkyWeb.Data;
using BulkyWeb.Models;
using BulkyWeb.Repository.IRepository;
using DataAccess.Repository.IRepository;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Utilities;
namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitofWork _unitofWork;

        public CompanyController(IUnitofWork unitofWork, IWebHostEnvironment webHostEnvironment) 
        {
            _unitofWork = unitofWork;
        }

        public IActionResult Index()
        {
            List<Company> objCompanyList = _unitofWork.Company.GetAll().ToList();
            return View(objCompanyList);
        }

        public IActionResult Upsert(int? ID) 
        {
            if (ID == null || ID == 0)
            {
                return View(new Company());
            }
            else 
            {
                Company company = _unitofWork.Company.Get(u => u.Id == ID);
                return View(company);
            }
        }
        [HttpPost]
        public IActionResult Upsert(Company companyobj, IFormFile? file) 
        {
            if (ModelState.IsValid)
            {
                if (companyobj.Id == 0)
                {
                    _unitofWork.Company.Add(companyobj);
                }
                else 
                {
                    _unitofWork.Company.Update(companyobj);
                }

                _unitofWork.Save();
                TempData["success"] = "Company created successfully";
                return RedirectToAction("Index");
            }
            else 
            {
                return View(companyobj);
            }
        }


        [HttpPost]
        public IActionResult Edit(Company obj) 
        {
            if (ModelState.IsValid) 
            {
                _unitofWork.Company.Update(obj);
                _unitofWork.Save();
                TempData["success"] = "Company updated successfully";
                return RedirectToAction("Index");
            }
            return View();
        }




        //API Call
        [HttpGet]
        public IActionResult GetAll(int id) 
        {
            List<Company> objCompanyList = _unitofWork.Company.GetAll().ToList();
            return Json(new { data = objCompanyList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id) 
        {
            var company = _unitofWork.Company.Get(companys => companys.Id == id);
            if (company == null) 
            {
                return Json(new { success = false, message = "error while deleting" });
            }

            _unitofWork.Company.Remove(company);

            _unitofWork.Save();

            return Json(new { success = true, message = "Delete successful" });
        }
    }
}
