using Data;

namespace DataTest;

[TestClass]
public sealed class IDataLayerTest
{
    [TestMethod]
    public void IDataLayerTest_CreateDataLayer()
    {
        IDataLayer layer = IDataLayer.CreateDataLayer();

        Assert.IsNotNull(layer);
        Assert.IsNotNull(layer.Library);
    }
}