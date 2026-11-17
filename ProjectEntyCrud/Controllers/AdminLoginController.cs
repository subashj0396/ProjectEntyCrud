using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectEntyCrud.Data;
using ProjectEntyCrud.Models;
using System.Reflection.Metadata.Ecma335;

namespace ProjectEntyCrud.Controllers
{
    public class AdminLoginController : Controller
    {
        private readonly ApplicationDbContext Context;
        public AdminLoginController(ApplicationDbContext context)
        {
            Context = context;
        }

        [HttpGet]
        
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string aname,string password)
        {
            var Data = await Context.adminLogins.FirstOrDefaultAsync(x => x.aname == aname&&x.password==password);
            if (Data != null)
            {
                HttpContext.Session.SetString("AdminName", Data.aname);

                return RedirectToAction("Dashboard", "AdminDashboard");
                //return RedirectToAction("Dashboard", "AdminDashboard", new {aname=Data.aname});
            }
            else
            {
                ViewBag.Message = "Invalid Admin Name or Password";
                return View();
            }
            
        }
        public IActionResult Dashboard() {
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {

            HttpContext.Session.Clear();


            return RedirectToAction("Login", "EmployeeDashboard");
        }
    }
}
