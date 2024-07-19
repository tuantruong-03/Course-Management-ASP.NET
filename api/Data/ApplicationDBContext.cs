using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Helpers;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class ApplicationDBContext : IdentityDbContext<User>
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions){

        }

        // Add model
        public DbSet<Course> Courses {get;set;}
        public DbSet<Score> Scores {get; set;}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Many to many
            builder.Entity<CourseUser>()
            .HasOne(coureUser => coureUser.Course)
            .WithMany(course => course.CourseUser)
            .HasForeignKey(courseUser => courseUser.CourseId);
            // Automatically delete on cascade (remove a course, it will remove a courseUser associated with that course)

            builder.Entity<CourseUser>()
            .HasOne(courseUser => courseUser.User)
            .WithMany(user => user.CourseUser)
            .HasForeignKey(courseUser => courseUser.UserId);
            // Automatically delete on cascade (remove a user, it will remove a courseUser associated with that user)


            builder.Entity<Score>()
            .HasOne(score => score.User)
            .WithMany(user => user.Scores)
            .HasForeignKey(score => score.UserId);

             builder.Entity<Score>()
            .HasOne(score => score.Course)
            .WithMany(user => user.Scores)
            .HasForeignKey(score => score.CourseId);
            // Add role
            List<IdentityRole> roles = new List<IdentityRole> {
                new IdentityRole {
                    Id = "0",
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole {
                    Id = "1",
                    Name = "Student",
                    NormalizedName = "STUDENT"
                },
                new IdentityRole {
                    Id = "2",
                    Name = "Teacher",
                    NormalizedName = "TEACHER"
                }
            };
            builder.Entity<IdentityRole>().HasData(roles);
            DataGenerator.SeedData(builder);
        }
    }
}