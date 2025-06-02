namespace Communication;

public class NewClientRequest
{
    private Guid id;

    public NewClientRequest()
    {
        id = Guid.Empty;
    }

    public NewClientRequest(Guid id)
    {
        this.Id = id;
    }

    public Guid Id
    {
        get { return id; }
        set { id = value; }
    }
}
