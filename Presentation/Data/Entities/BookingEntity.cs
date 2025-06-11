using System.ComponentModel.DataAnnotations.Schema;

namespace Presentation.Data.Entities;

public class BookingEntity
{

    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string EventId { get; set; } = null!;

    public int TicketQuantity { get; set; } = 1;

    public DateTime BookingDate { get; set; }
    public string PackageId { get; set; } = null!;

    [ForeignKey(nameof(BookingOwner))]
    public string? BookingOwnerId { get; set; } 
    public BookingOwnerEntity? BookingOwner { get; set; }


    public string EventTitle { get; set; } = null!;
    public DateTime EventDate { get; set; } // The date of the event itself
    public string EventLocation { get; set; } = null!;
    public decimal EventPrice { get; set; } // Price of the single ticket/package
    public decimal TotalPrice { get; set; } // Total price for all tickets in this booking

}
