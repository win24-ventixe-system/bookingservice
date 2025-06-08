namespace Presentation.Models;

public class Event
{
    public string Id { get; set; } = null!;
    public string? Title { get; set; }
    public DateTime EventDate { get; set; }
    public string? Location { get; set; }
    public string? Image { get; set; }
    public string? Description { get; set; }
    public List<Package> Packages { get; set; } = [];
}
