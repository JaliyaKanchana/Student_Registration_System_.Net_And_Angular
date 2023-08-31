using Microsoft.EntityFrameworkCore;
using Studentregistrationsystem.Models;

namespace Studentregistrationsystem.Model
{
    public class StudentContext : DbContext
    {
        public StudentContext(DbContextOptions<StudentContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
    }
}

