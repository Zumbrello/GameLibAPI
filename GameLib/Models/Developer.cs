using System;
using System.Collections.Generic;

namespace GameLib.Models;

public partial class Developer
{
    public int IdDeveloper { get; set; }

    public string? Developer1 { get; set; }

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}
