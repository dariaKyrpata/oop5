using OOP5.BLL;
using Xunit;

namespace OOP5.Tests;

public class HikingServiceTests
{
    [Fact]
    public void Constructor_NullConditions_ThrowsArgumentNullException()
    {
        Action action = () => new HikingService(null!);

        Assert.Throws<ArgumentNullException>(action);
    }

    [Theory]
    [InlineData(true, true, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, false)]
    public void CanGoHiking_ReturnsExpectedResult(
        bool hasEquipment,
        bool isGoodWeather,
        bool expected)
    {
        StubHikingConditions conditions = new(
            hasEquipment,
            isGoodWeather);

        HikingService service = new(conditions);

        bool result = service.CanGoHiking();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CanGoHiking_ConditionsChange_UsesCurrentConditions()
    {
        StubHikingConditions conditions = new(true, true);
        HikingService service = new(conditions);

        bool initialResult = service.CanGoHiking();

        Assert.True(initialResult);

        conditions.IsGoodWeather = false;

        bool updatedResult = service.CanGoHiking();

        Assert.False(updatedResult);
    }
}