using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using WebApp.Data;
using WebApp.Models;
using WebApp.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApp.Controllers
{
    public class RegisterController : Controller
    {
        private readonly AppDbContext _context;

        public RegisterController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Check if session cookie exists and matches a user
            if (HttpContext != null && HttpContext.Items.TryGetValue("UserFirstName", out var firstName))
            {
                if (firstName != null) ViewBag.Name = firstName;
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // You can log errors to console or debug
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                // For quick debugging, add this to ViewBag:
                ViewBag.Errors = errors;
                return View(model);
            }

            if (ModelState.IsValid)
            {
                // Check if email already exists
                if (_context.Employees.Any(e => e.email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email already registered.");
                    return View(model);
                }

                var newEmployee = new Employees
                {
                    email = model.Email,
                    hashPassword = HashPassword(model.Password),
                    firstName = model.FirstName,
                    lastName = model.LastName
                };

                _context.Employees.Add(newEmployee);
                _context.SaveChanges();

                return RedirectToAction("Index", "Login");
            }

            return View(model);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
