using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectEntyCrud.Data;
using ProjectEntyCrud.Models;

namespace ProjectEntyCrud.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext Context;
        public AdminDashboardController(ApplicationDbContext context)
        {
            Context = context;
        }
        public async Task<IActionResult> DashBoard(string aname)
        {
            ViewBag.TotalEmps = await Context.empRecords.CountAsync();
            ViewBag.AdminName = HttpContext.Session.GetString("AdminName");
            return View();
        }

        public async Task<IActionResult> MonthlyReport(string search, string dept, int? empId, int? month, int? year)
        {
            var employees =  Context.empRecords.Include(x => x.Department).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                employees = employees.Where(
                          x => x.EmpName.Contains(search) ||
                          x.Email.Contains(search) ||
                          x.EmpId.ToString().Contains(search));
            }

            if (!string.IsNullOrEmpty(dept) && int.TryParse(dept, out int deptId))
            {
                employees = employees.Where(x => x.d_id == deptId);
            }

            ViewBag.Employees = await employees.ToListAsync();

            ViewBag.Departments = await Context.Depts.ToListAsync();

            ViewBag.Search = search;
            ViewBag.Dept = dept;
                

            int m = month ?? DateTime.Today.Month;
            int y = year ?? DateTime.Today.Year;

            if (m < 1 || m > 12 || y < 1 || y > 9999)
            {
                return BadRequest("Invalid month or year");
            }
            ViewBag.Month = m;
            ViewBag.Year = y;

            int totalDays = DateTime.DaysInMonth(y,m);
            ViewBag.TotalDays = totalDays;
            ViewBag.TotalPresent = 0;
            ViewBag.TotalAbsent = 0;
            ViewBag.Attendance = new List<AttendanceRecord>();
            ViewBag.EmployeeNotFound = false;

            if (empId != null)
            {
                var employee = await Context.empRecords.Include(x => x.Department).FirstOrDefaultAsync(x => x.EmpId == empId);

                if (employee == null)
                {
                    return NotFound();
                }

                ViewBag.EmpId = employee.EmpId;
                ViewBag.EmpName = employee.EmpName;
                ViewBag.Email = employee.Email;
                ViewBag.DeptId = employee.d_id;
                ViewBag.DeptName = employee.Department?.DName ?? "Not Available";

                DateTime startDate = new DateTime(y, m, 1);
                DateTime endDate = startDate.AddMonths(1);

                var attendance = await Context.AttendanceRecords
                               .Where(x => x.EmpID == empId && x.AttendanceDate >= startDate && x.AttendanceDate < endDate)
                               .OrderByDescending(x => x.AttendanceDate)
                               .ToListAsync();

                ViewBag.Attendance = attendance;

                int totalPresent = attendance.Count(x => x.CheckIn != null);
                int totalAbsent = totalDays - totalPresent;

                ViewBag.TotalPresent = totalPresent ;
                ViewBag.TotalAbsent = totalAbsent;
            }
            return View();
        }
        public async Task<IActionResult> AttendanceHistory(string search,string dept,DateTime? date)
        {
            var history = await (from a in Context.AttendanceRecords
                           join e in Context.empRecords
                           on a.EmpID equals e.EmpId
                           join d in Context.Depts
                           on e.d_id equals d.DID
                           orderby a.AttendanceDate descending
                           select new AdminAttendanceHistoryViewModel
                           {
                               EmpID = e.EmpId,
                               EmpName = e.EmpName,
                               Email = e.Email,

                               d_id = d.DID,
                               d_name = d.DName,

                               AttendanceDate = a.AttendanceDate,
                               CheckIn = a.CheckIn,
                               CheckOut = a.CheckOut
                           }).ToListAsync();

            if (!string.IsNullOrEmpty(search))
            {
                history = history.Where(x =>
                x.EmpName.Contains(search) ||
                x.Email.Contains(search) ||
                x.EmpID.ToString().Contains(search)).ToList();

                
            }
            if (!string.IsNullOrEmpty(dept) &&
        int.TryParse(dept, out int deptId))
            
            {
                history = history.Where(x => x.d_id == deptId).ToList();
            }

            if (date.HasValue)
            {
                history = history.Where(x =>
                    x.AttendanceDate.Date == date.Value.Date
                ).ToList();
            }

            ViewBag.Departments = new SelectList(Context.Depts.ToList(),
              "DID",
              "DName",
               dept
               );

            ViewBag.Search = search;
            ViewBag.Dept = dept;
            return View(history);
        }
     
    }
}

