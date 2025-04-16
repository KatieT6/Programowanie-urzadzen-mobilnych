namespace Communication;

public class NewClientRequest
{
    private Guid id;

    public NewClientRequest(Guid id)
    {
        this.id = id;
    }

    public Guid Id
    {
        get { return id; }
        set { id = value; }
    }
}
