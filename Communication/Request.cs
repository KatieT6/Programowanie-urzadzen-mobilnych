namespace Communication;

public class Request
{
    private string name = "";
    private string argsJson = "";

    public Request(string name, string argsJson)
    {
        this.Name = name;
        this.ArgsJson = argsJson;
    }

    public string Name { get => name; set => name = value; }
    public string ArgsJson { get => argsJson; set => argsJson = value; }
}
