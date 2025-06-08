using Presentation.Models;

namespace Presentation.Services
{
    public interface IBookingService
    {
        Task<BookingResult> CreateBookingAsync(CreateBookingRequest req);

        Task<BookingResult<IEnumerable<Booking>>> GetBookingsAsync();
    }
}