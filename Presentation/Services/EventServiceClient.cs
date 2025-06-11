using Azure;
using Presentation.Models;
using System.Text.Json; 
using System.Text.Json.Serialization; 

namespace Presentation.Services;

public class EventServiceClient(HttpClient httpClient) : IEventServiceClient
{
    private readonly HttpClient _httpClient = httpClient;

    // Define the JsonSerializerOptions once
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true, 
        ReferenceHandler = ReferenceHandler.Preserve // Crucial for handling $id and $values
    };
    public async Task<IEnumerable<Event>> GetAllEventsAsync()
    {
        var response = await _httpClient.GetAsync("/api/Events");
        response.EnsureSuccessStatusCode(); 

        var jsonString = await response.Content.ReadAsStringAsync();

        var serviceResponse = JsonSerializer.Deserialize<BookingResult<IEnumerable<Event>>>(jsonString, _jsonSerializerOptions);

        return serviceResponse?.Result ?? [];
    }

    public async Task<Event?> GetEventByIdAsync(string id)
    {
        var response = await _httpClient.GetAsync($"/api/Events/{id}");
        response.EnsureSuccessStatusCode(); 

        var jsonString = await response.Content.ReadAsStringAsync();
      
        var serviceResponse = JsonSerializer.Deserialize<BookingResult<Event>>(jsonString, _jsonSerializerOptions);

        return serviceResponse?.Result; 
    }
}

