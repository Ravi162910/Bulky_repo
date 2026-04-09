using BulkyWeb.Data;
using BulkyWeb.Models;
using BulkyWeb.Repository.IRepository;
using DataAccess.Repository.IRepository;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _ProductRepository;
        private readonly IUnitofWork _unitofWork;
        //public ProductController(IProductRepository db) 
        //{
        //    _ProductRepository = db;
        //}
        public ProductController(IUnitofWork unitofWork) 
        {
            _unitofWork = unitofWork;
        }

        public IActionResult Index()
        {
            List<Product> objProductList = _unitofWork.Product.GetAll().ToList();
            return View(objProductList);
        }

        public IActionResult Create() 
        {
            IEnumerable<SelectListItem> CategoryList = _unitofWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()

            });
            ViewBag.CategoryList = CategoryList;
            return View();
        }
        [HttpPost]
        public IActionResult Create(Product obj) 
        {
            _unitofWork.Product.Add(obj);
            _unitofWork.Save();
            TempData["success"] = "Product created successfully";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id) 
        {
            if (id == null || id == 0) 
            {
                return NotFound();
            }
            Product? Productobj = _unitofWork.Product.Get(u=>u.Id == id);
            if (Productobj == null) 
            {
                return NotFound();
            }
            return View(Productobj);
        }


        [HttpPost]
        public IActionResult Edit(Product obj) 
        {
            if (ModelState.IsValid) 
            {
                _unitofWork.Product.Update(obj);
                _unitofWork.Save();
                TempData["success"] = "Product updated successfully";
                return RedirectToAction("Index");
            }
            return View();
        }


        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? Productobj = _unitofWork.Product.Get(u => u.Id == id);
            if (Productobj == null)
            {
                return NotFound();
            }
            return View(Productobj);
        }


        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Product? Productobj = _unitofWork.Product.Get(u => u.Id == id);
            if (Productobj == null) 
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _unitofWork.Product.Remove(Productobj);
                _unitofWork.Save();
                TempData["success"] = "Product deleted successfully";
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
