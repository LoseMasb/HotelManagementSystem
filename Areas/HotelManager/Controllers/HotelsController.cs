using System.Security.Claims;
using HotelManagementSystem.Data;
using HotelManagementSystem.ViewModels.HotelManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Areas.HotelManager.Controllers;

[Area("HotelManager")]
[Authorize(Roles = SeedData.HotelManagerRole)]
public class HotelsController(ApplicationDbContext dbContext) : Controller
{
    public async Task<IActionResult> Edit()
    {
        var hotel = await GetMyHotelAsync();
        if (hotel is null) return View("NoHotel");

        var vm = new MyHotelEditVm
        {
            Id = hotel.Id,
            Name = hotel.Name,
            City = hotel.City,
            Address = hotel.Address,
            Description = hotel.Description
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(MyHotelEditVm vm)
    {
        var hotel = await GetMyHotelAsync();
        if (hotel is null) return View("NoHotel");

        if (!ModelState.IsValid) return View(vm);

        hotel.Name = vm.Name;
        hotel.City = vm.City;
        hotel.Address = vm.Address;
        hotel.Description = vm.Description;

        await dbContext.SaveChangesAsync();
        TempData["Success"] = "酒店信息已更新。";
        return RedirectToAction(nameof(Edit));
    }

    private Task<Models.Hotel?> GetMyHotelAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return dbContext.Hotels.FirstOrDefaultAsync(h => h.ManagerUserId == userId);
    }
}
