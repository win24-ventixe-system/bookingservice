namespace Presentation.Models;

public class Booking
{
    public string BookingId { get; set; } = Guid.NewGuid().ToString();
    public string EventId { get; set; } = null!;

    public string EventTitle { get; set; } = null!;
    public DateTime EventDate { get; set; }
    public string Location { get; set; } = null!;


    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string StreetName { get; set; } = null!;

    public string PostalCode { get; set; } = null!;

    public string City { get; set; } = null!;
    public string PackageOption { get; set; } = null!;

    public int TicketQuantity { get; set; } = 1;

    public decimal TotalPrice { get; set; }

}
