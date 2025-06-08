namespace Presentation.Models;

public class BookingConfirmationEmail
{
    public string Id { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string EventTitle { get; set; } = null!;
    public string TicketQuantity { get; set; } = null!;
    public DateTime EventDate { get; set; }
    public decimal TotalPrice { get; set; }
}
