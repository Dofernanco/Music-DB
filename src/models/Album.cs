public class Album
{
    public int Id { get; set; }
    public string Path { get; set; }
    public string Name { get; set; }
    public int Year { get; set; }

    public Album(int id, string path, string name, int year)
    {
        Id = id;
        Path = path;
        Name = name;
        Year = year;
    }
}
