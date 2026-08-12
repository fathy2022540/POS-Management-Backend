using Microsoft.EntityFrameworkCore;
using POS.Domain.Entities;
using static POS.Infrastructure.EntitiesSeedData;

namespace POS.Infrastructure
{
    public partial class POSDBContext : DbContext
    {

        public POSDBContext(DbContextOptions<POSDBContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new UsersSeedData());
            modelBuilder.ApplyConfiguration(new RolesSeedData());
            //modelBuilder.ApplyConfiguration(new LookupSeedData());
            //modelBuilder.ApplyConfiguration(new LookupItemsSeedData());




            // ==========================================
            // UNIQUE CONSTRAINTS
            // ==========================================


            modelBuilder.Entity<Users>(builder =>
            {
                builder.HasKey(x => x.Id);

                builder.HasOne(x => x.Roles)
                       .WithMany(x => x.UsersRoles)
                       .HasForeignKey(x => x.RoleId)
                       .OnDelete(DeleteBehavior.NoAction);

                builder.ToTable("Users");
            });

            modelBuilder.Entity<RolesPermissions>(builder =>
            {
                builder.HasKey(x => new { x.RoleId, x.PermissionId });
                builder.HasOne(x => x.Role)
                    .WithMany(x => x.RolesPermissions)
                    .HasForeignKey(x => x.RoleId)
                    .OnDelete(DeleteBehavior.NoAction);
                builder.HasOne(x => x.Permission)
                    .WithMany(x => x.RolePermissions)
                    .HasForeignKey(x => x.PermissionId)
                    .OnDelete(DeleteBehavior.NoAction);
                builder.ToTable("RolesPermissions");
            });

            modelBuilder.Entity<SystemModules>(builder =>
            {
                builder.HasKey(x => x.Id);
                builder.HasIndex(x => x.Code).IsUnique();
                builder.ToTable("SystemModules");
            });
            modelBuilder.Entity<SystemScreens>(builder =>
            {
                builder.HasKey(x => x.Id);
                builder.HasIndex(x => x.ScreenCode).IsUnique();
                builder.HasOne(x => x.Module)
                    .WithMany(x => x.Screens)
                    .HasForeignKey(x => x.ModuleId)
                    .OnDelete(DeleteBehavior.NoAction);
                builder.ToTable("SystemScreens");
            });
            modelBuilder.Entity<RoleScreenPermissions>(builder =>
            {
                builder.HasKey(x => new { x.RoleId, x.ScreenId });
                builder.HasOne(x => x.Role)
                    .WithMany(x => x.RoleScreenPermissions)
                    .HasForeignKey(x => x.RoleId)
                    .OnDelete(DeleteBehavior.NoAction);
                builder.HasOne(x => x.Screen)
                    .WithMany(x => x.RoleScreenPermissions)
                    .HasForeignKey(x => x.ScreenId)
                    .OnDelete(DeleteBehavior.NoAction);
                builder.ToTable("RoleScreenPermissions");
            });
            modelBuilder.Entity<UserScreenPermissionOverrides>(builder =>
            {
                builder.HasKey(x => new { x.UserId, x.ScreenId });
                builder.HasOne(x => x.User)
                    .WithMany(x => x.UserScreenPermissionOverrides)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
                builder.HasOne(x => x.Screen)
                    .WithMany(x => x.UserScreenPermissionOverrides)
                    .HasForeignKey(x => x.ScreenId)
                    .OnDelete(DeleteBehavior.NoAction);
                builder.ToTable("UserScreenPermissionOverrides");
            });

            modelBuilder.Entity<Notification>(builder =>
            {
                builder.HasKey(x => x.Id);
                builder.Property(x => x.TitleEn).HasColumnType("nvarchar(250)");
                builder.Property(x => x.TitleAr).HasColumnType("nvarchar(250)");
                builder.Property(x => x.MessageEn).HasColumnType("nvarchar(1000)");
                builder.Property(x => x.MessageAr).HasColumnType("nvarchar(1000)");
                builder.Property(x => x.ReferenceType).HasColumnType("nvarchar(100)");
                builder.Property(x => x.ActionUrl).HasColumnType("nvarchar(500)");
                builder.HasOne(x => x.User)
                    .WithMany(x => x.Notifications)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
                builder.ToTable("Notifications");
            });
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var foreignKeys = entityType.GetForeignKeys();
                foreach (var foreignKey in foreignKeys)
                {
                    if (foreignKey.DeleteBehavior == DeleteBehavior.Cascade)
                    {
                        foreignKey.DeleteBehavior = DeleteBehavior.NoAction;
                    }
                }
            }
            modelBuilder.Entity<WorkflowAction>(builder =>
            {
                builder.HasKey(x => x.Id);

                builder.HasOne(x => x.User)
                       .WithMany(u => u.WorkflowActions)
                       .HasForeignKey(x => x.UserId)
                       .OnDelete(DeleteBehavior.NoAction);

                builder.HasOne(x => x.WorkflowInstance)
                .WithMany(w => w.Actions)
                .HasForeignKey(x => x.WorkflowInstanceId);

                builder.HasOne(x => x.ActionType)
                       .WithMany(l => l.WorkflowActions)
                       .HasForeignKey(x => x.ActionTypeId)
                       .OnDelete(DeleteBehavior.NoAction);

                builder.ToTable("WorkflowActions");
            });

            modelBuilder.Entity<WorkflowInstance>(builder =>
            {
                builder.ToTable("WorkflowInstance");
                builder.HasKey(x => x.Id);

                builder.HasOne(x => x.WorkflowStatus)
                       .WithMany(l => l.WorkflowInstances)
                       .HasForeignKey(x => x.WorkflowStatusId)
                       .OnDelete(DeleteBehavior.NoAction);
            });
        }

        // 1. Define DbSets for all Aggregate Roots and Entities
        public DbSet<Users> Users { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<RolesPermissions> RolesPermissions { get; set; }
        public DbSet<SystemModules> SystemModules { get; set; }
        public DbSet<SystemScreens> SystemScreens { get; set; }
        public DbSet<RoleScreenPermissions> RoleScreenPermissions { get; set; }
        public DbSet<UserScreenPermissionOverrides> UserScreenPermissionOverrides { get; set; }
        public DbSet<Lookup> Lookup { get; set; }
        public DbSet<LookupItems> LookupItems { get; set; }

        public DbSet<AuditLog> AuditLog { get; set; }
        public DbSet<WorkflowAction> WorkflowAction { get; set; }
        public DbSet<WorkflowInstance> WorkflowInstance { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Contact> ContactMessages { get; set; }

        public DbSet<ParamteresConfiguration> ParamteresConfiguration { get; set; }
    }
}
