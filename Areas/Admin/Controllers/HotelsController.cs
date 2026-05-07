using HotelManagementSystem.Data;
using HotelManagementSystem.Models;
using HotelManagementSystem.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SeedData.AdminRole)]
public class HotelsController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager) : Controller
{
    public async Task<IActionResult> Index(string? search)
    {
        var query = dbContext.Hotels.Include(h => h.ManagerUser).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(h => h.Name.Contains(search) || h.City.Contains(search));
        }
        var list = await query.OrderByDescending(h => h.Id).ToListAsync();
        ViewBag.Search = search;
        return View(list);
    }

    public async Task<IActionResult> Create()
    {
        await LoadManagersAsync();
        return View("Edit", new HotelEditVm());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HotelEditVm vm)
    {
        if (!ModelState.IsValid)
        {
            await LoadManagersAsync(vm.ManagerUserId);
            return View("Edit", vm);
        }

        dbContext.Hotels.Add(new Hotel
        {
            Name = vm.Name,
            City = vm.City,
            Address = vm.Address,
            Description = vm.Description,
            IsEnabled = vm.IsEnabled,
            ManagerUserId = vm.ManagerUserId
        });
        await dbContext.SaveChangesAsync();
        TempData["Success"] = "酒店已创建。";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var hotel = await dbContext.Hotels.FindAsync(id);
        if (hotel is null) return NotFound();

        var vm = new HotelEditVm
        {
            Id = hotel.Id,
            Name = hotel.Name,
            City = hotel.City,
            Address = hotel.Address,
            Description = hotel.Description,
            IsEnabled = hotel.IsEnabled,
            ManagerUserId = hotel.ManagerUserId
        };
        await LoadManagersAsync(vm.ManagerUserId);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, HotelEditVm vm)
    {
        if (id != vm.Id) return BadRequest();
        var hotel = await dbContext.Hotels.FindAsync(id);
        if (hotel is null) return NotFound();

        if (!ModelState.IsValid)
        {
            await LoadManagersAsync(vm.ManagerUserId);
            return View(vm);
        }

        hotel.Name = vm.Name;
        hotel.City = vm.City;
        hotel.Address = vm.Address;
        hotel.Description = vm.Description;
        hotel.IsEnabled = vm.IsEnabled;
        hotel.ManagerUserId = vm.ManagerUserId;
        await dbContext.SaveChangesAsync();

        TempData["Success"] = "酒店信息已更新。";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var hotel = await dbContext.Hotels.FindAsync(id);
        if (hotel is null) return NotFound();

        dbContext.Hotels.Remove(hotel);
        await dbContext.SaveChangesAsync();

        TempData["Success"] = "酒店已删除。";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadManagersAsync(string? selected = null)
    {
        var users = await userManager.GetUsersInRoleAsync(SeedData.HotelManagerRole);
        ViewBag.Managers = users.Select(u => new ManagerOptionVm { UserId = u.Id, Email = u.Email! }).ToList();
        ViewBag.SelectedManager = selected;
    }
}
