using Microsoft.EntityFrameworkCore;
using SolidMReader.Data.Entities;

namespace SolidMReader.Data.Context;

public partial class SolidMReaderContext : DbContext
{
    public SolidMReaderContext()
    {
    }

    public SolidMReaderContext(DbContextOptions<SolidMReaderContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<MeterReading> MeterReadings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.Property(e => e.AccountId).ValueGeneratedNever();
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
        });

        modelBuilder.Entity<MeterReading>(entity =>
        {
            entity.HasKey(e => e.MeterReadingGuid);

            entity.Property(e => e.MeterReadingGuid).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Account).WithMany(p => p.MeterReadings)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MeterReadings_Accounts");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
