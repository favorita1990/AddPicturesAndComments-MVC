using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MVCBackendIliyan.Context;

namespace MVCBackendIliyan.Models
{
    public class UserRole : IdentityUserRole<int> { }
    public class UserClaim : IdentityUserClaim<int> { }
    public class UserLogin : IdentityUserLogin<int> { }

    public class Identity2Role : IdentityRole<int, UserRole>
    {
        public Identity2Role() { }
        public Identity2Role(string name) { Name = name; }
    }

    public class UserStore : UserStore<UserModel, Identity2Role, int, UserLogin, UserRole, UserClaim>
    {
        public UserStore(MVCBackendDb context)
            : base(context)
        {
        }
    }

    public class RoleStore : RoleStore<Identity2Role, int, UserRole>
    {
        public RoleStore(MVCBackendDb context)
            : base(context)
        {
        }
    }

    public class UserModel : IdentityUser<int, UserLogin, UserRole, UserClaim>
    {
        public UserModel()
        {
            this.Forums = new HashSet<Forum>();
            this.Galleries = new HashSet<Gallery>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Genter { get; set; }
        public DateTime Time { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<UserModel, int> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public virtual ICollection<Forum> Forums { get; set; }
        public virtual ICollection<Gallery> Galleries { get; set; }
    }
}