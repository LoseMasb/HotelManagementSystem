using System.Security.Claims;
using HotelManagementSystem.Data;
using HotelManagementSystem.Models;
using HotelManagementSystem.ViewModels.HotelManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Areas.HotelManager.Controllers;

[Area("HotelManager")]
[Authorize(Roles = SeedData.HotelManagerRole)]
public class OrdersController(ApplicationDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index()
    {
        var hotelId = await GetMyHotelIdAsync();
        if (hotelId == 0) return View("~/Areas/HotelManager/Views/Hotels/NoHotel.cshtml");

        var list = await dbContext.Bookings.AsNoTracking()
            .Where(b => b.HotelId == hotelId)
            .OrderByDescending(b => b.CreatedAt)
            .SelectMany(b => b.Items.Select(i => new ManagerOrderVm
            {
                BookingId = b.Id,
                CustomerEmail = b.CustomerUser!.Email!,
                RoomTypeName = i.RoomType!.Name,
                CheckInDate = b.CheckInDate,
                CheckOutDate = b.CheckOutDate,
                Quantity = i.Quantity,
                Status = b.Status,
                CreatedAt = b.CreatedAt
            }))
            .ToListAsync();

        return View(list);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int bookingId, BookingStatus status)
    {
        var hotelId = await GetMyHotelIdAsync();
        var booking = await dbContext.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId && b.HotelId == hotelId);
        if (booking is null) return NotFound();

        booking.Status = status;
        await dbContext.SaveChangesAsync();
        TempData["Success"] = "订单状态已更新。";
        return RedirectToAction(nameof(Index));
    }

    private async Task<int> GetMyHotelIdAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return await dbContext.Hotels.Where(h => h.ManagerUserId == userId).Select(h => h.Id).FirstOrDefaultAsync();
    }
}
