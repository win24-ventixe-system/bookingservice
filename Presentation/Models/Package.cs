﻿using System.Text.Json.Serialization;

namespace Presentation.Models;

public class Package
{

    public string Id { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string SeatingArrangement { get; set; } = null!;

    public string? Placement { get; set; }

    public decimal Price { get; set; }

    public string? Currency { get; set; }
}
