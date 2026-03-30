using BulkyWeb.Data;
using BulkyWeb.Models;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitofWork _unitofWork;
        //public CategoryController(ICategoryRepository db) 
        //{
        //    _categoryRepository = db;
        //}
        public CategoryController(IUnitofWork unitofWork) 
        {
            _unitofWork = unitofWork;
        }

        public IActionResult Index()
        {
            List<Category> objCategoryList = _unitofWork.Category.GetAll().ToList();
            return View(objCategoryList);
        }

        public IActionResult Create() 
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj) 
        {
            _unitofWork.Category.Add(obj);
            _unitofWork.Save();
            TempData["success"] = "Category created successfully";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id) 
        {
            if (id == null || id == 0) 
            {
                return NotFound();
            }
            Category? categoryobj = _unitofWork.Category.Get(u=>u.Id == id);
            if (categoryobj == null) 
            {
                return NotFound();
            }
            return View(categoryobj);
        }


        [HttpPost]
        public IActionResult Edit(Category obj) 
        {
            if (ModelState.IsValid) 
            {
                _unitofWork.Category.Update(obj);
                _unitofWork.Save();
                TempData["success"] = "Category updated successfully";
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
            Category? categoryobj = _unitofWork.Category.Get(u => u.Id == id);
            if (categoryobj == null)
            {
                return NotFound();
            }
            return View(categoryobj);
        }


        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Category? categoryobj = _unitofWork.Category.Get(u => u.Id == id);
            if (categoryobj == null) 
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _unitofWork.Category.Remove(categoryobj);
                _unitofWork.Save();
                TempData["success"] = "Category deleted successfully";
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
