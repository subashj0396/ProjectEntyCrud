using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectEntyCrud.Data;
using ProjectEntyCrud.Models;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace ProjectEntyCrud.Controllers
{
    public class EmployeeDashboardController : Controller
    {
        private readonly ApplicationDbContext Context;
        public EmployeeDashboardController(ApplicationDbContext context)
        {
            Context = context;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  async Task<IActionResult> Login(int id, int deptId,string password)
        {
            var emp = await Context.empRecords.Include(x => x.Department).FirstOrDefaultAsync(
     x => x.EmpId == id && x.d_id == deptId
 );
            // var emp = Context.empRecords.Include(x => x.Department).FirstOrDefault(x => x.EmpId == id&&x.d_id==deptId);

            if (emp == null)
            {
                ViewBag.Error = "Invalid Employee Id or Department ID";
                return View();
            }

            var user =await Context.UserLogins.FirstOrDefaultAsync(
             x => x.EmpId == id && x.d_id == deptId
);

            if (user == null)
            {
                ViewBag.Error = "Please create your password first.";
                return View();
               
            }

            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<UserLogin>();
            var result = hasher.VerifyHashedPassword(user, user.pwd, password);

            if (result == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed)
            {
                ViewBag.Error = "Invalid Password";
                return View();
            }

            HttpContext.Session.SetInt32("EmpId", emp.EmpId);
            HttpContext.Session.SetInt32("DeptId", emp.d_id);
            HttpContext.Session.SetString("EmpName", emp.EmpName);
            HttpContext.Session.SetString("DeptName", emp.Department?.DName ?? "");

            return RedirectToAction("DashBoard");

        }
        
        
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int id,int deptId,string password,string confirmPassword)
        {
            var emp = await Context.empRecords.FirstOrDefaultAsync(x => x.EmpId == id && x.d_id == deptId);

            if (emp == null)
            {
                ViewBag.Error = "Invalid Employee IDor Department ID";
                return View();
            }

            var existingUser = await Context.UserLogins.FirstOrDefaultAsync(x => x.EmpId == id);

            if (existingUser != null)
            {
                ViewBag.Error = "Password already registered. Please Login.";
                return View();
            }

            if (string.IsNullOrWhiteSpace(password)||
            
                password.Length<8||
                !password.Any(char.IsUpper)||
                !password.Any(char.IsLower)||
                !password.Any(char.IsDigit)||
                !password.Any(c=>
                !char.IsAsciiLetterOrDigit(c)))
                {
                    ViewBag.Error = "Password must contain at least 8 characters, uppercase, lowercase, number and special character.";
                    return View();
                }

            if (password != confirmPassword)
            {
                ViewBag.Error= "Password and Confirm Password do not match.";
                return View();
            }

            var user = new UserLogin
            {
                EmpId = id,
                d_id = emp.d_id,

                pwd = new Microsoft.AspNetCore.Identity.PasswordHasher<UserLogin>().HashPassword(null, password)
            };

           await Context.UserLogins.AddAsync(user);
            await Context.SaveChangesAsync();
            return RedirectToAction("Login");

        }
        public IActionResult DashBoard()
        {
            int? id = HttpContext.Session.GetInt32("EmpId");
            if (id == null)
            {
                return RedirectToAction("Login");
            }
            ViewBag.EmpId = id;
            ViewBag.EmpName = HttpContext.Session.GetString("EmpName");
            ViewBag.DeptId = HttpContext.Session.GetInt32("DeptId");
            ViewBag.DeptName = HttpContext.Session.GetString("DeptName");
            return View();
        }

        public async Task<IActionResult> MyRecords()
        {
            int? id = HttpContext.Session.GetInt32("EmpId");
            if (id == null)
            {
                return RedirectToAction("Login");
            }
            var emp = await Context.empRecords.Include(x => x.Department).FirstOrDefaultAsync(x => x.EmpId == id);
            if (emp == null)
            {
                return NotFound();
            }
            return View(emp);
        }


        public async Task<IActionResult> Edit()
        {
            int? id = HttpContext.Session.GetInt32("EmpId");
            if (id == null)
            {
                return RedirectToAction("Login");
            }
            var emp = await Context.empRecords.Include(x => x.Department)
            .FirstOrDefaultAsync(x => x.EmpId == id);
            if (emp == null)
            {
                return NotFound();
            }
            ViewBag.Departments = await Context.Depts.ToListAsync();
            return View(emp);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmpRecords ed)
        {
            int? id = HttpContext.Session.GetInt32("EmpId");
            if (id == null)
            {
                return RedirectToAction("Login");
            }
            var exemp = await Context.empRecords.FirstOrDefaultAsync(x => x.EmpId == id);

            if (exemp == null)
            {
                return NotFound();
            }
            
                exemp.EmpName = ed.EmpName;
                exemp.Age = ed.Age;
                exemp.Email = ed.Email;
                exemp.Ph = ed.Ph;
                exemp.d_id = ed.d_id;
                
                exemp.Desigination = ed.Desigination;
                exemp.JoiningDate = ed.JoiningDate;

                await Context.SaveChangesAsync();
                TempData["Success"] = "Your records updated successfully!";

                return RedirectToAction("MyRecords");
            
            //ViewBag.Departments = Context.Depts.ToList();
            //return View(ed);
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }

        public async Task<IActionResult> MonthlyAttendance(int? month,int? year)
        {
            int? id = HttpContext.Session.GetInt32("EmpId");
            if (id == null)
            {
                return RedirectToAction("Login");
            }
            ViewBag.EmpName = HttpContext.Session.GetString("EmpName");

            ViewBag.DeptId = HttpContext.Session.GetInt32("DeptId");
            ViewBag.DeptName = HttpContext.Session.GetString("DeptName");


            int m = month ?? DateTime.Today.Month;
            int y = year ?? DateTime.Today.Year;

            if (m < 1 || m > 12 || y < 2000 || y >2100)
            {
                return BadRequest("Invalid Month Or Year");
                

            }
            ViewBag.Month = m;
            ViewBag.Year = y;

            DateTime startDate = new DateTime(y, m, 1);
            DateTime endDate = startDate.AddMonths(1);

            DateTime reportEnd = endDate;

            if (y == DateTime.Today.Year && m == DateTime.Today.Month)
            {
                reportEnd = DateTime.Today.AddDays(1);
            }
            else if (startDate > DateTime.Today)
            {
                reportEnd = startDate;
            }

            var attendance = await Context.AttendanceRecords
                           .Where(x => x.EmpID == id
                            && x.AttendanceDate >=startDate
                            && x.AttendanceDate < reportEnd
                            && x.CheckIn != null &&
                            x.CheckOut != null)
                           .ToListAsync();

            int totalPresent = attendance.Count;
            int totalWorkingDays = 0;

            for(DateTime date = startDate; date < reportEnd; date = date.AddDays(1))
            {
                totalWorkingDays++;
            }

            int totalAbsent = totalWorkingDays - totalPresent;

            double percentage = 0;

            if (totalWorkingDays > 0)
            {
                percentage = (double)totalPresent / totalWorkingDays * 100;
            }

            ViewBag.TotalPresent = totalPresent;
            ViewBag.TotalWorkingDays = totalWorkingDays;
            ViewBag.TotalAbsent = totalAbsent;
            ViewBag.AttendancePercentange = Math.Round(percentage, 2);

            return View();
        }
        
    }
}
