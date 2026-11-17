
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectEntyCrud.Models;
using ProjectEntyCrud.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

public class EmpRecordsController : Controller
{
    private readonly ApplicationDbContext _context;

    public EmpRecordsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: EMPRECORDSS

    private void LoadDepartments(int? selectdId = null)
    {
        ViewBag.Departments = new SelectList(
        _context.Depts.ToList(),
        "DID",
        "DName",
          selectdId
          );
    }
    public async Task<IActionResult> Index(string search,string dept,string desig,int? age)    
    {
        var employees = _context.empRecords.Include(x=>x.Department).AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            employees = employees.Where(x => x.EmpName.Contains(search) || x.Email.Contains(search) || x.EmpId.ToString().Contains(search));    
        }

        if (!string.IsNullOrEmpty(dept) && int.TryParse(dept,out int deptId))
        {
            employees = employees.Where(x => x.d_id == deptId);

        }

        if (!string.IsNullOrEmpty(desig))
        {
            employees = employees.Where(x => x.Desigination == desig);
        }

        if (age != null)
        {
            employees = employees.Where(x => x.Age == age);
        }
        ViewBag.Departments =new SelectList(
                            _context.Depts.ToList(),
                             "DID",
                             "DName",
                             dept
                             );
        ViewBag.Designations = await _context.empRecords
          .Where(x => x.Desigination != null)
          .Select(x => x.Desigination)
          .Distinct()
          .ToListAsync();

        ViewBag.Search = search;
        ViewBag.Dept = dept;
        ViewBag.Desig = desig;
        ViewBag.Age = age;

        return View(await employees.ToListAsync());
    }

    // GET: EMPRECORDSS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var emprecords = await _context.empRecords
            .Include(x=>x.Department)
            .FirstOrDefaultAsync(m => m.EmpId == id);
        if (emprecords == null)
        {
            return NotFound();
        }

        return View(emprecords);
    }

    // GET: EMPRECORDSS/Create
    public IActionResult Create()
    {
        LoadDepartments();
        return View();
    }

    // POST: EMPRECORDSS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("EmpId,Age,EmpName,Email,Ph,d_id,Desigination,JoiningDate")] EmpRecords emprecords)
    {
        if (ModelState.IsValid)
        {
            _context.empRecords.Add(emprecords);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        LoadDepartments(emprecords.d_id);
        return View(emprecords);
    }

    // GET: EMPRECORDSS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var emprecords = await _context.empRecords.Include(x => x.Department)
            .FirstOrDefaultAsync(x => x.EmpId == id);

        if (emprecords == null)
        {
            return NotFound();
        }
        LoadDepartments(emprecords.d_id);

        return View(emprecords);
    }

    // POST: EMPRECORDSS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("EmpId,Age,EmpName,Email,Ph,d_id,Desigination,JoiningDate")] EmpRecords emprecords)
    {
        if (id != emprecords.EmpId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var employee = await _context.empRecords.FindAsync(id);

                if (employee == null)
                {
                    return NotFound();
                }


                employee.Age = emprecords.Age;
                employee.EmpName = emprecords.EmpName;
                employee.Email = emprecords.Email;
                employee.Ph = emprecords.Ph;
                employee.d_id = emprecords.d_id;
                employee.Desigination = emprecords.Desigination;
                employee.JoiningDate = emprecords.JoiningDate;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmpRecordsExists(emprecords.EmpId))
                    return NotFound();

                throw;
            }
        }

        LoadDepartments(emprecords.d_id);
        return View(emprecords);
    }

        

    // GET: EMPRECORDSS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var emprecords = await _context.empRecords
             .Include(x => x.Department)
            .FirstOrDefaultAsync(m => m.EmpId == id);
        if (emprecords == null)
        {
            return NotFound();
        }

        return View(emprecords);
    }

    // POST: EMPRECORDSS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {

        if (id == null)
        {
            return NotFound();
        }
        var emprecords = await _context.empRecords.FindAsync(id);
        if (emprecords == null)
        {
            return NotFound();
            
        }
        _context.empRecords.Remove(emprecords);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool EmpRecordsExists(int? id)
    {
        return _context.empRecords.Any(e => e.EmpId == id);
    }
}
