using Microsoft.EntityFrameworkCore;
using InvestmentTracker.API.Models;

namespace InvestmentTracker.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<AssetCategory> AssetCategories => Set<AssetCategory>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =========================
        // USERS
        // =========================
        modelBuilder.Entity<User>(eb =>
        {
            eb.ToTable("Users");

            eb.HasKey(u => u.Id);
            eb.Property(u => u.Id)
                .ValueGeneratedOnAdd();

            eb.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100);

            eb.HasIndex(u => u.Username)
                .IsUnique();

            eb.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);
        });

        // =========================
        // ASSET CATEGORIES
        // =========================
        modelBuilder.Entity<AssetCategory>(eb =>
        {
            eb.ToTable("AssetCategories");

            eb.HasKey(c => c.Id);
            eb.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            eb.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);

            eb.Property(c => c.IsActive)
                .HasDefaultValue(true);

            eb.Property(c => c.UserId)
                .IsRequired();

            eb.HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            eb.HasIndex(c => new { c.UserId, c.Name })
                .IsUnique();
        });

        // =========================
        // ASSETS
        // =========================
        modelBuilder.Entity<Asset>(eb =>
        {
            eb.ToTable("Assets");

            eb.HasKey(a => a.Id);
            eb.Property(a => a.Id)
                .ValueGeneratedOnAdd();

            eb.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(200);

            eb.Property(a => a.UserId)
                .IsRequired();

            eb.HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            eb.Property(a => a.CategoryId)
                .IsRequired();

            eb.HasOne(a => a.Category)
                .WithMany(c => c.Assets)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            eb.Property(a => a.Status)
                .IsRequired()
                .HasConversion<int>();

            eb.Property(a => a.ValueChangeCurrency)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("EUR");

            eb.Property(a => a.DividendCurrency)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("EUR");

            eb.HasIndex(a => new { a.UserId, a.Name })
                .IsUnique();

            eb.ToTable(tb =>
            {
                tb.HasCheckConstraint(
                    "CK_Asset_ValueChangeCurrency",
                    "ValueChangeCurrency IN ('EUR','USD')");

                tb.HasCheckConstraint(
                    "CK_Asset_DividendCurrency",
                    "DividendCurrency IN ('EUR','USD')");
            });
        });

        // =========================
        // TRANSACTIONS
        // =========================
        modelBuilder.Entity<Transaction>(eb =>
        {
            eb.ToTable("Transactions");

            eb.HasKey(t => t.Id);
            eb.Property(t => t.Id)
                .ValueGeneratedOnAdd();

            eb.Property(t => t.UserId)
                .IsRequired();

            eb.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            eb.Property(t => t.Date)
                .HasColumnType("datetime")
                .IsRequired();

            eb.Property(t => t.ValueChange)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0m);

            eb.Property(t => t.CurrentValue)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0m);

            eb.Property(t => t.Dividend)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0m);

            eb.Property(t => t.Notes)
                .HasMaxLength(500);

            eb.HasOne(t => t.Asset)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AssetId)
                .OnDelete(DeleteBehavior.Cascade);

            eb.HasIndex(t => new { t.UserId, t.AssetId, t.Date })
                .IsUnique();

            eb.ToTable(tb =>
            {
                tb.HasCheckConstraint(
                    "CK_Transaction_CurrentValue_NonNegative",
                    "CurrentValue >= 0");

                tb.HasCheckConstraint(
                    "CK_Transaction_Dividend_NonNegative",
                    "Dividend >= 0");
            });
        });
    }
}
