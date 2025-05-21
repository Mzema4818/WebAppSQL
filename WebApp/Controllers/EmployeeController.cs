using Microsoft.AspNetCore.Mvc;
using WebApp.Data;      // Your DbContext namespace
using WebApp.Models;    // Your Model namespace
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly AppDbContext _context;

        public EmployeesController(AppDbContext context)
        {
            // Check if session cookie exists and matches a user
            if (HttpContext != null && HttpContext.Items.TryGetValue("UserFirstName", out var firstName))
            {
                if (firstName != null) ViewBag.Name = firstName;
            }

            _context = context;
        }

        // GET: /Employees
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees.ToListAsync();
            return View(employees);  // Returns View passing the list of employees
        }
    }
}
