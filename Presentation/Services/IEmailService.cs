using Presentation.Models;

public interface IEmailService
{
    Task SendBookingConfirmation(BookingConfirmationEmail model);
}