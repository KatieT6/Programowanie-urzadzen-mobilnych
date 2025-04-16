namespace Communication;

public class NewClientRequest(Guid id)
{
    private Guid id = id;

    public Guid Id
    {
        get { return id; }
        set { id = value; }
    }
}
