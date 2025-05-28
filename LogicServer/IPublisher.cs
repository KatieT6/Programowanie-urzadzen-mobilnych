using DataCommon;

namespace LogicServer;

public interface IPublisher
{
    IBookInitData GetNewBook();

    public static IPublisher CreatePublisher()
    {
        return new Publisher();
    }
}
