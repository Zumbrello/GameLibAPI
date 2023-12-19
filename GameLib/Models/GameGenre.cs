using System;
using System.Collections.Generic;

namespace GameLib.Models;

public partial class GameGenre
{
    public int IdGame { get; set; }

    public int IdGenre { get; set; }

    public int Id { get; set; }

    public virtual Game IdGameNavigation { get; set; } = null!;

    public virtual Genere IdGenreNavigation { get; set; } = null!;
}
