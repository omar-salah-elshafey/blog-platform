using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace UserAuthentication.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //seedRoles(builder);
            //addAdmin(builder);
            //builder.Entity<IdentityUserRole<string>>().HasData(
            //    new IdentityUserRole<string>
            //    {
            //        UserId = "11f6dbf7-e2fd-4add-acf4-f4d3df7d7aba", // Admin user ID
            //        RoleId = "e2955fae-bc9a-4ed0-82f3-7a54cd7a2aa7"  // Admin role ID
            //    }
            //);

            builder.Entity<Post>()
                .HasOne(p => p.Author)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

        }

        private static void seedRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole() { Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin".ToUpper() },
                new IdentityRole() { Name = "Author", ConcurrencyStamp = "2", NormalizedName = "Author".ToUpper() },
                new IdentityRole() { Name = "Reader", ConcurrencyStamp = "3", NormalizedName = "Reader".ToUpper() }
                );
        }

        private static void addAdmin(ModelBuilder modelBuilder)
        {
            var adminUser = new ApplicationUser()
            {
                FirstName = "Omar",
                LastName = "Salah",
                UserName = "omar_salah",
                Email = "omarsalah@test.com",
                EmailConfirmed = true,
                NormalizedUserName = "OMAR_SALAH",
                NormalizedEmail = "omarsalah@test.com".ToUpper(),

            };
            var passwordHasher = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "P@ssw0rd");
            modelBuilder.Entity<ApplicationUser>().HasData(adminUser);

        }
    }
}
