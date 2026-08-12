using POS.Domain.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Security.Cryptography;

namespace POS.Infrastructure
{
    public static class EntitiesSeedData
    {
        private static readonly DateTime SeedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public class RolesSeedData : IEntityTypeConfiguration<Roles>
        {
            public void Configure(EntityTypeBuilder<Roles> builder)
            {
                builder.ToTable("Roles");
                Roles roles = new Roles()
                {
                    Id = 1,
                    Code = "ADMIN",
                    NameEn = "admin",
                    NameAr = "ادمن",
                    CreatedBy = 1,
                    CreatedDate = SeedDate, // FIXED
                    IsActive = true,
                };
                builder.HasData(roles);
            }
        }

        public class UsersSeedData : IEntityTypeConfiguration<Users>
        {
            public void Configure(EntityTypeBuilder<Users> builder)
            {
                builder.ToTable("Users");
                Users user = new Users()
                {
                    Id = 2,
                    UserName = "admin",
                    FullName = "admin last_admin",
                    FirstName = "admin",
                    LastName = "last_admin",
                    Email = "admin@gmail.com",
                    CreatedBy = 1,
                    CreatedDate = SeedDate, // FIXED
                    IsActive = true,
                };

                user.PasswordHash = HashPassword("Admin123");
                user.Pass1 = user.PasswordHash;
                user.PassDate1 = SeedDate; // FIXED
                builder.HasData(user);
            }
        }

        //public class LookupSeedData : IEntityTypeConfiguration<Lookup>
        //{
        //    public void Configure(EntityTypeBuilder<Lookup> builder)
        //    {
        //        builder.ToTable("Lookup");
        //        builder.HasData(
        //            new Lookup { Id = 1, Name = "VendorStatus", NameAR = "حالة الموردين", CreatedBy = 1, CreatedDate = SeedDate, IsActive = true } // FIXED
        //        );
        //    }
        //}

        //public class LookupItemsSeedData : IEntityTypeConfiguration<LookupItems>
        //{
        //    public void Configure(EntityTypeBuilder<LookupItems> builder)
        //    {
        //        builder.ToTable("LookupItems");
        //        builder.HasData(
        //            new LookupItems { Id = 1, LookupId = 1, NameEn = "Draft", NameAR = "مسودة", CreatedBy = 1, CreatedDate = SeedDate, IsActive = true }, // FIXED
        //            new LookupItems { Id = 2, LookupId = 1, NameEn = "PendingReview", NameAR = "قيد المراجعة", CreatedBy = 1, CreatedDate = SeedDate, IsActive = true }, // FIXED
        //            new LookupItems { Id = 3, LookupId = 1, NameEn = "Approved", NameAR = "معتمد", CreatedBy = 1, CreatedDate = SeedDate, IsActive = true }, // FIXED
        //            new LookupItems { Id = 4, LookupId = 1, NameEn = "Rejected", NameAR = "مرفوض", CreatedBy = 1, CreatedDate = SeedDate, IsActive = true }, // FIXED
        //            new LookupItems { Id = 5, LookupId = 1, NameEn = "Suspended", NameAR = "موقوف", CreatedBy = 1, CreatedDate = SeedDate, IsActive = true }, // FIXED
        //            new LookupItems { Id = 6, LookupId = 1, NameEn = "Blacklisted", NameAR = "مدرج في القائمة السوداء", CreatedBy = 1, CreatedDate = SeedDate, IsActive = true } // FIXED
        //        );
        //    }
        //}


        private static string HashPassword(string password)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create()) { rng.GetBytes(salt); }

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password, salt: salt, prf: KeyDerivationPrf.HMACSHA256, iterationCount: 10000, numBytesRequested: 256 / 8));

            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }
    }



}

