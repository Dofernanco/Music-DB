public class Performer
{
    public int Id { get; set; }
    public int IdType { get; set; }  // 0: Persona, 1: Grupo
    public string Name { get; set; }

    public Performer(int id, int idType, string name)
    {
        Id = id;
        IdType = idType;
        Name = name;
    }
}
