using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Data;

namespace WebApp.Middlewear
{
    public class SessionMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
        {
            var path = context.Request.Path.Value;

            // Check if user has valid session
            bool isLoggedIn = false;
            if (context.Request.Cookies.TryGetValue("SessionID", out var cookieSessionId))
            {
                var user = dbContext.Employees.FirstOrDefault(e => e.sessionID == cookieSessionId);
                if (user != null)
                {
                    context.Items["UserFirstName"] = user.firstName;
                    isLoggedIn = true;
                }
            }

            // If user is NOT logged in and tries to access a protected page, redirect
            if (!isLoggedIn && path != null && path.Equals("/Employees", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.Redirect("/Login");
                return;  // Important: stop pipeline here so the page doesn't load
            }

            // Otherwise, continue processing normally
            await _next(context);
        }

    }
}
