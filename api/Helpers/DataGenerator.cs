using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using BCrypt.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Helpers
{
    public class DataGenerator
    {
        private static string[] usernames = new string[40]
        {
            "david123", "thomas123", "emma123", "olivia123", "john123",
            "william123", "sophia123", "james123", "charlotte123", "michael123",
            "mary123", "joseph123", "elizabeth123", "daniel123", "linda123",
            "matthew123", "patricia123", "benjamin123", "barbara123", "jennifer123",
            "robert123", "richard123", "emily123", "jacob123", "emma123",
            "joseph123", "dorothy123", "david123", "elizabeth123", "sarah123",
            "samuel123", "rebecca123", "george123", "caroline123", "thompson123",
            "anna123", "peter123", "rachel123", "adam123", "katherine123"
        };

        private static string[] firstNames = new string[40] // Updated to 40 total names
       {
            "David", "Thomas", "Emma", "Olivia", "John",
            "William", "Sophia", "James", "Charlotte", "Michael",
            "Mary", "Joseph", "Elizabeth", "Daniel", "Linda",
            "Matthew", "Patricia", "Benjamin", "Barbara", "Jennifer",
            "Robert", "Richard", "Emily", "Jacob", "Emma",
            "Joseph", "Dorothy", "David", "Elizabeth", "Sarah",
            "Samuel", "Rebecca", "George", "Caroline", "Thompson",
            "Anna", "Peter", "Rachel", "Adam", "Katherine"
       };

        private static Course[] courses = new Course[10]{
                 new Course { Id = -1, Name = "Mathematics", MaxNumberOfStudents = 20, CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow },
                new Course { Id = -2, Name = "Physics", MaxNumberOfStudents = 24, CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow },
                new Course { Id = -3, Name = "Literature", MaxNumberOfStudents = 25, CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow },
                new Course { Id = -4, Name = "History", MaxNumberOfStudents = 18, CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow },
                new Course { Id = -5, Name = "Biology", MaxNumberOfStudents = 20, CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow },
                new Course { Id = -6, Name = "Chemistry", MaxNumberOfStudents = 30, CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow },
                new Course { Id = -7, Name = "Computer Science", MaxNumberOfStudents = 22, CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow },
                new Course { Id = -8, Name = "Art", MaxNumberOfStudents = 18, CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow },
                new Course { Id = -9, Name = "Music", MaxNumberOfStudents = 20, CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow },
                new Course { Id = -10, Name = "Physical Education", MaxNumberOfStudents = 25, CreatedAt = DateTime.UtcNow, ModifiedAt = DateTime.UtcNow }
            };

        private static string[] lastNames = new string[40]
        {
            "Smith", "Johnson", "Brown", "Davis", "Miller",
            "Wilson", "Moore", "Taylor", "Anderson", "Thomas",
            "Jackson", "White", "Harris", "Martin", "Thompson",
            "Garcia", "Martinez", "Robinson", "Clark", "Rodriguez",
            "Lewis", "Lee", "Walker", "Hall", "Allen",
            "Young", "King", "Wright", "Scott", "Adams",
            "Samuel", "Rebecca", "George", "Caroline", "Thompson",
            "Anna", "Peter", "Rachel", "Adam", "Katherine"
        };

        public static void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>().HasData(courses.ToArray()); // Add courses

            var random = new Random();
            var students = new List<User>();
            var teachers = new List<User>();

            string[] emails = new string[40];
            for (int i = 0; i < 30; i++)
            {
                emails[i] = $"{usernames[i]}@student.com";
            }
            for (int i = 30; i < 40; i++)
            {
                emails[i] = $"{usernames[i]}@teacher.com";
            }
            var hasher = new PasswordHasher<User>();

            User admin = new User
            {
                UserName = "admin123",
                Email = "admin123@gmail.com",
                FirstName = "Tuan",
                LastName = "Truong",
                Provider = Provider.Local,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };
            admin.PasswordHash = hasher.HashPassword(admin, "P@ss123");
            modelBuilder.Entity<User>().HasData(admin);
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string> { UserId = admin.Id, RoleId = "0" });



            for (int i = 0; i < 40; i++)
            {
                User user = new User
                {
                    UserName = usernames[i],
                    Email = emails[i],
                    FirstName = firstNames[i],
                    LastName = lastNames[i],
                    Provider = Provider.Local,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                };
                string roleId = user.Email.Contains("student") ? "1" : "2"; // Student or Teacher
                user.PasswordHash = hasher.HashPassword(user, "P@ss123");
                modelBuilder.Entity<User>().HasData(user);
                // Assign Student role to the user
                modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string> { UserId = user.Id, RoleId = roleId });

                if (i < 30)
                {
                    students.Add(user);
                }
                else
                {
                    teachers.Add(user);
                }
            }

            // Assign random students and teachers to courses
            foreach (var course in courses)
            {
                var courseStudents = students.OrderBy(x => random.Next()).Take(15).ToList();
                var courseTeachers = teachers.OrderBy(x => random.Next()).Take(3).ToList();

                foreach (var student in courseStudents)
                {
                    modelBuilder.Entity<CourseUser>().HasData(new CourseUser { CourseId = course.Id, UserId = student.Id });
                }

                foreach (var teacher in courseTeachers)
                {
                    modelBuilder.Entity<CourseUser>().HasData(new CourseUser { CourseId = course.Id, UserId = teacher.Id });
                }
            }


        }
    }
}