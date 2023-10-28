using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SmartAdmin.WebUI.Entities
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public ApplicationContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Applicant> Applicants { get; set; }
        public DbSet<TuitionPlan> TuitionPlans { get; set; }
        public DbSet<RuyaDocument> RuyaDocuments { get; set; }
        public DbSet<Parent> Parents { get; set; }

    }
}
