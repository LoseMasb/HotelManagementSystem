using System.ComponentModel.DataAnnotations;
using HotelManagementSystem.Models;

namespace HotelManagementSystem.ViewModels.HotelManager;

public class MyHotelEditVm
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string City { get; set; } = string.Empty;

    [Required, StringLength(300)]
    public string Address { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }
}

public class RoomTypeEditVm
{
    public int? Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(0, 99999)]
    public decimal PricePerNight { get; set; }

    [Range(0, 10000)]
    public int Inventory { get; set; }

    public bool IsActive { get; set; } = true;
}

public class ManagerOrderVm
{
    public int BookingId { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public string RoomTypeName { get; set; } = string.Empty;
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int Quantity { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
