using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Data;

namespace WebApp.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EmployeeListController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeeListController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public JsonResult CreateEdit(Employees employees)
        {
            if (employees == null) return new JsonResult("Data not inputted correctly");

            var employeeInDb = _context.Employees.Find(employees.ID);
            if (employeeInDb != null) return new JsonResult("ID Already in use");

            _context.Employees.Add(employees);
            _context.SaveChanges();

            return new JsonResult(Ok(employees));
        }

        [HttpGet]
        public JsonResult Get(int ID)
        {
            var employeeInDb = _context.Employees.Find(ID);
            if(employeeInDb == null) return new JsonResult(NotFound());

            return new JsonResult(Ok(employeeInDb));
        }

        [HttpGet()]
        public JsonResult GetAll()
        {
            var result = _context.Employees.ToList();

            return new JsonResult(result);
        }
    }
}
