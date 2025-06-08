using Presentation.Models;

namespace Presentation.Services;

public class EventServiceClient(HttpClient httpClient) : IEventServiceClient
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<IEnumerable<Event>> GetAllEventsAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<Event>>("/api/Events") ?? [];
    }

    public async Task<Event?> GetEventByIdAsync(string id)
    {
        return await _httpClient.GetFromJsonAsync<Event>($"/api/Events/{id}");
    }
}
