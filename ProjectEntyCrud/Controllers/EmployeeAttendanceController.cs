using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectEntyCrud.Data;
using ProjectEntyCrud.Models;

namespace ProjectEntyCrud.Controllers
{
    public class EmployeeAttendanceController : Controller
    {
        private readonly ApplicationDbContext Context;
        public EmployeeAttendanceController(ApplicationDbContext context)
        {
            Context = context;
        }
        public async Task<IActionResult> Index()
        {
            int? id = HttpContext.Session.GetInt32("EmpId");
            if (id == null)
            {
                return RedirectToAction("Login", "EmployeeDashboard");
            }

            ViewBag.EmpId = id;
            ViewBag.EmpName = HttpContext.Session.GetString("EmpName");
            ViewBag.DeptId = HttpContext.Session.GetInt32("DeptId");
            ViewBag.DeptName = HttpContext.Session.GetString("DeptName");

            DateTime today = DateTime.Today;

            var attendance = await Context.AttendanceRecords
                .FirstOrDefaultAsync(a => a.EmpID == id &&
                    a.AttendanceDate >= today &&
                    a.AttendanceDate < today.AddDays(1));

            return View(attendance);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIn()
        {
            int? id = HttpContext.Session.GetInt32("EmpId");
            if (id == null)
            {
                return RedirectToAction("Login", "EmployeeDashboard");
            }
            DateTime today = DateTime.Today;

            var attendance = await Context.AttendanceRecords.FirstOrDefaultAsync(x => x.EmpID == id && x.AttendanceDate >= today &&
                x.AttendanceDate < today.AddDays(1));

            if (attendance != null)
            {
                TempData["Message"] = "You Have already Check In today";
                return RedirectToAction("Index");
            }

            var emp = await Context.empRecords
               .Include(x => x.Department)
               .FirstOrDefaultAsync(x => x.EmpId == id);

            if (emp == null)
            {
                return NotFound("Employee not found.");
            }

            if (emp.Department == null)
            {
                TempData["Message"] = "Department not found for this employee.";
                return RedirectToAction("Index");
            }


            AttendanceRecord record = new AttendanceRecord
            {
                EmpID = emp.EmpId,
                d_id = emp.d_id,                 
                d_name = emp.Department.DName,  
                AttendanceDate = today,
                CheckIn = DateTime.Now
            };




            await Context.AttendanceRecords.AddAsync(record);


           await  Context.SaveChangesAsync();

            TempData["Message"] = "Check-In successful!";

            return RedirectToAction("Index");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut()
        {
            int? id = HttpContext.Session.GetInt32("EmpId");
            if (id == null)
            {
                return RedirectToAction("Login", "EmployeeDashboard");
            }
            DateTime today = DateTime.Today;


            var attendance = await Context.AttendanceRecords.FirstOrDefaultAsync(x => x.EmpID == id &&
                x.AttendanceDate >= today &&
                x.AttendanceDate < today.AddDays(1));


            if (attendance == null)
            {
                TempData["Message"] = "Please Check-In first.";
                return RedirectToAction("Index");
            }


            if (attendance.CheckOut != null)
            {
                TempData["Message"] = "You have already checked out today.";
                return RedirectToAction("Index");
            }





            attendance.CheckOut = DateTime.Now;
            await Context.SaveChangesAsync();

            TempData["Message"] = "Check-Out successful!";

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> History()
        {
            int? id = HttpContext.Session.GetInt32("EmpId");
            if (id == null)
            {
                return RedirectToAction("Login", "EmployeeDashboard");
            }

            ViewBag.EmpId = id;
            ViewBag.EmpName = HttpContext.Session.GetString("EmpName");
            ViewBag.DeptId = HttpContext.Session.GetInt32("DeptId");
            ViewBag.DeptName = HttpContext.Session.GetString("DeptName");


            var history = await Context.AttendanceRecords.Where(x => x.EmpID == id)
                .OrderByDescending(x => x.AttendanceDate)
                .ToListAsync();
            return View(history);
        }
    }
}