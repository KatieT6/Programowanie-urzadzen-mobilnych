namespace DataServer;

internal class DataLayer : IDataLayer
{
    IDatabase database;

    public IDatabase Database => database;

    public DataLayer()
    {
        database = IDatabase.CreateDatabase();
    }
}
