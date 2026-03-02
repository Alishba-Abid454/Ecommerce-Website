using EcommerceFullstackDesign.Models;
using EcommerceFullstackDesign.Models.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceFullstackDesign.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductsRepository _productRepository;
        private readonly IHomeRepository _homeRepository;

        public ProductsController(IProductsRepository productRepository, IHomeRepository homeRepository)
        {
            _productRepository = productRepository;
            _homeRepository = homeRepository;
        }
        public async Task<IActionResult> Index(int? categoryId, string? category)
        {
            try
            {
                // Get categories for sidebar
                var categories = await _homeRepository.GetCategoriesAsync();
                ViewBag.Categories = categories;

                // Get category name
                if (categoryId.HasValue)
                {
                    var selectedCategory = categories.FirstOrDefault(c => c.Id == categoryId);
                    ViewBag.CategoryName = selectedCategory?.Name ?? "Products";
                    ViewBag.SelectedCategoryId = categoryId;

                    // Get brands for filter - FIXED: Pass the categoryId
                    var brands = await _productRepository.GetBrandsAsync(categoryId);
                    ViewBag.Brands = brands ?? new List<string>();

                    // Get features - FIXED: Pass the categoryId
                    var features = await _productRepository.GetFeaturesAsync(categoryId);
                    ViewBag.Features = features ?? new List<string>();

                    // Get price range - FIXED: Pass the categoryId
                    var priceRange = await _productRepository.GetPriceRangeAsync(categoryId);
                    ViewBag.MinPrice = priceRange.Min;
                    ViewBag.MaxPrice = priceRange.Max;

                    // Get breadcrumb
                    var breadcrumb = await _productRepository.GetCategoryBreadcrumbAsync(categoryId.Value);
                    ViewBag.Breadcrumb = breadcrumb;
                }
                else
                {
                    ViewBag.CategoryName = "All Products";
                    ViewBag.Brands = new List<string>();
                    ViewBag.Features = new List<string>();
                    ViewBag.MinPrice = 0;
                    ViewBag.MaxPrice = 1000;
                    ViewBag.Breadcrumb = new List<BreadcrumbItem>
            {
                new BreadcrumbItem { Title = "Home", Url = "/" },
                new BreadcrumbItem { Title = "Products", IsActive = true }
            };
                }

                return View();
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DebugFilters(int? categoryId)
        {
            try
            {
                var brands = await _productRepository.GetBrandsAsync(categoryId);
                var features = await _productRepository.GetFeaturesAsync(categoryId);
                var priceRange = await _productRepository.GetPriceRangeAsync(categoryId);

                return Json(new
                {
                    success = true,
                    categoryId = categoryId,
                    brands = brands,
                    brandsCount = brands.Count,
                    features = features,
                    featuresCount = features.Count,
                    minPrice = priceRange.Min,
                    maxPrice = priceRange.Max
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // AJAX endpoint to get filtered products
        [HttpPost]
        public async Task<IActionResult> GetFilteredProducts([FromBody] ProductFilterViewModel filter)
        {
            try
            {
                var result = await _productRepository.GetFilteredProductsAsync(filter);
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var product = await _productRepository.GetProductByIdAsync(id);

                if (product == null)
                {
                    return NotFound();
                }

                // Get related products
                var relatedProducts = await _productRepository.GetRelatedProductsAsync(id, product.CategoryId, 6);
                ViewBag.RelatedProducts = relatedProducts;

                // Get breadcrumb
                var breadcrumb = await _productRepository.GetCategoryBreadcrumbAsync(product.CategoryId);
                breadcrumb.Add(new BreadcrumbItem { Title = product.Name, IsActive = true });
                ViewBag.Breadcrumb = breadcrumb;

                // Mock data for reviews (if not in database)
                ViewBag.Rating = product.Rating;
                ViewBag.ReviewCount = product.ReviewCount;
                ViewBag.SoldCount = product.OrderCount;

                return View(product); // This returns ProductViewModel
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message}");
            }
        }

        // AJAX endpoint for search
        [HttpGet]
        public async Task<IActionResult> Search(string term, int page = 1)
        {
            try
            {
                var result = await _productRepository.SearchProductsAsync(term, page, 10);
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // AJAX endpoint for quick view
        [HttpGet]
        public async Task<IActionResult> QuickView(int id)
        {
            try
            {
                var product = await _productRepository.GetProductByIdAsync(id);
                if (product == null)
                {
                    return Json(new { success = false, message = "Product not found" });
                }

                return PartialView("_ProductQuickView", product);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}


