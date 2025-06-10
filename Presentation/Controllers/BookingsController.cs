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
        if(!ModelState.IsValid)
        return BadRequest(ModelState);

        var booking = await _bookingsService.CreateBookingAsync (req);
        return booking.Success 
            ? Ok(booking) 
            : StatusCode(StatusCodes.Status500InternalServerError, "Unable to create booking.");
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
