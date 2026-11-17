namespace ProjectEntyCrud.Data;
using Microsoft.EntityFrameworkCore;
using ProjectEntyCrud.Models;

public class ApplicationDbContext:DbContext
    {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
    {

    }
    public DbSet<EmpRecords> empRecords { get; set; }
    public DbSet<AdminLogin> adminLogins { get; set; }
    public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
    public DbSet<UserLogin> UserLogins { get; set; }
    public DbSet<Dept> Depts { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AdminLogin>()
            .ToTable("AdminLogin");


    }
}

