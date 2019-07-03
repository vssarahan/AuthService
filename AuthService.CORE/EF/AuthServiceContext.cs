using AuthService.DATA.Enteties;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.CORE.EF
{
    public class AuthServiceContext: IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public AuthServiceContext(DbContextOptions<AuthServiceContext> opt): base(opt)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<RefreshToken>()
                .HasKey(rt => new { rt.UserId, rt.Token });
        }

    }
}
