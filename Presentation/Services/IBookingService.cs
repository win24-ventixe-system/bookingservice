using Presentation.Models;

namespace Presentation.Services
{
    public interface IBookingService
    {
        Task<BookingResult<Booking>> CreateBookingAsync(CreateBookingRequest req);

        Task<BookingResult<IEnumerable<Booking>>> GetBookingsAsync();

        Task<BookingResult<IEnumerable<Booking>>> GetBookingsForUserAsync(string email);
    }
}