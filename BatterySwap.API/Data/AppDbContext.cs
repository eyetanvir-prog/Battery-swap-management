using BatterySwap.API.Models;
using BatterySwap.Shared.Constants;
using Microsoft.EntityFrameworkCore;

namespace BatterySwap.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Station> Stations => Set<Station>();
    public DbSet<Battery> Batteries => Set<Battery>();
    public DbSet<Swap> Swaps => Set<Swap>();
    public DbSet<BalanceRecharge> BalanceRecharges => Set<BalanceRecharge>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.ToTable("admins");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Phone).HasMaxLength(20).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(150).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(256).IsRequired();
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.HasIndex(x => x.Phone).IsUnique();
            entity.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("employees");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Phone).HasMaxLength(20).IsRequired();
            entity.Property(x => x.Nid).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Address).HasMaxLength(300);
            entity.Property(x => x.PhotoPath).HasMaxLength(500);
            entity.Property(x => x.Username).HasMaxLength(100).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(256).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(20).IsRequired();
            entity.HasIndex(x => x.Phone).IsUnique();
            entity.HasIndex(x => x.Nid).IsUnique();
            entity.HasIndex(x => x.Username).IsUnique();
            entity.HasIndex(x => x.StationId).IsUnique().HasFilter("[StationId] IS NOT NULL");
            entity.HasOne<Station>()
                .WithOne()
                .HasForeignKey<Employee>(x => x.StationId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.ToTable("clients");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Phone).HasMaxLength(20).IsRequired();
            entity.Property(x => x.Nid).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Address).HasMaxLength(300);
            entity.Property(x => x.PhotoPath).HasMaxLength(500);
            entity.Property(x => x.VehicleType).HasMaxLength(50);
            entity.Property(x => x.VehicleNumber).HasMaxLength(50);
            entity.Property(x => x.Balance).HasPrecision(10, 2).HasDefaultValue(BatterySwapDefaults.RegistrationBalance);
            entity.Property(x => x.RegistrationDate).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(x => x.Status).HasMaxLength(20).IsRequired();
            entity.HasIndex(x => x.Phone).IsUnique();
            entity.HasIndex(x => x.Nid).IsUnique();
            entity.HasIndex(x => x.CurrentBatteryId).IsUnique().HasFilter("[CurrentBatteryId] IS NOT NULL");
            entity.HasOne<Battery>()
                .WithMany()
                .HasForeignKey(x => x.CurrentBatteryId)
                .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne<Admin>()
                .WithMany()
                .HasForeignKey(x => x.CreatedByAdminId)
                .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne<Employee>()
                .WithMany()
                .HasForeignKey(x => x.CreatedByEmployeeId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Station>(entity =>
        {
            entity.ToTable("stations");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.StationName).HasMaxLength(150).IsRequired();
            entity.Property(x => x.StationCode).HasMaxLength(30).IsRequired();
            entity.Property(x => x.Address).HasMaxLength(300);
            entity.Property(x => x.Latitude).HasPrecision(9, 6);
            entity.Property(x => x.Longitude).HasPrecision(9, 6);
            entity.Property(x => x.Status).HasMaxLength(20).IsRequired();
            entity.HasIndex(x => x.StationCode).IsUnique();
            entity.HasIndex(x => x.EmployeeId).IsUnique().HasFilter("[EmployeeId] IS NOT NULL");
            entity.HasOne<Employee>()
                .WithOne()
                .HasForeignKey<Station>(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Battery>(entity =>
        {
            entity.ToTable("batteries");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.BatteryCode).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(30).IsRequired();
            entity.Property(x => x.LastUpdated).HasDefaultValueSql("GETUTCDATE()");
            entity.HasIndex(x => x.BatteryCode).IsUnique();
            entity.HasIndex(x => x.CurrentClientId).IsUnique().HasFilter("[CurrentClientId] IS NOT NULL");
            entity.HasOne<Station>()
                .WithMany()
                .HasForeignKey(x => x.StationId)
                .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne<Client>()
                .WithMany()
                .HasForeignKey(x => x.CurrentClientId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Swap>(entity =>
        {
            entity.ToTable("swaps");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.SwapCost).HasPrecision(10, 2).HasDefaultValue(BatterySwapDefaults.SwapCost);
            entity.Property(x => x.SwapTime).HasDefaultValueSql("GETUTCDATE()");
            entity.HasOne<Client>()
                .WithMany()
                .HasForeignKey(x => x.ClientId)
                .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne<Station>()
                .WithMany()
                .HasForeignKey(x => x.StationId)
                .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne<Battery>()
                .WithMany()
                .HasForeignKey(x => x.ReturnedBatteryId)
                .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne<Battery>()
                .WithMany()
                .HasForeignKey(x => x.AssignedBatteryId)
                .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne<Employee>()
                .WithMany()
                .HasForeignKey(x => x.ProcessedByEmployeeId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<BalanceRecharge>(entity =>
        {
            entity.ToTable("balance_recharges");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Amount).HasPrecision(10, 2).IsRequired();
            entity.Property(x => x.RechargeTime).HasDefaultValueSql("GETUTCDATE()");
            entity.HasOne<Client>()
                .WithMany()
                .HasForeignKey(x => x.ClientId)
                .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne<Station>()
                .WithMany()
                .HasForeignKey(x => x.StationId)
                .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne<Admin>()
                .WithMany()
                .HasForeignKey(x => x.AddedByAdminId)
                .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne<Employee>()
                .WithMany()
                .HasForeignKey(x => x.AddedByEmployeeId)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }
}
