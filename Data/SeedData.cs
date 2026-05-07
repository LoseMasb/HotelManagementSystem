using HotelManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Data;

public static class SeedData
{
    public const string AdminRole = "Admin";
    public const string HotelManagerRole = "HotelManager";
    public const string CustomerRole = "Customer";

    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await db.Database.MigrateAsync();

        foreach (var role in new[] { AdminRole, HotelManagerRole, CustomerRole })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var admin = await EnsureUserAsync(userManager, "admin@hotel.local", "Admin@123", "系统管理员", AdminRole);
        var manager = await EnsureUserAsync(userManager, "manager@hotel.local", "Manager@123", "示例酒店经理", HotelManagerRole);
        _ = await EnsureUserAsync(userManager, "customer@hotel.local", "Customer@123", "示例客户", CustomerRole);

        if (!await db.Hotels.AnyAsync())
        {
            var hotel = new Hotel
            {
                Name = "星河商务酒店",
                City = "上海",
                Address = "浦东新区世纪大道 100 号",
                Description = "交通便利，近地铁，适合商务出行。",
                IsEnabled = true,
                ManagerUserId = manager.Id,
                RoomTypes =
                [
                    new RoomType { Name = "标准大床房", Inventory = 20, PricePerNight = 399, IsActive = true },
                    new RoomType { Name = "豪华双床房", Inventory = 15, PricePerNight = 499, IsActive = true }
                ]
            };

            db.Hotels.Add(hotel);
            await db.SaveChangesAsync();
        }
    }

    private static async Task<ApplicationUser> EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string displayName,
        string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                DisplayName = displayName
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"创建默认用户失败: {email}, {string.Join(",", result.Errors.Select(e => e.Description))}");
            }
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            await userManager.AddToRoleAsync(user, role);
        }

        return user;
    }
}
