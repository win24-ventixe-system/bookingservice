using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using Presentation.Services;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingsController(IBookingService bookingsService) : ControllerBase
{
    private readonly IBookingService _bookingsService = bookingsService;

    [HttpPost]
    public async Task<IActionResult> Create (CreateBookingRequest req)
    {

        if (!ModelState.IsValid)
        return BadRequest(ModelState);

        var bookingResult = await _bookingsService.CreateBookingAsync (req);
        if (bookingResult.Success)
        {
            // Return the booking data directly for easier frontend access
            return Ok(new
            {
                success = true,
                result = bookingResult.Result,
                id = bookingResult.Result?.Id, 
                bookingDate = bookingResult.Result?.BookingDate,
                message = "Booking created successfully"
            });
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError, bookingResult.Error);
        }

    }

    [HttpGet]
    public async Task<IActionResult> GetAll ()
    {
        var bookings = await _bookingsService.GetBookingsAsync();
        return Ok(bookings);
    }

    [HttpGet("my")]
    [Authorize]
    public async Task<IActionResult> GetMyBookings()
    {
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
            return Unauthorized();

        var result = await _bookingsService.GetBookingsForUserAsync(email);

        if (!result.Success)
            return StatusCode(500, result.Error);

        return Ok(result.Result);
    }
}
