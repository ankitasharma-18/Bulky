using BulkyWeb.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CategoryController(ApplicationDbContext db)
        {
            _context = db;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _context.Categories.ToList();
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
                ModelState.AddModelError("Name", "The DisplayOrder Cannot  match the Name.");
            }
            if (ModelState.IsValid)
            {
				_context.Categories.Add(obj);
				_context.SaveChanges();
				return RedirectToAction("Index");
			}          
			return View();

		}
		public IActionResult Edit(int? id)
		{
			if(id==null || id == 0)
			{
				return NotFound();
			}
			Category? categoryFromDb = _context.Categories.FirstOrDefault(u => u.Id == id);
			if(categoryFromDb == null)
			{
				return NotFound();
			}
			return View(categoryFromDb);

		}
		[HttpPost]
		public IActionResult Edit(Category obj)
		{
			
			if (ModelState.IsValid)
			{
				_context.Categories.Update(obj);
				_context.SaveChanges();
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
			Category? categoryFromDb = _context.Categories.FirstOrDefault(u => u.Id == id);
			if (categoryFromDb == null)
			{
				return NotFound();
			}
			return View(categoryFromDb);

		}
		[HttpPost,ActionName("Delete")]
		public IActionResult DeletePost(int? id)
		{
			Category obj = _context.Categories.Find(id);
			if(obj== null)
			{
				return NotFound();
			}
	
				_context.Categories.Remove(obj);
				_context.SaveChanges();
				return RedirectToAction("Index");

		}
	}
}
