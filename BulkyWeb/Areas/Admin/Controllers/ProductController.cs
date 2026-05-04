using BulkyWeb.Data;
using BulkyWeb.Models;
using BulkyWeb.Repository.IRepository;
using DataAccess.Repository.IRepository;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.Models.ViewModels;
namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _ProductRepository;
        private readonly IUnitofWork _unitofWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        //public ProductController(IProductRepository db) 
        //{
        //    _ProductRepository = db;
        //}
        public ProductController(IUnitofWork unitofWork, IWebHostEnvironment webHostEnvironment) 
        {
            _unitofWork = unitofWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> objProductList = _unitofWork.Product.GetAll(includeproperties: "Category").ToList();
            return View(objProductList);
        }

        public IActionResult Upsert(int? ID) 
        {

            ProductVM productVM = new()
            {
                CategoryList = _unitofWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };

            if (ID == null || ID == 0)
            {
                return View(productVM);
            }
            else 
            {
                productVM.Product = _unitofWork.Product.Get(u => u.Id == ID);
                return View(productVM);
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file) 
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null) 
                {
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productpath = Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        var oldimgpath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.Trim('\\'));

                        if (System.IO.File.Exists(oldimgpath)) 
                        {
                            System.IO.File.Delete(oldimgpath);
                        }
                    }
                    
                    using (var fileStream = new FileStream(Path.Combine(productpath, filename), FileMode.Create)) 
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\images\product\" + filename;
                }
                if (productVM.Product.Id == 0)
                {
                    _unitofWork.Product.Add(productVM.Product);
                }
                else 
                {
                    _unitofWork.Product.Update(productVM.Product);
                }

                _unitofWork.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            else 
            {
                productVM.CategoryList = _unitofWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
            }
            return View(productVM);

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




        //API Call
        [HttpGet]
        public IActionResult GetAll(int id) 
        {
            List<Product> objProductList = _unitofWork.Product.GetAll(includeproperties: "Category").ToList();
            return Json(new { data = objProductList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id) 
        {
            var product = _unitofWork.Product.Get(products => products.Id == id);
            if (product == null) 
            {
                return Json(new { success = false, message = "error while deleting" });
            }

            var oldimagepath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldimagepath)) 
            {
                System.IO.File.Delete(oldimagepath);
            }

            _unitofWork.Product.Remove(product);

            _unitofWork.Save();

            return Json(new { success = true, message = "Delete successful" });
        }
    }
}
