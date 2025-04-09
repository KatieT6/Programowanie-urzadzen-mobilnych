namespace Communication;

public class Request
{
    private RequestTypes name;
    private List<string> args = new();

    public Request(RequestTypes name, List<string> args)
    {
        this.Name = name;
        this.Args = args;
    }

    public RequestTypes Name { get => name; set => name = value; }
    public List<string> Args { get => args; set => args = value; }
}
