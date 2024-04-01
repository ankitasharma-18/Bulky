
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _webhostEnviornment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webhostEnviornment)
        {
            _unitOfWork = unitOfWork;
            _webhostEnviornment = webhostEnviornment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            return View(objProductList);
        }
        public IActionResult Upsert(int ?id)
        {
          
            ProductVM productvm = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString(),
				}),
                Product = new Product()
             };
            if(id ==null || id == 0)
            {
                //create
				return View(productvm);
			}
            else
            {
                //update
                productvm.Product=_unitOfWork.Product.Get(u=>u.Id==id);
                return View(productvm);
            }

        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productvm, IFormFile? file)
        {
          
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webhostEnviornment.WebRootPath;
                if(file != null)
                {
                    string fileName=Guid.NewGuid().ToString()+Path.GetExtension(file.FileName);
                    string productPath=Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(productvm.Product.ImageUrl))
                    {
                        //delete the old image
                        var oldImagePath= Path.Combine(wwwRootPath, productvm.Product.ImageUrl.TrimStart('\\'));
					    if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);    
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName),FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productvm.Product.ImageUrl = @"\images\product\" + fileName;

				}

                if (productvm.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productvm.Product);
                }
                else
                {
					_unitOfWork.Product.Update(productvm.Product);
				}

                _unitOfWork.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            else
			{
				productvm.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                });
              
				return View(productvm);
			}     
        }
       
    
       

        #region APICALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }

        [HttpDelete]
		public IActionResult Delete(int? id)
		{
            var productToBeDeleted= _unitOfWork.Product.Get(u=>u.Id == id);

            if (productToBeDeleted == null)
            {
                return Json(new {success = false,message="Error while deleting"});
            }

			var oldImagePath = Path.Combine(_webhostEnviornment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));
			if (System.IO.File.Exists(oldImagePath))
			{
				System.IO.File.Delete(oldImagePath);
			}

            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

			return Json(new { success = true, message = "Delete successful" });

		}

		#endregion
	}
}

