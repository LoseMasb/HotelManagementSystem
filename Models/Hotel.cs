using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Models;

public class Hotel
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

    public bool IsEnabled { get; set; } = true;

    public string? ManagerUserId { get; set; }

    public ApplicationUser? ManagerUser { get; set; }

    public ICollection<RoomType> RoomTypes { get; set; } = new List<RoomType>();
}
