using DataCommon;

namespace LogicServer;

internal interface IPublisher
{
    IBookInitData GetNewBook();

    public static IPublisher CreatePublisher()
    {
        return new Publisher();
    }
}
