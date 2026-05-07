using HotelManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<RoomType> RoomTypes => Set<RoomType>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingItem> BookingItems => Set<BookingItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Hotel>()
            .HasOne(h => h.ManagerUser)
            .WithMany()
            .HasForeignKey(h => h.ManagerUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<RoomType>()
            .HasIndex(r => new { r.HotelId, r.Name })
            .IsUnique();

        builder.Entity<Booking>()
            .Property(b => b.Status)
            .HasConversion<string>();

        builder.Entity<Booking>()
            .HasMany(b => b.Items)
            .WithOne(i => i.Booking)
            .HasForeignKey(i => i.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<BookingItem>()
            .Property(i => i.UnitPrice)
            .HasPrecision(18, 2);

        builder.Entity<BookingItem>()
            .HasOne(i => i.RoomType)
            .WithMany(r => r.BookingItems)
            .HasForeignKey(i => i.RoomTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
