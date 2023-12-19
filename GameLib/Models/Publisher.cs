using System;
using System.Collections.Generic;

namespace GameLib.Models;

public partial class Publisher
{
    public int IdPublisher { get; set; }

    public string? Publisher1 { get; set; }

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}
