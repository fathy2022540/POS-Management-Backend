using Microsoft.EntityFrameworkCore;
using POS.Domain.Entities;

namespace POS.Infrastructure
{
    public sealed class POSDBContext : DbContext
    {
        public POSDBContext(DbContextOptions<POSDBContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ApplyAssemblyConfigurations(modelBuilder);
            ApplyCoreSecurityMappings(modelBuilder);
            ApplyWorkflowMappings(modelBuilder);
            ApplyNotificationMappings(modelBuilder);
            ApplyGlobalDeleteBehavior(modelBuilder);
        }

        private static void ApplyAssemblyConfigurations(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(POSDBContext).Assembly);
        }

        private static void ApplyCoreSecurityMappings(ModelBuilder modelBuilder)
        {
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
        }

        private static void ApplyWorkflowMappings(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkflowAction>(builder =>
            {
                builder.HasKey(x => x.Id);

                builder.HasOne(x => x.User)
                    .WithMany(x => x.WorkflowActions)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.NoAction);

                builder.HasOne(x => x.WorkflowInstance)
                    .WithMany(x => x.Actions)
                    .HasForeignKey(x => x.WorkflowInstanceId)
                    .OnDelete(DeleteBehavior.NoAction);

                builder.HasOne(x => x.ActionType)
                    .WithMany(x => x.WorkflowActions)
                    .HasForeignKey(x => x.ActionTypeId)
                    .OnDelete(DeleteBehavior.NoAction);

                builder.ToTable("WorkflowActions");
            });

            modelBuilder.Entity<WorkflowInstance>(builder =>
            {
                builder.HasKey(x => x.Id);

                builder.HasOne(x => x.WorkflowStatus)
                    .WithMany(x => x.WorkflowInstances)
                    .HasForeignKey(x => x.WorkflowStatusId)
                    .OnDelete(DeleteBehavior.NoAction);

                builder.ToTable("WorkflowInstance");
            });
        }

        private static void ApplyNotificationMappings(ModelBuilder modelBuilder)
        {
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
        }

        private static void ApplyGlobalDeleteBehavior(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var foreignKey in entityType.GetForeignKeys())
                {
                    if (foreignKey.DeleteBehavior == DeleteBehavior.Cascade)
                    {
                        foreignKey.DeleteBehavior = DeleteBehavior.NoAction;
                    }
                }
            }
        }

        public DbSet<Users> Users => Set<Users>();
        public DbSet<Roles> Roles => Set<Roles>();
        public DbSet<Permissions> Permissions => Set<Permissions>();
        public DbSet<RolesPermissions> RolesPermissions => Set<RolesPermissions>();
        public DbSet<SystemModules> SystemModules => Set<SystemModules>();
        public DbSet<SystemScreens> SystemScreens => Set<SystemScreens>();
        public DbSet<RoleScreenPermissions> RoleScreenPermissions => Set<RoleScreenPermissions>();
        public DbSet<UserScreenPermissionOverrides> UserScreenPermissionOverrides => Set<UserScreenPermissionOverrides>();
        public DbSet<Lookup> Lookup => Set<Lookup>();
        public DbSet<LookupItems> LookupItems => Set<LookupItems>();
        public DbSet<AuditLog> AuditLog => Set<AuditLog>();
        public DbSet<WorkflowAction> WorkflowAction => Set<WorkflowAction>();
        public DbSet<WorkflowInstance> WorkflowInstance => Set<WorkflowInstance>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<Contact> ContactMessages => Set<Contact>();
        public DbSet<ParamteresConfiguration> ParamteresConfiguration => Set<ParamteresConfiguration>();

        public DbSet<Order> Order => Set<Order>();
        public DbSet<OrderItem> OrderItem => Set<OrderItem>();
        public DbSet<Payment> Payment => Set<Payment>();
        public DbSet<Product> Product => Set<Product>();
        public DbSet<ProductRecipeItem> ProductRecipeItem => Set<ProductRecipeItem>();
        public DbSet<InventoryItem> InventoryItem => Set<InventoryItem>();
        public DbSet<StockTransaction> StockTransaction => Set<StockTransaction>();
        public DbSet<Store> Store => Set<Store>();
        public DbSet<InventoryTransfer> InventoryTransfer => Set<InventoryTransfer>();
    }
}