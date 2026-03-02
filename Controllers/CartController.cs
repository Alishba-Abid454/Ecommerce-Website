
using EcommerceFullstackDesign.Models.Interface;
using EcommerceFullstackDesign.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceFullstackDesign.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepository;
        private readonly IHomeRepository _homeRepository;
        private readonly ILogger<CartController> _logger;

        public CartController(
            ICartRepository cartRepository,
            IHomeRepository homeRepository,
            ILogger<CartController> logger)
        {
            _cartRepository = cartRepository;
            _homeRepository = homeRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            int userId = GetUserId();

            var cartSummary = await _cartRepository.GetCartSummaryAsync(userId);
            var recommendedProducts = await _homeRepository.GetRecommendedItemsAsync(4);

            var viewModel = new CartPageViewModel
            {
                CartSummary = cartSummary,
                RecommendedProducts = recommendedProducts
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            try
            {
                int userId = GetUserId();
                var items = await _cartRepository.GetCartItemsAsync(userId);
                return Json(new { count = items.Sum(x => x.Quantity) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart count");
                return Json(new { count = 0 });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            try
            {
                int userId = GetUserId();

                _logger.LogInformation($"Adding to cart: User={userId}, Product={productId}, Quantity={quantity}");

                await _cartRepository.AddToCartAsync(userId, productId, quantity);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }

                TempData["SuccessMessage"] = "Product added to cart!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding to cart");

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Error adding to cart" });
                }

                TempData["ErrorMessage"] = "Error adding to cart";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQuantityRequest request)
        {
            try
            {
                await _cartRepository.UpdateQuantityAsync(request.CartItemId, request.Quantity);

                int userId = GetUserId();
                var summary = await _cartRepository.GetCartSummaryAsync(userId);

                return Json(new
                {
                    success = true,
                    subtotal = summary.Subtotal,
                    total = summary.Total,
                    discount = summary.Discount,
                    tax = summary.Tax,
                    itemCount = summary.ItemCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating quantity");
                return Json(new { success = false, message = "Error updating quantity" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            try
            {
                _logger.LogInformation($"Removing cart item {id}");

                // Database se item delete karo
                await _cartRepository.RemoveItemAsync(id);

                // Updated cart summary return karo
                int userId = GetUserId();
                var summary = await _cartRepository.GetCartSummaryAsync(userId);

                return Json(new
                {
                    success = true,
                    subtotal = summary.Subtotal,
                    total = summary.Total,
                    discount = summary.Discount,
                    tax = summary.Tax,
                    itemCount = summary.ItemCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item");
                return Json(new { success = false, message = "Error removing item: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                int userId = GetUserId();
                _logger.LogInformation($"Clearing cart for user {userId}");
                await _cartRepository.ClearCartAsync(userId);

                return Json(new
                {
                    success = true,
                    subtotal = 0,
                    total = 0,
                    discount = 0,
                    tax = 0,
                    itemCount = 0
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart");
                return Json(new { success = false, message = "Error clearing cart" });
            }
        }

        public async Task<IActionResult> Checkout()
        {
            int userId = GetUserId();
            var cartSummary = await _cartRepository.GetCartSummaryAsync(userId);

            if (!cartSummary.Items.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty";
                return RedirectToAction("Index");
            }

            return View(cartSummary);
        }

        [HttpPost]
        public async Task<IActionResult> OrdersSuccess(
    string FullName,
    string FullAddress,
    string City,
    string State,
    string Pincode,
    string PhoneNumber,
    string Email,
    string PaymentMethod)
        {
            try
            {
                int userId = GetUserId();
                var cartSummary = await _cartRepository.GetCartSummaryAsync(userId);

                if (!cartSummary.Items.Any())
                {
                    TempData["ErrorMessage"] = "Your cart is empty";
                    return RedirectToAction("Index");
                }

                // Create order in database
                int orderNumber = await _cartRepository.CreateOrderAsync(userId);

                // Save shipping information (you can create a separate table for this)
                _logger.LogInformation($"Order {orderNumber} created for user {userId}");
                _logger.LogInformation($"Shipping: {FullName}, {FullAddress}, {City}, {State}, {Pincode}, {PhoneNumber}, {Email}, Payment: {PaymentMethod}");

                // Store order details in TempData or ViewBag
                ViewBag.OrderNumber = orderNumber;
                ViewBag.OrderTotal = cartSummary.Total;
                ViewBag.FullName = FullName;
                ViewBag.ShippingAddress = $"{FullAddress}, {City}, {State} - {Pincode}";
                ViewBag.PaymentMethod = PaymentMethod;

                return View("OrdersSuccess");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming order");
                TempData["ErrorMessage"] = "Error processing order";
                return RedirectToAction("Index");
            }
        }

        /*[HttpPost]
        public async Task<IActionResult> OrdersSuccess()
        {
            try
            {
                int userId = GetUserId();
                var cartSummary = await _cartRepository.GetCartSummaryAsync(userId);

                if (!cartSummary.Items.Any())
                {
                    TempData["ErrorMessage"] = "Your cart is empty";
                    return RedirectToAction("Index");
                }

                int orderNumber = await _cartRepository.CreateOrderAsync(userId);
                _logger.LogInformation($"Order {orderNumber} created for user {userId}");

                ViewBag.OrderNumber = orderNumber;
                return View("OrderSuccess");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming order");
                TempData["ErrorMessage"] = "Error processing order";
                return RedirectToAction("Index");
            }
        }*/

        private int GetUserId()
        {
            // Get from session or create new
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                userId = new Random().Next(1000, 9999);
                HttpContext.Session.SetInt32("UserId", userId.Value);

                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(30),
                    HttpOnly = true,
                    IsEssential = true
                };
                Response.Cookies.Append("UserId", userId.Value.ToString(), cookieOptions);
            }
            return userId.Value;
        }
    }

    public class UpdateQuantityRequest
    {
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
    }
}








/*using EcommerceFullstackDesign.Models.Interface;
using EcommerceFullstackDesign.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceFullstackDesign.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepository;
        private readonly IHomeRepository _homeRepository;
        private readonly ILogger<CartController> _l;

        public CartController(ICartRepository cartRepository, IHomeRepository homeRepository, ILogger<CartController> logger)
        {
            _cartRepository = cartRepository;
            _homeRepository = homeRepository;
            _l = logger;
        }
*//*        public CartController(ICartRepository cartRepository, IHomeRepository homeRepository)
        {
            _cartRepository = cartRepository;
            _homeRepository = homeRepository;
        }*//*

        public async Task<IActionResult> Index()
        {
            int userId = GetUserId();

            var cartSummary = await _cartRepository.GetCartSummaryAsync(userId);
            var recommendedProducts = await _homeRepository.GetRecommendedItemsAsync(4);

            var viewModel = new CartPageViewModel
            {
                CartSummary = cartSummary,
                RecommendedProducts = recommendedProducts
            };

            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            try
            {
                int userId = GetUserId();
                var items = await _cartRepository.GetCartItemsAsync(userId);
                return Json(new { count = items.Sum(x => x.Quantity) });
            }
            catch (Exception ex)
            {
                _l.LogError(ex, "Error getting cart count");
                return Json(new { count = 0 });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            try
            {
                int userId = GetUserId();

                _logger.LogInformation($"Adding to cart: User={userId}, Product={productId}, Quantity={quantity}");

                await _cartRepository.AddToCartAsync(userId, productId, quantity);

                TempData["SuccessMessage"] = "Product added to cart successfully!";

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding to cart");

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Error adding to cart" });
                }

                TempData["ErrorMessage"] = "Error adding to cart";
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQuantityRequest request)
        {
            try
            {
                await _cartRepository.UpdateQuantityAsync(request.CartItemId, request.Quantity);
                var summary = await _cartRepository.GetCartSummaryAsync(GetUserId());

                return Json(new
                {
                    success = true,
                    subtotal = summary.Subtotal,
                    total = summary.Total,
                    discount = summary.Discount,
                    tax = summary.Tax,
                    itemCount = summary.ItemCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating quantity");
                return Json(new { success = false });
            }
        }

        public class UpdateQuantityRequest
        {
            public int CartItemId { get; set; }
            public int Quantity { get; set; }
        }


*//*        [HttpPost]
        public async Task<IActionResult> Remove([FromBody] int id)
        {
           *//* try
            {*//*
                _logger.LogInformation($"Removing cart item {id}");

                await _cartRepository.RemoveItemAsync(id);

                int userId = GetUserId();
                var summary = await _cartRepository.GetCartSummaryAsync(userId);

                return Json(new
                {
                    success = true,
                    subtotal = summary.Subtotal,
                    total = summary.Total,
                    discount = summary.Discount,
                    tax = summary.Tax,
                    itemCount = summary.Items.Sum(x => x.Quantity)
                });
            *//*}*/
/*            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item");
                return Json(new { success = false });
            }*//*
        }*//*
        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                int userId = GetUserId();
                _logger.LogInformation($"Clearing cart for user {userId}");
                await _cartRepository.ClearCartAsync(userId);

                return Json(new
                {
                    success = true,
                    subtotal = 0,
                    total = 0,
                    discount = 0,
                    tax = 0,
                    itemCount = 0
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart");
                return Json(new { success = false, message = "Error clearing cart" });
            }
        }

        public async Task<IActionResult> Checkout()
        {
            int userId = GetUserId();
            var cartSummary = await _cartRepository.GetCartSummaryAsync(userId);

            if (!cartSummary.Items.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty";
                return RedirectToAction("Index");
            }

            return View(cartSummary);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmOrder()
        {
            try
            {
                int userId = GetUserId();
                var cartSummary = await _cartRepository.GetCartSummaryAsync(userId);

                if (!cartSummary.Items.Any())
                {
                    TempData["ErrorMessage"] = "Your cart is empty";
                    return RedirectToAction("Index");
                }

                int orderNumber = await _cartRepository.CreateOrderAsync(userId);

                _logger.LogInformation($"Order {orderNumber} created for user {userId}");

                ViewBag.OrderNumber = orderNumber;
                return View("OrderSuccess");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming order");
                TempData["ErrorMessage"] = "Error processing order";
                return RedirectToAction("Index");
            }
        }

        private int GetUserId()
        {
            // Get from session or create new
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                userId = new Random().Next(1000, 9999);
                HttpContext.Session.SetInt32("UserId", userId.Value);

                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(30),
                    HttpOnly = true,
                    IsEssential = true
                };
                Response.Cookies.Append("UserId", userId.Value.ToString(), cookieOptions);
            }
            return userId.Value;
        }

        private readonly ILogger<CartController> _logger;
    }

    public class UpdateQuantityRequest
    {
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
    }

}



            */