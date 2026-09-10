using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RekeszAppBackend.Domain;

namespace RekeszAppBackend.Data;

public class AppDbContext : DbContext
{
    private readonly IHttpContextAccessor? httpContextAccessor;

    public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor? httpContextAccessor = null)
        : base(options)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Partner> Partnerek => Set<Partner>();
    public DbSet<Vevo> Vevek => Set<Vevo>();
    public DbSet<Zoldseg> Zoldsegek => Set<Zoldseg>();
    public DbSet<RekeszTipus> RekeszTipusok => Set<RekeszTipus>();
    public DbSet<FelvasarlasTetel> FelvasarlasTetelek => Set<FelvasarlasTetel>();
    public DbSet<EladasTetel> EladasTetelek => Set<EladasTetel>();

    private int? CurrentUserId
    {
        get
        {
            var value = httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(value, out var id) ? id : null;
        }
    }

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>().HasIndex(x => x.Email).IsUnique();
        b.Entity<Partner>().HasIndex(x => new { x.UserId, x.Nev }).IsUnique();
        b.Entity<Zoldseg>().HasIndex(x => new { x.UserId, x.Nev }).IsUnique();
        b.Entity<RekeszTipus>().HasIndex(x => new { x.UserId, x.Nev }).IsUnique();

        b.Entity<Zoldseg>()
            .HasOne(x => x.AlapertelmezettRekeszTipus).WithMany().HasForeignKey(x => x.AlapertelmezettRekeszTipusId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<FelvasarlasTetel>().HasIndex(x => x.Datum);
        b.Entity<FelvasarlasTetel>().HasIndex(x => x.UserId);
        b.Entity<FelvasarlasTetel>()
            .HasOne(x => x.Partner).WithMany().HasForeignKey(x => x.PartnerId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<FelvasarlasTetel>()
            .HasOne(x => x.Zoldseg).WithMany().HasForeignKey(x => x.ZoldsegId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<FelvasarlasTetel>()
            .HasOne(x => x.RekeszTipus).WithMany().HasForeignKey(x => x.RekeszTipusId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<FelvasarlasTetel>().Property(x => x.Egysegar).HasPrecision(10, 2);

        b.Entity<EladasTetel>().HasIndex(x => x.Datum);
        b.Entity<EladasTetel>().HasIndex(x => x.UserId);
        b.Entity<EladasTetel>()
            .HasOne(x => x.Vevo).WithMany().HasForeignKey(x => x.VevoId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<EladasTetel>()
            .HasOne(x => x.Zoldseg).WithMany().HasForeignKey(x => x.ZoldsegId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<EladasTetel>()
            .HasOne(x => x.RekeszTipus).WithMany().HasForeignKey(x => x.RekeszTipusId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<EladasTetel>().Property(x => x.Egysegar).HasPrecision(10, 2);

        b.Entity<Partner>().HasQueryFilter(x => CurrentUserId != null && x.UserId == CurrentUserId);
        b.Entity<Vevo>().HasQueryFilter(x => CurrentUserId != null && x.UserId == CurrentUserId);
        b.Entity<Zoldseg>().HasQueryFilter(x => CurrentUserId != null && x.UserId == CurrentUserId);
        b.Entity<RekeszTipus>().HasQueryFilter(x => CurrentUserId != null && x.UserId == CurrentUserId);
        b.Entity<FelvasarlasTetel>().HasQueryFilter(x => CurrentUserId != null && x.UserId == CurrentUserId);
        b.Entity<EladasTetel>().HasQueryFilter(x => CurrentUserId != null && x.UserId == CurrentUserId);
    }

    public override int SaveChanges()
    {
        ApplyUserOwnership();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyUserOwnership();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyUserOwnership();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ApplyUserOwnership();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ApplyUserOwnership()
    {
        var userId = CurrentUserId;
        if (userId is null) return;

        foreach (var entry in ChangeTracker.Entries<IFelhasznaloTulajdona>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.UserId = userId.Value;
            }
            else if (entry.State is EntityState.Modified or EntityState.Deleted)
            {
                if (entry.Entity.UserId != userId.Value)
                    throw new UnauthorizedAccessException("A rekord nem tartozik a bejelentkezett felhasználóhoz.");

                if (entry.State == EntityState.Modified)
                    entry.Property(nameof(IFelhasznaloTulajdona.UserId)).IsModified = false;
            }
        }
    }
}
