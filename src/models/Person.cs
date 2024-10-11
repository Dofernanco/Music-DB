public class Person
{
    public int Id { get; set; }
    public string StageName { get; set; }
    public string RealName { get; set; }
    public string BirthDate { get; set; }
    public string DeathDate { get; set; }

    public Person(int id, string stageName, string realName, string birthDate, string deathDate = null)
    {
        Id = id;
        StageName = stageName;
        RealName = realName;
        BirthDate = birthDate;
        DeathDate = deathDate;
    }
}
