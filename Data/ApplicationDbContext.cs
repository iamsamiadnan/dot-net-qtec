using System;
using dot_net_qtec.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace dot_net_qtec.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        List<IdentityRole> roles = new List<IdentityRole> {
            new IdentityRole { Id = "1916617c-1ec0-4f93-9e3f-f20f5d7f2eb0", Name = "admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id = "28f6aa6e-a405-4ca9-9310-4724c4b12be3", Name = "accountant", NormalizedName = "ACCOUNTANT" },
            new IdentityRole { Id = "c4647ff9-4c2e-434b-bd1f-987207a32e70", Name = "viewer", NormalizedName = "VIEWER" }
        };

        builder.Entity<IdentityRole>().HasData(roles);
    }
}