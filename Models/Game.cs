using System;
using System.Collections.Generic;

namespace StudentFunctions.Models;

public partial class Game
{
    public int GameId { get; set; }

    public int Year { get; set; }

    public string Gender { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string Continent { get; set; } = null!;

    public string Winner { get; set; } = null!;

    public DateTime? Created { get; set; }
}
