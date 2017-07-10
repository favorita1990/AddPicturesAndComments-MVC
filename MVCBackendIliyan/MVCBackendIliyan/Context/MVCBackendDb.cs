using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MVCBackendIliyan.Models;

namespace MVCBackendIliyan.Context
{

    public class MVCBackendDb : IdentityDbContext<UserModel, Identity2Role,
    int, UserLogin, UserRole, UserClaim>
    {
        public MVCBackendDb()
            : base("MVCBackend")
        {
        }

        public static MVCBackendDb Create()
        {
            return new MVCBackendDb();
        }

        public virtual DbSet<Forum> Forums { get; set; }
        public virtual DbSet<Gallery> Galleries { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Forum>().ToTable("Forums");
            modelBuilder.Entity<Gallery>().ToTable("Galleries");

            base.OnModelCreating(modelBuilder);
        }
    }
}