using Microsoft.AspNetCore.Mvc;
using WebApp.Data;

namespace WebApp.Controllers
{
    public class SessionController : Controller
    {
        private readonly AppDbContext _context;

        public SessionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Logout()
        {
            // Try to get the session ID from the cookie
            if (Request.Cookies.TryGetValue("SessionID", out var sessionId))
            {
                // Find the user with that session ID
                var user = _context.Employees.FirstOrDefault(e => e.sessionID == sessionId);

                if (user != null)
                {
                    // Clear the sessionID from the database
                    user.sessionID = null;
                    _context.SaveChanges();
                }
            }

            // Delete the session cookie
            Response.Cookies.Delete("SessionID");

            // Clear the in-memory context item (optional)
            HttpContext.Items["UserFirstName"] = null;

            return RedirectToAction("Index", "Home");
        }

    }

}
