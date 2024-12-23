using AnketPortali.Models;
using AspNetCoreHero.ToastNotification.Notyf.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NETCore.Encrypt.Extensions;

namespace AnketPortali.Models
{
    public class AppDbContext : IdentityDbContext <AppUser,AppRole,string>
    {
        private readonly IConfiguration _config;
        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration config) : base(options)
        {
            _config = config;
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Survey> Surveys { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<SurveyApplication> SurveyApplications { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }

       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
 
            var adminRole = Guid.NewGuid().ToString();
            var adminUser = Guid.NewGuid().ToString();

    
            modelBuilder.Entity<AppRole>().HasData(
                                new AppRole
                                {
                                    Id = adminRole, 
                                    Name = "Admin",
                                    NormalizedName = "ADMIN"
                                });

            modelBuilder.Entity<AppRole>().HasData(
                               new AppRole
                               {
                                   Id = Guid.NewGuid().ToString(),
                                   Name = "User",
                                   NormalizedName = "USER"
                               });

            modelBuilder.Entity<AppUser>().HasData(
               new AppUser
               {
                   Id = adminUser,
                   UserName = "admin",
                   NormalizedUserName = "ADMIN",
                   Email = "admin@admin.com",
                   NormalizedEmail = "ADMIN@ADMIN.COM",
                   EmailConfirmed = true,
                   PasswordHash = new PasswordHasher<AppUser>().HashPassword(null, "Admin")
               });

        
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    UserId = adminUser, 
                    RoleId = adminRole  
                });

            modelBuilder.Entity<SurveyApplication>()
                   .HasOne(sa => sa.User)
                   .WithMany()
                   .HasForeignKey(sa => sa.UserId);

            modelBuilder.Entity<SurveyApplication>()
                .HasOne(sa => sa.Survey)
                .WithMany()
                .HasForeignKey(sa => sa.SurveyId);

            base.OnModelCreating(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }
    }
}
