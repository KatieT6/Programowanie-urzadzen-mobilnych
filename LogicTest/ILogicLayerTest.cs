using LogicClient;

namespace LogicTest;

[TestClass]
public sealed class ILogicLayerTest
{
    [TestMethod]
    public void LogicLayer_CreationTest()
    {
        var layer = ILogicLayer.CreateLogicLayer();
        Assert.IsNotNull(layer);
        Assert.IsNotNull(layer.LibraryLogic);
    }
}
