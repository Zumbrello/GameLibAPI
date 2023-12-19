namespace GameLib;

public class ModelGameInformation
{
    public int IdGame { get; set; }

    public string? Name { get; set; }

    public string? Publisher { get; set; }
    public string? Developer { get; set; }

    public string[] Geners { get; set; }

    public string? Description { get; set; }
    public string? SystemRequestMin { get; set; }

    public string? SystemRequestRec { get; set; }

    public string? ReleaseDate { get; set; }
    public string? MainImage { get; set; }
}