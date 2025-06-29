﻿namespace Application.Models;

public class CreatePackageRequest
{
    public string Title { get; set; } = null!;
    public string SeatingArrangement { get; set; } = null!;
    public string? Placement { get; set; }
    public decimal? Price { get; set; }
    public string? Currency { get; set; }
}
