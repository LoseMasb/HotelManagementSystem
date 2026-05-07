using System.ComponentModel.DataAnnotations;
using HotelManagementSystem.Models;

namespace HotelManagementSystem.ViewModels.Customer;

public class HotelListItemVm
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

public class HotelListVm
{
    public string? Search { get; set; }
    public int Page { get; set; }
    public int TotalPages { get; set; }
    public List<HotelListItemVm> Hotels { get; set; } = [];
}

public class RoomTypeAvailabilityVm
{
    public int RoomTypeId { get; set; }
    public string RoomTypeName { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int Inventory { get; set; }
    public int AvailableQuantity { get; set; }
}

public class HotelDetailsVm
{
    public Hotel Hotel { get; set; } = new();
    public DateOnly? CheckInDate { get; set; }
    public DateOnly? CheckOutDate { get; set; }
    public List<RoomTypeAvailabilityVm> RoomTypes { get; set; } = [];
}

public class CreateBookingVm
{
    [Required]
    public int RoomTypeId { get; set; }

    [Required, DataType(DataType.Date)]
    public DateOnly CheckInDate { get; set; }

    [Required, DataType(DataType.Date)]
    public DateOnly CheckOutDate { get; set; }

    [Range(1, 20)]
    public int Quantity { get; set; } = 1;

    [StringLength(500)]
    public string? Remark { get; set; }
}

public class MyBookingItemVm
{
    public int BookingId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string RoomTypeName { get; set; } = string.Empty;
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
