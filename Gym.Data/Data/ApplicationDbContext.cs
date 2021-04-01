using Gym.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gym.Data.Data
{

    //Add-Migration Init -OutPutDir "Data/Migrations"
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<GymClass> GymClasses { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUserGymClass>().HasKey(a => new { a.ApplicationUserId, a.GymClassId });

            builder.Entity<GymClass>().HasQueryFilter(g => g.StartDate > DateTime.Now);
        }
    }
}
