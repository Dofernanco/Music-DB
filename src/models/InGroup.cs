public class InGroup
{
    public int PersonId { get; set; }
    public int GroupId { get; set; }

    public InGroup(int personId, int groupId)
    {
        PersonId = personId;
        GroupId = groupId;
    }
}
