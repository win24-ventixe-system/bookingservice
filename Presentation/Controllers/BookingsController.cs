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

        var booking = await _bookingsService.CreateBookingasync (req);
        return booking.Success 
            ? Ok(booking) 
            : StatusCode(StatusCodes.Status500InternalServerError, "Unable to create booking.");
    }
}
