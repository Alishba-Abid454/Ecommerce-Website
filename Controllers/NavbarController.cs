using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using EcommerceFullstackDesign.Models;

namespace EcommerceFullstackDesign.Controllers
{
    public class NavbarController : Controller
    {
        private readonly ILogger<NavbarController> _logger;

        public NavbarController(ILogger<NavbarController> logger)
        {
            _logger = logger;
        }

        // ========== HOT OFFERS ACTION ==========
        public IActionResult HotOffers()
        {
            try
            {
                // Mock data for hot offers
                var hotOffers = new List<NavbarProductViewModel>
                {
                    new NavbarProductViewModel
                    {
                        Id = 33,
                        Name = "Wireless Headphones",
                        Description = "Premium noise-cancelling headphones",
                        Price = 99.99m,
                        DiscountPrice = 79.99m,
                        DiscountPercent = 20,
                        ImageUrl = "/Image/headphones.png",
                        Category = "Electronics"
                    },
                    new NavbarProductViewModel
                    {
                        Id = 1,
                        Name = "Smart Watch",
                        Description = "Fitness tracker with heart rate monitor",
                        Price = 199.99m,
                        DiscountPrice = 149.99m,
                        DiscountPercent = 25,
                        ImageUrl = "/Image/smartwatchwithHB.png",
                        Category = "Electronics"
                    },
                    new NavbarProductViewModel
                    {
                        Id = 4,
                        Name = "Gaming Laptop",
                        Description = "High-performance gaming laptop",
                        Price = 1299.99m,
                        DiscountPrice = 1099.99m,
                        DiscountPercent = 15,
                        ImageUrl = "/Image/laptops.png",
                        Category = "Electronics"
                    },
                    new NavbarProductViewModel
                    {
                        Id = 34,
                        Name = "Bluetooth Speaker",
                        Description = "Portable waterproof speaker",
                        Price = 79.99m,
                        DiscountPrice = 59.99m,
                        DiscountPercent = 25,
                        ImageUrl = "/Image/portableblutoothspeaker.png",
                        Category = "Audio"
                    },
                    new NavbarProductViewModel
                    {
                        Id = 10,
                        Name = "Men's shirt",
                        Description = "Summer Cotton Shirt",
                        Price = 89.99m,
                        DiscountPrice = 69.99m,
                        DiscountPercent = 22,
                        ImageUrl = "/Image/shirt.png",
                        Category = "Fashion"
                    },
                    new NavbarProductViewModel
                    {
                        Id = 13,
                        Name = "Shoes",
                        Description = "Lightweight shoes",
                        Price = 120.00m,
                        DiscountPrice = 89.99m,
                        DiscountPercent = 25,
                        ImageUrl = "/Image/shoes for men.png",
                        Category = "Sports"
                    },
                    new NavbarProductViewModel
                    {
                        Id = 35,
                        Name = "Earphones",
                        Description = "Automatic earphones",
                        Price = 149.99m,
                        DiscountPrice = 119.99m,
                        DiscountPercent = 20,
                        ImageUrl = "/Image/earphone.png",
                        Category = "Home"
                    }
                };

                ViewBag.Title = "Hot Offers";
                ViewBag.Description = "Check out our hottest deals with up to 25% off!";
                ViewBag.CurrentPage = "HotOffers";

                // Store in session for later use (optional)
                HttpContext.Session.SetString("LastVisitedPage", "HotOffers");

                return View(hotOffers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in HotOffers page");
                return View(new List<NavbarProductViewModel>());
            }
        }

        // ========== GIFT BOXES ACTION ==========
        public IActionResult GiftBoxes()
        {
            try
            {
                // Mock data for gift boxes
                var giftBoxes = new List<NavbarGiftViewModel>
                {
                    new NavbarGiftViewModel
                    {
                        Id = 1001,
                        Name = "Birthday Gift Box",
                        Description = "Perfect birthday gift set with candles, chocolates and more",
                        Price = 49.99m,
                        ImageUrl = "/Image/birthdays.png",
                        Items = 5,
                        Occasion = "Birthday"
                    },
                    new NavbarGiftViewModel
                    {
                        Id = 1002,
                        Name = "Wedding Gift Box",
                        Description = "Elegant wedding gift set",
                        Price = 99.99m,
                        ImageUrl = "/Image/wedding.png",
                        Items = 8,
                        Occasion = "Wedding"
                    },
                    new NavbarGiftViewModel
                    {
                        Id = 1003,
                        Name = "Christmas Gift Box",
                        Description = "Festive Christmas gift collection",
                        Price = 59.99m,
                        ImageUrl = "/Image/christmis.png",
                        Items = 6,
                        Occasion = "Christmas"
                    },
                    new NavbarGiftViewModel
                    {
                        Id = 1004,
                        Name = "Thank You Gift Box",
                        Description = "Show appreciation with this gift set",
                        Price = 39.99m,
                        ImageUrl = "/Image/thanku.png",
                        Items = 4,
                        Occasion = "Thank You"
                    },
                    new NavbarGiftViewModel
                    {
                        Id = 1005,
                        Name = "Get Well Soon Gift Box",
                        Description = "Cheer up your loved ones",
                        Price = 44.99m,
                        ImageUrl = "/Image/getwellsoon.png",
                        Items = 5,
                        Occasion = "Get Well"
                    }
                };

                ViewBag.Title = "Gift Boxes";
                ViewBag.Description = "Perfect gifts for every occasion";
                ViewBag.CurrentPage = "GiftBoxes";

                return View(giftBoxes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GiftBoxes page");
                return View(new List<NavbarGiftViewModel>());
            }
        }

        // ========== PROJECTS ACTION ==========
        public IActionResult Projects()
        {
            try
            {
                // Mock data for projects
                var projects = new List<NavbarProjectViewModel>
                {
                    new NavbarProjectViewModel
                    {
                        Id = 1,
                        Name = "Home Renovation",
                        Description = "Complete home renovation supplies",
                        ImageUrl = "/Image/Services/hoomandoutdoor.png",
                        Items = 24,
                        Status = "In Progress",
                        StartDate = DateTime.Now.AddMonths(-2),
                        EndDate = DateTime.Now.AddMonths(2)
                    },
                    new NavbarProjectViewModel
                    {
                        Id = 2,
                        Name = "Office Setup",
                        Description = "Modern office furniture and equipment",
                        ImageUrl = "/images/project-office.png",
                        Items = 18,
                        Status = "Completed",
                        StartDate = DateTime.Now.AddMonths(-6),
                        EndDate = DateTime.Now.AddMonths(-1)
                    },
                    new NavbarProjectViewModel
                    {
                        Id = 3,
                        Name = "Garden Makeover",
                        Description = "Garden tools and plants",
                        ImageUrl = "/images/project-garden.png",
                        Items = 15,
                        Status = "Planning",
                        StartDate = DateTime.Now.AddMonths(1),
                        EndDate = DateTime.Now.AddMonths(4)
                    },
                    new NavbarProjectViewModel
                    {
                        Id = 4,
                        Name = "Kitchen Upgrade",
                        Description = "Modern kitchen appliances",
                        ImageUrl = "/images/project-kitchen.png",
                        Items = 12,
                        Status = "In Progress",
                        StartDate = DateTime.Now.AddMonths(-1),
                        EndDate = DateTime.Now.AddMonths(2)
                    }
                };

                ViewBag.Title = "Projects";
                ViewBag.Description = "Manage your projects and bulk orders";
                ViewBag.CurrentPage = "Projects";

                return View(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Projects page");
                return View(new List<NavbarProjectViewModel>());
            }
        }

        // ========== MENU ITEM ACTION ==========
        public IActionResult MenuItem()
        {
            try
            {
                // Mock data for menu items (featured products)
                var menuItems = new List<NavbarProductViewModel>
                {
                    new NavbarProductViewModel
                    {
                        Id = 1006,
                        Name = "Featured Item 1",
                        Description = "This week's featured product",
                        Price = 49.99m,
                        ImageUrl = "/Image/p1.png",
                        Category = "Featured",
                        IsFeatured = true
                    },
                    new NavbarProductViewModel
                    {
                        Id = 1007,
                        Name = "Featured Item 2",
                        Description = "Customer favorite",
                        Price = 79.99m,
                        ImageUrl = "/Image/p1.png",
                        Category = "Featured",
                        IsFeatured = true
                    }
                };

                ViewBag.Title = "Menu Item";
                ViewBag.Description = "Featured items just for you";
                ViewBag.CurrentPage = "MenuItem";

                return View(menuItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in MenuItem page");
                return View(new List<NavbarProductViewModel>());
            }
        }

        // ========== HELP ACTION ==========
        public IActionResult Help()
        {
            try
            {
                ViewBag.Title = "Help Center";
                ViewBag.CurrentPage = "Help";

                // Mock data for help categories
                ViewBag.HelpCategories = new List<HelpCategory>
                {
                    new HelpCategory { Id = 1, Name = "Account & Login", Icon = "fa-user", Count = 12 },
                    new HelpCategory { Id = 2, Name = "Orders & Shipping", Icon = "fa-truck", Count = 8 },
                    new HelpCategory { Id = 3, Name = "Returns & Refunds", Icon = "fa-rotate-left", Count = 6 },
                    new HelpCategory { Id = 4, Name = "Payments & Pricing", Icon = "fa-credit-card", Count = 10 },
                    new HelpCategory { Id = 5, Name = "Technical Support", Icon = "fa-headset", Count = 15 },
                    new HelpCategory { Id = 6, Name = "Product Information", Icon = "fa-circle-info", Count = 9 }
                };

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Help page");
                return View();
            }
        }
        // ========== FAQ ACTION ==========
        public IActionResult FAQ()
        {
            try
            {
                ViewBag.Title = "Frequently Asked Questions";
                ViewBag.CurrentPage = "FAQ";

                // Mock data for FAQs
                var faqs = new List<FAQItem>
                {
                    new FAQItem
                    {
                        Id = 1,
                        Question = "How do I place an order?",
                        Answer = "To place an order, simply browse our products, add items to your cart, and proceed to checkout. You'll need to provide shipping information and payment details to complete your order.",
                        Category = "Orders"
                    },
                    new FAQItem
                    {
                        Id = 2,
                        Question = "What payment methods do you accept?",
                        Answer = "We accept Visa, Mastercard, American Express, PayPal, and Cash on Delivery (for select locations).",
                        Category = "Payments"
                    },
                    new FAQItem
                    {
                        Id = 3,
                        Question = "How long does shipping take?",
                        Answer = "Standard shipping takes 3-5 business days. Express shipping takes 1-2 business days. International shipping may take 7-14 business days.",
                        Category = "Shipping"
                    },
                    new FAQItem
                    {
                        Id = 4,
                        Question = "What is your return policy?",
                        Answer = "We offer 30-day returns for most items. Items must be unused and in original packaging. Some restrictions apply.",
                        Category = "Returns"
                    },
                    new FAQItem
                    {
                        Id = 5,
                        Question = "How can I track my order?",
                        Answer = "Once your order ships, you'll receive a tracking number via email. You can also track your order in your account dashboard.",
                        Category = "Orders"
                    },
                    new FAQItem
                    {
                        Id = 6,
                        Question = "Do you offer international shipping?",
                        Answer = "Yes, we ship to most countries worldwide. Shipping costs and delivery times vary by location.",
                        Category = "Shipping"
                    }
                };

                return View(faqs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in FAQ page");
                return View(new List<FAQItem>());
            }
        }


        // ========== CONTACT ACTION ==========
        public IActionResult Contact()
        {
            try
            {
                ViewBag.Title = "Contact Us";
                ViewBag.CurrentPage = "Contact";

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Contact page");
                return View();
            }
        }

        // ========== ABOUT ACTION ==========
        public IActionResult About()
        {
            try
            {
                ViewBag.Title = "About Us";
                ViewBag.CurrentPage = "About";

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in About page");
                return View();
            }
        }

        // ========== CONTACT FORM SUBMISSION ==========
        [HttpPost]
        public IActionResult SubmitContact(string name, string email, string subject, string message)
        {
            try
            {
                // Here you would typically send an email or save to database
                _logger.LogInformation($"Contact form submitted: {name}, {email}, {subject}");

                TempData["SuccessMessage"] = "Thank you for contacting us! We'll get back to you soon.";
                return RedirectToAction("Contact");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting contact form");
                TempData["ErrorMessage"] = "Error sending message. Please try again.";
                return RedirectToAction("Contact");
            }
        }
    }

}