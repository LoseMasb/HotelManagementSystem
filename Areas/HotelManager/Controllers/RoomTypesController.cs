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
public class RoomTypesController(ApplicationDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index()
    {
        var hotel = await GetMyHotelAsync();
        if (hotel is null) return View("~/Areas/HotelManager/Views/Hotels/NoHotel.cshtml");

        var list = await dbContext.RoomTypes.Where(r => r.HotelId == hotel.Id).OrderBy(r => r.Name).ToListAsync();
        return View(list);
    }

    public IActionResult Create() => View("Edit", new RoomTypeEditVm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RoomTypeEditVm vm)
    {
        var hotel = await GetMyHotelAsync();
        if (hotel is null) return View("~/Areas/HotelManager/Views/Hotels/NoHotel.cshtml");
        if (!ModelState.IsValid) return View("Edit", vm);

        dbContext.RoomTypes.Add(new RoomType
        {
            HotelId = hotel.Id,
            Name = vm.Name,
            PricePerNight = vm.PricePerNight,
            Inventory = vm.Inventory,
            IsActive = vm.IsActive
        });
        await dbContext.SaveChangesAsync();
        TempData["Success"] = "房型已新增。";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var roomType = await GetMyRoomTypeAsync(id);
        if (roomType is null) return NotFound();

        return View(new RoomTypeEditVm
        {
            Id = roomType.Id,
            Name = roomType.Name,
            PricePerNight = roomType.PricePerNight,
            Inventory = roomType.Inventory,
            IsActive = roomType.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RoomTypeEditVm vm)
    {
        var roomType = await GetMyRoomTypeAsync(id);
        if (roomType is null) return NotFound();
        if (!ModelState.IsValid) return View(vm);

        roomType.Name = vm.Name;
        roomType.PricePerNight = vm.PricePerNight;
        roomType.Inventory = vm.Inventory;
        roomType.IsActive = vm.IsActive;
        await dbContext.SaveChangesAsync();

        TempData["Success"] = "房型已更新。";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var roomType = await GetMyRoomTypeAsync(id);
        if (roomType is null) return NotFound();

        dbContext.RoomTypes.Remove(roomType);
        await dbContext.SaveChangesAsync();
        TempData["Success"] = "房型已删除。";
        return RedirectToAction(nameof(Index));
    }

    private async Task<Hotel?> GetMyHotelAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return await dbContext.Hotels.FirstOrDefaultAsync(h => h.ManagerUserId == userId);
    }

    private async Task<RoomType?> GetMyRoomTypeAsync(int roomTypeId)
    {
        var hotel = await GetMyHotelAsync();
        if (hotel is null) return null;
        return await dbContext.RoomTypes.FirstOrDefaultAsync(r => r.Id == roomTypeId && r.HotelId == hotel.Id);
    }
}
