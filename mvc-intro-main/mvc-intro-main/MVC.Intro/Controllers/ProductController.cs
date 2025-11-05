using Microsoft.AspNetCore.Mvc;
using MVC.Intro.Models;
using MVC.Intro.Services;

namespace MVC.Intro.Controllers
{
    [Route("[controller]/[action]")]
    public class ProductController : Controller
    {
        ProductService _productService;
        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        public IActionResult Index()
        {
            return View(_productService.GetAllProducts());
        }
        [HttpGet("{id}")]//https://localhost:7206/Product/Details/52e9ba06-3b56-4f3d-973c-388efb0e4417
        public IActionResult Details(Guid id)
        {
            return View(_productService.GetProductById(id));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Delete(Guid id)
        {
            _productService.DeleteProduct(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult CreateProduct(Product product, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", product);
            }
            if (imageFile != null && imageFile.Length > 0)
            {
                using var ms = new MemoryStream();
                imageFile.CopyTo(ms);
                product.ImageData = ms.ToArray();
                product.ImageContentType = imageFile.ContentType;
            }
            _productService.AddProduct(product);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("{id}")]
        public IActionResult Image(Guid id)
        {
            var product = _productService.GetProductById(id);
            if (product.ImageData == null || string.IsNullOrEmpty(product.ImageContentType))
            {
                return NotFound();
            }
            return File(product.ImageData, product.ImageContentType);
        }
    }
}
