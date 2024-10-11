public class Group
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }

    public Group(int id, string name, string startDate, string endDate = null)
    {
        Id = id;
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
    }
}
