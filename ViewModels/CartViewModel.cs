using Microsoft.AspNetCore.Mvc;

namespace EcommerceFullstackDesign.ViewModels
{
    public class CartViewModel : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
