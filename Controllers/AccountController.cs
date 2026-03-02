using EcommerceFullstackDesign.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace EcommerceFullstackDesign.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Yahan aap database mein user save kar sakte hain
                    // For now, simple demo hai

                    // Auto login after registration
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.Username),
                        new Claim(ClaimTypes.Email, model.Email),
                        new Claim("UserId", Guid.NewGuid().ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    // Set session for greeting
                    HttpContext.Session.SetString("UserGreeting", $"Hi, {model.Username}");
                    HttpContext.Session.SetString("UserSubGreeting", "Welcome to our store");
                    HttpContext.Session.SetString("UserAvatar", "/Image/profile.png");

                    TempData["SuccessMessage"] = "Registration successful!";
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Registration error");
                    ModelState.AddModelError("", "Registration failed. Please try again.");
                }
            }
            return View(model);
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Yahan aap database se user verify kar sakte hain
                    // Demo ke liye simple check

                    if (model.Username == "demo" && model.Password == "password")
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, model.Username),
                            new Claim(ClaimTypes.Email, "user@example.com"),
                            new Claim("UserId", "12345")
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                        };

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);

                        // Set session for greeting
                        HttpContext.Session.SetString("UserGreeting", $"Hi, {model.Username}");
                        HttpContext.Session.SetString("UserSubGreeting", "Welcome back!");
                        HttpContext.Session.SetString("UserAvatar", "/Image/profile.png");

                        TempData["SuccessMessage"] = "Login successful!";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid username or password");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Login error");
                    ModelState.AddModelError("", "Login failed. Please try again.");
                }
            }
            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();

            TempData["SuccessMessage"] = "You have been logged out.";
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Profile
        [HttpGet]
        public IActionResult Profile()
        {
            return View();
        }
    }
}