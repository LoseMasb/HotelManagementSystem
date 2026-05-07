using HotelManagementSystem.Data;
using HotelManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Services;

public class BookingService(ApplicationDbContext dbContext) : IBookingService
{
    public async Task<int> GetAvailableQuantityAsync(int roomTypeId, DateOnly checkIn, DateOnly checkOut)
    {
        var roomType = await dbContext.RoomTypes.AsNoTracking().FirstOrDefaultAsync(r => r.Id == roomTypeId && r.IsActive && r.Hotel!.IsEnabled);
        if (roomType is null || checkOut <= checkIn)
        {
            return 0;
        }

        var booked = await dbContext.BookingItems
            .Where(i => i.RoomTypeId == roomTypeId)
            .Where(i => i.Booking != null && i.Booking.Status != BookingStatus.Cancelled)
            .Where(i => i.Booking!.CheckInDate < checkOut && i.Booking.CheckOutDate > checkIn)
            .SumAsync(i => (int?)i.Quantity) ?? 0;

        return Math.Max(0, roomType.Inventory - booked);
    }

    public async Task<(bool Success, string Message, int BookingId)> CreateBookingAsync(string customerUserId, int roomTypeId, DateOnly checkIn, DateOnly checkOut, int quantity, string? remark)
    {
        if (checkOut <= checkIn)
        {
            return (false, "离店日期必须大于入住日期。", 0);
        }

        if (quantity <= 0)
        {
            return (false, "预订数量必须大于 0。", 0);
        }

        var roomType = await dbContext.RoomTypes.Include(r => r.Hotel).FirstOrDefaultAsync(r => r.Id == roomTypeId);
        if (roomType is null || roomType.Hotel is null || !roomType.IsActive || !roomType.Hotel.IsEnabled)
        {
            return (false, "房型不可预订。", 0);
        }

        var available = await GetAvailableQuantityAsync(roomTypeId, checkIn, checkOut);
        if (available < quantity)
        {
            return (false, $"库存不足，当前可预订 {available} 间。", 0);
        }

        var booking = new Booking
        {
            HotelId = roomType.HotelId,
            CustomerUserId = customerUserId,
            CheckInDate = checkIn,
            CheckOutDate = checkOut,
            Remark = remark,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            Items =
            [
                new BookingItem
                {
                    RoomTypeId = roomTypeId,
                    Quantity = quantity,
                    UnitPrice = roomType.PricePerNight
                }
            ]
        };

        dbContext.Bookings.Add(booking);
        await dbContext.SaveChangesAsync();

        return (true, "预订提交成功。", booking.Id);
    }
}
