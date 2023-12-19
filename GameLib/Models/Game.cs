using System;
using System.Collections.Generic;

namespace GameLib.Models;

public partial class Game
{
    public int IdGame { get; set; }

    public string? GameName { get; set; }

    public int? IdDeveloper { get; set; }

    public int? IdPublisher { get; set; }

    public string? Description { get; set; }

    public string? SystemRequestMin { get; set; }

    public string? SystemRequestRec { get; set; }

    public string? ReleaseDate { get; set; }

    public string? MainImage { get; set; }

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<GameGenre> GameGenres { get; set; } = new List<GameGenre>();

    public virtual Developer? IdDeveloperNavigation { get; set; }

    public virtual Publisher? IdPublisherNavigation { get; set; }
}
