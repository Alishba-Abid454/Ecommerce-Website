
using EcommerceFullstackDesign.Models;
using EcommerceFullstackDesign.Models.Interface;
using EcommerceFullstackDesign.Models.Repository;
using EcommerceFullstackDesign.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace EcommerceFullstackDesign.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeRepository _homeRepository;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IHomeRepository homeRepository, ILogger<HomeController> logger)
        {
            _homeRepository = homeRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                int? userId = GetUserId();
                var data = await _homeRepository.GetHomePageDataAsync(userId);

                // Log the data for debugging
                _logger.LogInformation("Categories count: {Count}", data.Categories?.Count ?? 0);
                _logger.LogInformation("Recommended items count: {Count}", data.RecommendedItems?.Count ?? 0);
                _logger.LogInformation("Deals and offers count: {Count}", data.DealsAndOffers?.Count ?? 0);
                _logger.LogInformation("HeroBanner exists: {Exists}", data.HeroBanner != null);
                _logger.LogInformation("PromotionCards count: {Count}", data.PromotionCards?.Count ?? 0);
                _logger.LogInformation("Services count: {Count}", data.Services?.Count ?? 0);
                _logger.LogInformation("Suppliers count: {Count}", data.Suppliers?.Count ?? 0);

                // Store cart count in ViewBag for layout
                ViewBag.CartCount = data.CartCount;

                return View(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home page");

                // Return view with empty model instead of error message
                var emptyModel = new HomeViewModel();
                return View(emptyModel);
            }
        }
        public async Task<IActionResult> ProductDetails(int id)
        {
            try
            {
                var product = await _homeRepository.GetProductByIdAsync(id);

                if (product == null)
                {
                    return NotFound();
                }

                // Store last viewed product in cookie
                var lastViewed = GetLastViewedProducts();
                if (!lastViewed.Contains(id))
                {
                    lastViewed.Insert(0, id);
                    if (lastViewed.Count > 5)
                        lastViewed.RemoveAt(5);

                    SaveLastViewedProducts(lastViewed);
                }

                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading product details for id: {ProductId}", id);
                return Content($"Error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            try
            {
                int userId = GetOrCreateUserId();

                var newCount = await _homeRepository.AddToCartAsync(userId, request.ProductId, request.Quantity);

                // Update session and cookie
                HttpContext.Session.SetInt32("CartCount", newCount);
                Response.Cookies.Append("CartCount", newCount.ToString(), new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(30),
                    HttpOnly = true,
                    IsEssential = true
                });

                return Json(new { success = true, count = newCount });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding to cart");
                return Json(new { success = false, message = "Error adding to cart" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _homeRepository.GetCategoriesAsync();
                return Json(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting categories");
                return Json(new { success = false, message = "Error loading categories" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRecommendedItems()
        {
            try
            {
                var items = await _homeRepository.GetRecommendedItemsAsync();
                return Json(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recommended items");
                return Json(new { success = false, message = "Error loading items" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserCartCount()
        {
            try
            {
                int userId = GetUserId() ?? 0;
                if (userId == 0)
                    return Json(new { success = true, count = 0 });

                var count = await _homeRepository.GetUserCartCountAsync(userId);

                // Update session
                HttpContext.Session.SetInt32("CartCount", count);

                return Json(new { success = true, count = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart count");
                return Json(new { success = false, message = "Error loading cart count" });
            }
        }

        private int? GetUserId()
        {
            // Check session first
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                // Check cookie
                if (Request.Cookies.ContainsKey("UserId") && int.TryParse(Request.Cookies["UserId"], out int cookieUserId))
                {
                    userId = cookieUserId;
                    HttpContext.Session.SetInt32("UserId", userId.Value);
                }
            }

            return userId;
        }

        private int GetOrCreateUserId()
        {
            var userId = GetUserId();

            if (userId == null)
            {
                // Create new temporary user ID (in production, you'd create a real user)
                userId = new Random().Next(1000, 9999);

                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(30),
                    HttpOnly = true,
                    IsEssential = true
                };

                Response.Cookies.Append("UserId", userId.Value.ToString(), cookieOptions);
                HttpContext.Session.SetInt32("UserId", userId.Value);
            }

            return userId.Value;
        }

        private List<int> GetLastViewedProducts()
        {
            if (Request.Cookies.ContainsKey("LastViewedProducts"))
            {
                try
                {
                    var cookieValue = Request.Cookies["LastViewedProducts"];
                    return JsonSerializer.Deserialize<List<int>>(cookieValue) ?? new List<int>();
                }
                catch
                {
                    return new List<int>();
                }
            }
            return new List<int>();
        }

        private void SaveLastViewedProducts(List<int> products)
        {
            var cookieValue = JsonSerializer.Serialize(products);
            Response.Cookies.Append("LastViewedProducts", cookieValue, new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7),
                HttpOnly = true,
                IsEssential = true
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
        [HttpPost]
        public async Task<IActionResult> SendInquiry(Inquiry inquiry)
        {
            try
            {
                inquiry.UserId = GetUserId();
                var result = await _homeRepository.SubmitInquiryAsync(inquiry);

                if (result)
                {
                    TempData["SuccessMessage"] = "Inquiry sent successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to send inquiry. Please try again.";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending inquiry");
                TempData["ErrorMessage"] = "An error occurred. Please try again.";
                return RedirectToAction("Index");
            }
        } 
    }
    public class AddToCartRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}


