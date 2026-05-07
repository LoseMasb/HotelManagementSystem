using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Models;

public enum BookingStatus
{
    Pending = 0,
    Confirmed = 1,
    Cancelled = 2,
    Completed = 3
}

public class Booking
{
    public int Id { get; set; }

    public int HotelId { get; set; }
    public Hotel? Hotel { get; set; }

    [Required]
    public string CustomerUserId { get; set; } = string.Empty;
    public ApplicationUser? CustomerUser { get; set; }

    [DataType(DataType.Date)]
    public DateOnly CheckInDate { get; set; }

    [DataType(DataType.Date)]
    public DateOnly CheckOutDate { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(500)]
    public string? Remark { get; set; }

    public ICollection<BookingItem> Items { get; set; } = new List<BookingItem>();
}

public class BookingItem
{
    public int Id { get; set; }

    public int BookingId { get; set; }
    public Booking? Booking { get; set; }

    public int RoomTypeId { get; set; }
    public RoomType? RoomType { get; set; }

    [Range(1, 100)]
    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }
}
