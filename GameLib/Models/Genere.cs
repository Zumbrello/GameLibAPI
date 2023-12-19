using System;
using System.Collections.Generic;

namespace GameLib.Models;

public partial class Genere
{
    public int IdGenre { get; set; }

    public string? Gener { get; set; }

    public virtual ICollection<GameGenre> GameGenres { get; set; } = new List<GameGenre>();
}
