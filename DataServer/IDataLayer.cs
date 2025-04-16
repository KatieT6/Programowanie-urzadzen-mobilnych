namespace DataServer;

public interface IDataLayer
{
    public IDatabase Database { get; }
    public static IDataLayer CreateDataLayer() { return new DataLayer(); }
}
