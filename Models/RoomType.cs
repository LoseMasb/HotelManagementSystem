using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagementSystem.Models;

public class RoomType
{
    public int Id { get; set; }

    public int HotelId { get; set; }
    public Hotel? Hotel { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(0, 99999)]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PricePerNight { get; set; }

    [Range(0, 10000)]
    public int Inventory { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();
}
