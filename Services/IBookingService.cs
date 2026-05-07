using HotelManagementSystem.Models;

namespace HotelManagementSystem.Services;

public interface IBookingService
{
    Task<int> GetAvailableQuantityAsync(int roomTypeId, DateOnly checkIn, DateOnly checkOut);
    Task<(bool Success, string Message, int BookingId)> CreateBookingAsync(string customerUserId, int roomTypeId, DateOnly checkIn, DateOnly checkOut, int quantity, string? remark);
}
