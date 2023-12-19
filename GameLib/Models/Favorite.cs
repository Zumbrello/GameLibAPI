using System;
using System.Collections.Generic;

namespace GameLib.Models;

public partial class Favorite
{
    public int IdUser { get; set; }

    public int IdGame { get; set; }

    public string? Note { get; set; }

    public virtual Game IdGameNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;
}
