using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Presentation.Data.Entities;
using Presentation.Data.Repositories;
using Presentation.Models;

namespace Presentation.Services;

public class BookingService(IBookingRepository bookingRepository, IEventServiceClient eventClient, IEmailService emailService) : IBookingService
{
    private readonly IBookingRepository _bookingRepository = bookingRepository;
    private readonly IEventServiceClient _eventClient = eventClient;
    private readonly IEmailService _emailService = emailService;

    public async Task<BookingResult> CreateBookingAsync(CreateBookingRequest req)
    {
        try
        {
            var bookingEntity = new BookingEntity
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

            if (!result.Success)
                return new BookingResult { Success = false, Error = result.Error };

            // Get Event Info from external service
            var eventData = await _eventClient.GetEventByIdAsync(req.EventId);

            // Create email model from saved booking + event info
            var emailModel = new BookingConfirmationEmail
            {
                Id = bookingEntity.Id.ToString(), // This is the booking reference
                FirstName = req.FirstName,
                Email = req.Email,
                EventTitle = eventData?.Title ?? "Unknown Event",
                EventDate = eventData?.EventDate ?? DateTime.Now,
                TotalPrice = eventData?.Packages?.FirstOrDefault()?.Price ?? 0,
                BookingDate = req.BookingDate,
                TicketQuantity = req.TicketQuantity.ToString()
            };

            // Send confirmation email (assuming EmailService is injected)
            await _emailService.SendBookingConfirmation(emailModel);

            return new BookingResult { Success = true };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[BookingService] Error creating booking: {ex.Message}");
            Console.WriteLine(ex.StackTrace);

            return new BookingResult { Success = false, Error = "Internal server error" };
        }

    }

    public async Task<BookingResult<IEnumerable<Booking>>> GetBookingsAsync()
    {

        var result = await _bookingRepository.GetAllAsync();
        if (result.Result == null)
        {
            return new BookingResult<IEnumerable<Booking>>() { Success = true, Result = new List<Booking>() };
        }
        var bookings = new List<Booking>();

        foreach (var booking in result.Result)
        {
            var eventData = await _eventClient.GetEventByIdAsync(booking.EventId);

            bookings.Add(new Booking
            {
                Id = booking.Id,
                EventId = booking.EventId,
                FirstName = booking.BookingOwner!.FirstName,
                LastName = booking.BookingOwner!.LastName,
                Email = booking.BookingOwner!.Email,
                StreetName = booking.BookingOwner.Address!.StreetName,
                PostalCode = booking.BookingOwner.Address.PostalCode,
                City = booking.BookingOwner.Address.City,
                PackageOption = eventData?.Packages.FirstOrDefault()?.Title!,
                BookingDate = booking.BookingDate,
                TicketQuantity = booking.TicketQuantity
            });
        }
        return new BookingResult<IEnumerable<Booking>>() { Success = true, Result = bookings };
    }

    public async Task<BookingResult<IEnumerable<Booking>>> GetBookingsForUserAsync(string email)
    {
        var result = await _bookingRepository.GetAllAsync();
        if (result.Result == null)
        {
            return new BookingResult<IEnumerable<Booking>>() { Success = true, Result = new List<Booking>() };
        }

        var userBookings = result.Result
            .Where(b => b.BookingOwner != null && b.BookingOwner.Email == email)
            .ToList();

        var bookings = new List<Booking>();
        foreach (var booking in userBookings)
        {
            var eventData = await _eventClient.GetEventByIdAsync(booking.EventId);

            bookings.Add(new Booking
            {
                Id = booking.Id,
                EventId = booking.EventId,
                FirstName = booking.BookingOwner!.FirstName,
                LastName = booking.BookingOwner!.LastName,
                Email = booking.BookingOwner!.Email,
                StreetName = booking.BookingOwner.Address!.StreetName,
                PostalCode = booking.BookingOwner.Address.PostalCode,
                City = booking.BookingOwner.Address.City,
                PackageOption = eventData?.Packages.FirstOrDefault()?.Title ?? "Unknown",
                BookingDate = booking.BookingDate,
                TicketQuantity = booking.TicketQuantity
            });
        }

        return new BookingResult<IEnumerable<Booking>>() { Success = true, Result = bookings };
    }


}
