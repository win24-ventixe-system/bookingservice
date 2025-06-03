using Presentation.Data.Entities;
using Presentation.Data.Repositories;
using Presentation.Models;

namespace Presentation.Services;

public class BookingService(IBookingRepository bookingRepository) : IBookingService
{
    private readonly IBookingRepository _bookingRepository = bookingRepository;


    public async Task<BookingResult> CreateBookingasync(CreateBookingRequest req)
    {
        var bookingEntity = new Data.Entities.BookingEntity
        {
            EventId = req.EventId,
            BookingDate = DateTime.Now,
            TicketQuantity = req.TicketQuantity,
            BookingOwner = new BookingOwnerEntity
            {
                FirstName = req.FirstName,
                LastName = req.LastName,
                Email = req.Email,
                Address = new BookingAddressEntity
                {
                    StreetName = req.StreetName,
                    PostalCode = req.PostalCode,
                    City = req.City,
                }
            }
        };

        var result = await _bookingRepository.AddAsync(bookingEntity);
        return result.Success
            ? new BookingResult { Success = true }
            : new BookingResult { Success = false, Error = result.Error };
    }

}
