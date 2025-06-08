using Presentation.Models;

namespace Presentation.Services;

public interface IEventServiceClient
{
    Task<IEnumerable<Event>> GetAllEventsAsync();
    Task<Event?> GetEventByIdAsync(string id);
}