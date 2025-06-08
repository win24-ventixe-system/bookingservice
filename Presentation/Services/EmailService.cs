using Azure;
using Azure.Communication.Email;
using Presentation.Models;


public class EmailService : IEmailService
{
    private readonly EmailClient _emailClient;

    public EmailService()
    {
        string connectionString = Environment.GetEnvironmentVariable("COMMUNICATION_SERVICES_CONNECTION_STRING")!;
        _emailClient = new EmailClient(connectionString);
    }

    public async Task SendBookingConfirmation(BookingConfirmationEmail model)
    {
        string subject = $"Booking Confirmation - {model.EventTitle}";

                            string plainTextContent = $@"
                    Hello {model.FirstName},
                    Thank you for your booking.

                    Booking Reference: {model.Id}
                    Event: {model.EventTitle}
                    Date: {model.EventDate:MMMM dd, yyyy}
                    Amount of tickets: {model.TicketQuantity}

                    Total Paid: {model.TotalPrice:C}

                    This is an automated message. Please do not reply.";

                            string htmlContent = $@"
                    <html>
                        <body>
                            <h2>Hello {model.FirstName},</h2>
                            <p>Thank you for your booking.</p>
                            <p><strong>Booking Reference:</strong> {model.Id}</p>
                            <p><strong>Event:</strong> {model.EventTitle}</p>
                            <p><strong>Date:</strong> {model.EventDate:MMMM dd, yyyy}</p>
                            <p><strong>Amount of tickets:</strong> {model.TicketQuantity:C}</p>
                            <p><strong>Total Paid:</strong> {model.TotalPrice:C}</p>
                            <br/>
                            <p>This is an automated message. Please do not reply.</p>
                        </body>
                    </html>";

        var emailMessage = new EmailMessage(
            senderAddress: "DoNotReply@549e2035-9a59-4511-a3d1-dc80494178d6.azurecomm.net",
            content: new EmailContent(subject)
            {
                PlainText = plainTextContent,
                Html = htmlContent
            },
            recipients: new EmailRecipients(new List<EmailAddress>
            {
                new EmailAddress(model.Email)
            })
        );

        EmailSendOperation emailSendOperation = await _emailClient.SendAsync(WaitUntil.Completed, emailMessage);
    }
}