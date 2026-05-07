using HotelManagementSystem.Data;
using HotelManagementSystem.Services;
using HotelManagementSystem.ViewModels.Customer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Controllers;

public class HotelsController(ApplicationDbContext dbContext, IBookingService bookingService) : Controller
{
    public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 8)
    {
        var query = dbContext.Hotels.AsNoTracking().Where(h => h.IsEnabled);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(h => h.Name.Contains(search) || h.City.Contains(search));
        }

        var total = await query.CountAsync();
        var hotels = await query.OrderBy(h => h.City).ThenBy(h => h.Name)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(h => new HotelListItemVm { Id = h.Id, Name = h.Name, City = h.City, Address = h.Address })
            .ToListAsync();

        var vm = new HotelListVm
        {
            Search = search,
            Page = page,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize),
            Hotels = hotels
        };

        return View(vm);
    }

    public async Task<IActionResult> Details(int id, DateOnly? checkInDate, DateOnly? checkOutDate)
    {
        var hotel = await dbContext.Hotels.AsNoTracking()
            .Include(h => h.RoomTypes.Where(r => r.IsActive))
            .FirstOrDefaultAsync(h => h.Id == id && h.IsEnabled);

        if (hotel is null) return NotFound();

        var roomTypeVms = new List<RoomTypeAvailabilityVm>();
        foreach (var rt in hotel.RoomTypes.OrderBy(r => r.Name))
        {
            var available = rt.Inventory;
            if (checkInDate.HasValue && checkOutDate.HasValue)
            {
                available = await bookingService.GetAvailableQuantityAsync(rt.Id, checkInDate.Value, checkOutDate.Value);
            }

            roomTypeVms.Add(new RoomTypeAvailabilityVm
            {
                RoomTypeId = rt.Id,
                RoomTypeName = rt.Name,
                PricePerNight = rt.PricePerNight,
                Inventory = rt.Inventory,
                AvailableQuantity = available
            });
        }

        return View(new HotelDetailsVm
        {
            Hotel = hotel,
            CheckInDate = checkInDate,
            CheckOutDate = checkOutDate,
            RoomTypes = roomTypeVms
        });
    }
}
