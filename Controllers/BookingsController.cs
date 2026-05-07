using System.Security.Claims;
using HotelManagementSystem.Data;
using HotelManagementSystem.Models;
using HotelManagementSystem.Services;
using HotelManagementSystem.ViewModels.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Controllers;

[Authorize(Roles = SeedData.CustomerRole)]
public class BookingsController(ApplicationDbContext dbContext, IBookingService bookingService) : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBookingVm vm)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "请检查输入信息。";
            return RedirectToAction("Details", "Hotels", new { id = await GetHotelId(vm.RoomTypeId) });
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await bookingService.CreateBookingAsync(userId, vm.RoomTypeId, vm.CheckInDate, vm.CheckOutDate, vm.Quantity, vm.Remark);

        if (!result.Success)
        {
            TempData["Error"] = result.Message;
            return RedirectToAction("Details", "Hotels", new { id = await GetHotelId(vm.RoomTypeId), checkInDate = vm.CheckInDate, checkOutDate = vm.CheckOutDate });
        }

        TempData["Success"] = "预订成功，请在我的订单查看状态。";
        return RedirectToAction(nameof(MyOrders));
    }

    public async Task<IActionResult> MyOrders()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var list = await dbContext.Bookings.AsNoTracking()
            .Where(b => b.CustomerUserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .SelectMany(b => b.Items.Select(i => new MyBookingItemVm
            {
                BookingId = b.Id,
                HotelName = b.Hotel!.Name,
                RoomTypeName = i.RoomType!.Name,
                CheckInDate = b.CheckInDate,
                CheckOutDate = b.CheckOutDate,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Status = b.Status,
                CreatedAt = b.CreatedAt
            }))
            .ToListAsync();

        return View(list);
    }

    private async Task<int> GetHotelId(int roomTypeId)
    {
        return await dbContext.RoomTypes.Where(r => r.Id == roomTypeId).Select(r => r.HotelId).FirstOrDefaultAsync();
    }
}
