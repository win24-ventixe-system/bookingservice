using Azure.Core;
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

    public async Task<BookingResult<Booking>> CreateBookingAsync(CreateBookingRequest req)
    {
        try
        {
            // Get Event Info from external service
            var eventData = await _eventClient.GetEventByIdAsync(req.EventId);
            if (eventData == null)
            {
                return new BookingResult<Booking> { Success = false, Error = "Event not found or could not be retrieved." };
            }

            //  use PackageId to find the package
            decimal chosenPackagePrice = 0;
            string chosenPackageTitle = "Unknown package";
            var selectedPackage = eventData!.Packages?.FirstOrDefault(p => p.Id.ToString().Equals(req.PackageId));
            if (selectedPackage != null) 
            {
                chosenPackagePrice = selectedPackage.Price;
                chosenPackageTitle = selectedPackage.Title;
            }
            
         
            decimal calculatedTotalPrice = chosenPackagePrice * req.TicketQuantity;
            var bookingEntity = new BookingEntity
            {
                EventId = req.EventId,
                BookingDate = DateTime.Now,
                PackageId = req.PackageId,
                TicketQuantity = req.TicketQuantity,
                // Populate Event details from fetched eventData
                EventTitle = eventData.Title!,
                EventDate = eventData.EventDate,
                EventLocation = eventData.Location!,
                EventPrice = chosenPackagePrice,
                TotalPrice = calculatedTotalPrice,

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
                return new BookingResult<Booking> { Success = false, Error = result.Error };


            // Populate your Booking DTO with all necessary details
            var bookingDto = new Booking
            {
                Id = bookingEntity.Id.ToString(),
                EventId = bookingEntity.EventId.ToString(),
                FirstName = bookingEntity.BookingOwner?.FirstName!,
                LastName = bookingEntity.BookingOwner?.LastName!,
                Email = bookingEntity.BookingOwner?.Email!,
                StreetName = bookingEntity.BookingOwner?.Address?.StreetName!,
                PostalCode = bookingEntity.BookingOwner?.Address?.PostalCode!,
                City = bookingEntity.BookingOwner?.Address?.City!,
                PackageId = bookingEntity.PackageId,
                BookingDate = bookingEntity.BookingDate,
                TicketQuantity = bookingEntity.TicketQuantity,
                EventTitle = bookingEntity.EventTitle,        
                EventDate = bookingEntity.EventDate,          
                EventLocation = bookingEntity.EventLocation,  
                EventPrice = bookingEntity.EventPrice,
                TotalPrice = bookingEntity.TotalPrice,
            };
            
         
            // Create email model from saved booking + event info
            var emailModel = new BookingConfirmationEmail
            {
                Id = bookingEntity.Id.ToString(),
                FirstName = req.FirstName,
                Email = req.Email,
                EventTitle = eventData?.Title ?? "Unknown Event",
                EventDate = eventData?.EventDate ?? DateTime.Now,
                BookingDate = bookingEntity.BookingDate, // Fixed: Use actual booking date
                TicketQuantity = req.TicketQuantity.ToString(),
                TotalPrice = bookingEntity.TotalPrice,
            };

            // Send confirmation email
            await _emailService.SendBookingConfirmation(emailModel);
            return new BookingResult<Booking> { Success = true, Result = bookingDto };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[BookingService] Error creating booking: {ex.Message}");
            return new BookingResult<Booking> { Success = false, Error = "Internal server error" };
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

        foreach (var bookingEntity in result.Result)
        {
            bookings.Add(new Booking
            {
                Id = bookingEntity.Id,
                EventId = bookingEntity.EventId,
                FirstName = bookingEntity.BookingOwner?.FirstName!,
                LastName = bookingEntity.BookingOwner?.LastName!,
                Email = bookingEntity.BookingOwner?.Email!,
                StreetName = bookingEntity.BookingOwner?.Address?.StreetName!,
                PostalCode = bookingEntity.BookingOwner?.Address?.PostalCode!,
                City = bookingEntity.BookingOwner?.Address?.City!,
                PackageId = bookingEntity.PackageId,
                BookingDate = bookingEntity.BookingDate,
                TicketQuantity = bookingEntity.TicketQuantity,
                // Event details
                EventTitle = bookingEntity.EventTitle,
                EventDate = bookingEntity.EventDate,
                EventLocation = bookingEntity.EventLocation,
                EventPrice = bookingEntity.EventPrice,
                TotalPrice = bookingEntity.TotalPrice
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
        foreach (var bookingEntity in userBookings)
        {
            
                bookings.Add(new Booking
                {
                    Id = bookingEntity.Id,
                    EventId = bookingEntity.EventId,
                    FirstName = bookingEntity.BookingOwner!.FirstName,
                    LastName = bookingEntity.BookingOwner!.LastName,
                    Email = bookingEntity.BookingOwner!.Email,
                    StreetName = bookingEntity.BookingOwner.Address!.StreetName,
                    PostalCode = bookingEntity.BookingOwner.Address.PostalCode,
                    City = bookingEntity.BookingOwner.Address.City,
                    PackageId = bookingEntity.PackageId,
                    BookingDate = bookingEntity.BookingDate,
                    TicketQuantity = bookingEntity.TicketQuantity,
                    EventTitle = bookingEntity.EventTitle,
                    EventDate = bookingEntity.EventDate,
                    EventLocation = bookingEntity.EventLocation,
                    EventPrice = bookingEntity.EventPrice,
                    TotalPrice = bookingEntity.TotalPrice
                });
            }
        

        return new BookingResult<IEnumerable<Booking>>() { Success = true, Result = bookings };
    }
}
