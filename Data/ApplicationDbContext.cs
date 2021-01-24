using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Website.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Website.Models.ProjectModel> ProjectModel { get; set; }

        public DbSet<Website.Models.ContactModel> ContactModel { get; set; }
    }
}
