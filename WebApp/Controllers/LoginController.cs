using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
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
        public IActionResult Index(Employees model)
        {
            if (!string.IsNullOrWhiteSpace(model.email) && !string.IsNullOrWhiteSpace(model.hashPassword))
            {
                var user = _context.Employees.FirstOrDefault(e => e.email == model.email);

                if (user != null && user.hashPassword == HashPassword(model.hashPassword))
                {
                    // Generate a new secure sessionID
                    var sessionId = GenerateSessionId();

                    // Save sessionID to DB
                    user.sessionID = sessionId;
                    _context.SaveChanges();

                    // Set session cookie
                    Response.Cookies.Append("SessionID", sessionId, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,  // Make sure to use HTTPS in production!
                        Expires = DateTimeOffset.UtcNow.AddDays(7)
                    });

                    // Also store email in session for quick access
                    //HttpContext.Session.SetString("UserEmail", user.email);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid login credentials.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Please fill in all fields.");
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

        private string GenerateSessionId()
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[32];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
