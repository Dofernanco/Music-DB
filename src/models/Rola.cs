public class Rola
{
    public int Id { get; set; }
    public int PerformerId { get; set; }
    public int AlbumId { get; set; }
    public string Path { get; set; }
    public string Title { get; set; }
    public int TrackNumber { get; set; }
    public int Year { get; set; }
    public string Genre { get; set; }
    public string PerformerName { get; set; }
    public string AlbumName { get; set; }

    public Rola(int id, int performerId, int albumId, string path, string title, int trackNumber, int year, string genre)
    {
        Id = id;
        PerformerId = performerId;
        AlbumId = albumId;
        Path = path;
        Title = title;
        TrackNumber = trackNumber;
        Year = year;
        Genre = genre;
    }
}
