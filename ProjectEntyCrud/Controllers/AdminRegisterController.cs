using Microsoft.AspNetCore.Mvc;
using ProjectEntyCrud.Models;
using ProjectEntyCrud.Data;
using Microsoft.EntityFrameworkCore;


namespace ProjectEntyCrud.Controllers
{
    public class AdminRegisterController : Controller
    {
        private readonly ApplicationDbContext Context;
        public AdminRegisterController(ApplicationDbContext context)
        {
            Context = context;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AdminLogin ad)
        {
            if (!ModelState.IsValid)
            {
                return View(ad);
            }

            
            var existingAdmin = await Context.adminLogins
                .FirstOrDefaultAsync(x => x.aname == ad.aname);

            if (existingAdmin != null)
            {
                ViewBag.Message = "Admin Name already exists";
                return View(ad);
            }

            await Context.adminLogins.AddAsync(ad);
            await Context.SaveChangesAsync();

            return RedirectToAction("Login", "AdminLogin");
        }
    }
}
