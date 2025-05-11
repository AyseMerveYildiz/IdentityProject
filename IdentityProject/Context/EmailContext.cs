using IdentityProject.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityProject.Context
{
    public class EmailContext:IdentityDbContext<AppUser>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=MERMILIUS;initial Catalog=IdentityProjectDb;integrated security=true;trust server certificate=true");
        }
        public DbSet<Message> Messages { get; set; }
    }
}
