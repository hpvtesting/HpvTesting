using HPVTesting.Business.ViewModels.Account;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HPVTesting.Domain.Models;

namespace HPVTesting.Domain
{
    public partial class HPVTestingContext : IdentityDbContext<ApplicationUser>
    {
        public HPVTestingContext(DbContextOptions<HPVTestingContext> options) : base(options)
        {
        }

        public DbSet<User> User { get; set; }

        public DbSet<UserSocialConnection> UserSocialConnection { get; set; }

    }
}