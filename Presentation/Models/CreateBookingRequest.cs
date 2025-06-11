namespace Presentation.Models;

public class CreateBookingRequest
{
    public string EventId { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string StreetName { get; set; } = null!;

    public string PostalCode { get; set; } = null!;

    public string City { get; set; } = null!;
    public string PackageId { get; set; } = null!;
    public DateTime BookingDate { get; set; }

    public int TicketQuantity { get; set; } = 1;

}
