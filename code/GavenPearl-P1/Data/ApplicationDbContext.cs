using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GavenPearl_P1.Models;

namespace GavenPearl_P1.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public ApplicationDbContext()
        {

        }

        // DbSet for the Course model
        public virtual DbSet<Course> Course { get; set; }

        // DbSet for the RegisteredCourses model
        public virtual DbSet<RegisteredCourse> RegisteredCourses { get; set; }
    }
}