using Microsoft.AspNetCore.Identity;

namespace HotelManagementSystem.Models;

/// <summary>
/// 系统用户（Identity）
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
}
