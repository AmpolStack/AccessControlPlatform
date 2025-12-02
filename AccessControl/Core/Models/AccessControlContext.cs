using AccessControl.Infraestructure.Dto;
using Microsoft.EntityFrameworkCore;

namespace AccessControl.Core.Models;

public partial class AccessControlContext : DbContext
{
    public AccessControlContext()
    {
    }

    public AccessControlContext(DbContextOptions<AccessControlContext> options)
        : base(options)
    {
    }

    public DbSet<UserLoginDataDto> UserLoginDataDto { get; set; }
    public DbSet<CurrentCapacityDto> CurrentCapacityDto { get; set; }
    public DbSet<UserAccessHistoryDto> UserAccessHistoryDto { get; set; }
    public DbSet<HourlyAverageDto> HourlyAverageDto { get; set; }

    public DbSet<CreateUserResultDto> CreateUserResultDto { get; set; }
    public virtual DbSet<AccessRecord> AccessRecords { get; set; }

    public virtual DbSet<Establishment> Establishments { get; set; }

    public virtual DbSet<EstablishmentOpening> EstablishmentOpenings { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserLoginDataDto>(entity =>
        {
            entity.HasNoKey();
            entity.Property(e => e.Id);
            entity.Property(e => e.Email);
            entity.Property(e => e.FullName);
            entity.Property(e => e.EstablishmentId);
            entity.Property(e => e.Role);
            entity.Property(e => e.IsActive);
            entity.Property(e => e.IdentityDocument);
            entity.Property(e => e.PhoneNumber);
            entity.Property(e => e.MustChangePassword);
            entity.Property(e => e.CreatedDate);
            entity.Property(e => e.LastLoginDate);
            entity.Property(e => e.EstablishmentName);
            entity.Property(e => e.MaxCapacity);
        });

        modelBuilder.Entity<CreateUserResultDto>().HasNoKey();

        modelBuilder.Entity<CurrentCapacityDto>().HasNoKey();
        modelBuilder.Entity<UserAccessHistoryDto>().HasNoKey();
        modelBuilder.Entity<HourlyAverageDto>().HasNoKey();

        modelBuilder.Entity<AccessRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AccessRe__3214EC077D21DE8E");

            entity.ToTable(tb => tb.HasTrigger("tr_ValidateCapacity"));

            entity.HasOne(d => d.Establishment).WithMany(p => p.AccessRecords)
                .HasForeignKey(d => d.EstablishmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AccessRec__Estab__440B1D61");

            entity.HasOne(d => d.User).WithMany(p => p.AccessRecords)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AccessRec__UserI__4316F928");
        });

        modelBuilder.Entity<Establishment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Establis__3214EC07A32224C2");

            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
        });

        modelBuilder.Entity<EstablishmentOpening>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Establis__3214EC07B247B8DE");

            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Open");

            entity.HasOne(d => d.Establishment).WithMany(p => p.EstablishmentOpenings)
                .HasForeignKey(d => d.EstablishmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Establish__Estab__48CFD27E");

            entity.HasOne(d => d.User).WithMany(p => p.EstablishmentOpenings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Establish__UserI__49C3F6B7");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07FE934630");

            entity.ToTable(tb =>
                {
                    tb.HasTrigger("tr_PreventDuplicateIdentityDocument");
                    tb.HasTrigger("tr_UserAudit");
                });

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105346412FB56").IsUnique();

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IdentityDocument).HasMaxLength(20);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MustChangePassword).HasDefaultValue(true);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Role).HasMaxLength(20);

            entity.HasOne(d => d.Establishment).WithMany(p => p.Users)
                .HasForeignKey(d => d.EstablishmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__Establish__403A8C7D");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
