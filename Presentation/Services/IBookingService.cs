using Presentation.Models;

namespace Presentation.Services
{
    public interface IBookingService
    {
        Task<BookingResult> CreateBookingasync(CreateBookingRequest req);
    }
}