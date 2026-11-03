using OOP5.BLL;
using Xunit;

namespace OOP5.Tests;

public class ProfessionTests
{
    [Fact]
    public void Postman_Constructor_StoresTrimmedDataAndZeroCounters()
    {
        HikingService service = CreateHikingService();

        Postman postman = new(
            " Alex ",
            " P001 ",
            " Center ",
            service);

        Assert.Equal("Alex", postman.Name);
        Assert.Equal("P001", postman.EmployeeId);
        Assert.Equal("Center", postman.DeliveryArea);
        Assert.Equal(0, postman.DeliveredLetters);
        Assert.Equal(0, postman.HikesCompleted);
    }

    [Fact]
    public void Mountaineer_Constructor_StoresTrimmedDataAndZeroCounter()
    {
        HikingService service = CreateHikingService();

        Mountaineer mountaineer = new(
            " Maria ",
            " M001 ",
            " Hoverla ",
            service);

        Assert.Equal("Maria", mountaineer.Name);
        Assert.Equal("M001", mountaineer.ClimberId);
        Assert.Equal("Hoverla", mountaineer.Mountain);
        Assert.Equal(0, mountaineer.ClimbsCompleted);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Postman_Constructor_InvalidText_ThrowsArgumentException(
        string? invalidText)
    {
        HikingService service = CreateHikingService();
        string value = invalidText!;

        Assert.Throws<ArgumentException>(
            () => new Postman(value, "P001", "Center", service));

        Assert.Throws<ArgumentException>(
            () => new Postman("Alex", value, "Center", service));

        Assert.Throws<ArgumentException>(
            () => new Postman("Alex", "P001", value, service));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Mountaineer_Constructor_InvalidText_ThrowsArgumentException(
        string? invalidText)
    {
        HikingService service = CreateHikingService();
        string value = invalidText!;

        Assert.Throws<ArgumentException>(
            () => new Mountaineer(value, "M001", "Hoverla", service));

        Assert.Throws<ArgumentException>(
            () => new Mountaineer("Maria", value, "Hoverla", service));

        Assert.Throws<ArgumentException>(
            () => new Mountaineer("Maria", "M001", value, service));
    }

    [Fact]
    public void Postman_Constructor_NullService_ThrowsArgumentNullException()
    {
        Action action = () => new Postman(
            "Alex",
            "P001",
            "Center",
            null!);

        Assert.Throws<ArgumentNullException>(action);
    }

    [Fact]
    public void Mountaineer_Constructor_NullService_ThrowsArgumentNullException()
    {
        Action action = () => new Mountaineer(
            "Maria",
            "M001",
            "Hoverla",
            null!);

        Assert.Throws<ArgumentNullException>(action);
    }

    [Fact]
    public void DeliverLetter_RepeatedCalls_IncrementsCounter()
    {
        Postman postman = new(
            "Alex",
            "P001",
            "Center",
            CreateHikingService());

        postman.DeliverLetter();
        postman.DeliverLetter();

        Assert.Equal(2, postman.DeliveredLetters);
        Assert.Equal(0, postman.HikesCompleted);
    }

    [Theory]
    [InlineData(true, true, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, false)]
    public void Postman_TryGoHiking_UpdatesCounterOnlyWhenAllowed(
        bool hasEquipment,
        bool isGoodWeather,
        bool expected)
    {
        HikingService service = CreateHikingService(
            hasEquipment,
            isGoodWeather);

        Postman postman = new("Alex", "P001", "Center", service);

        bool result = postman.TryGoHiking();

        Assert.Equal(expected, result);
        Assert.Equal(expected ? 1 : 0, postman.HikesCompleted);
        Assert.Equal(0, postman.DeliveredLetters);
    }

    [Theory]
    [InlineData(true, true, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, false)]
    public void Mountaineer_TryGoHiking_UpdatesCounterOnlyWhenAllowed(
        bool hasEquipment,
        bool isGoodWeather,
        bool expected)
    {
        HikingService service = CreateHikingService(
            hasEquipment,
            isGoodWeather);

        Mountaineer mountaineer = new(
            "Maria",
            "M001",
            "Hoverla",
            service);

        bool result = mountaineer.TryGoHiking();

        Assert.Equal(expected, result);
        Assert.Equal(expected ? 1 : 0, mountaineer.ClimbsCompleted);
    }

    [Fact]
    public void Postman_FailedHike_PreservesPreviousSuccessfulHikes()
    {
        StubHikingConditions conditions = new(true, true);
        HikingService service = new(conditions);
        Postman postman = new("Alex", "P001", "Center", service);

        postman.TryGoHiking();
        postman.TryGoHiking();
        conditions.HasEquipment = false;

        bool result = postman.TryGoHiking();

        Assert.False(result);
        Assert.Equal(2, postman.HikesCompleted);
    }

    [Fact]
    public void Mountaineer_FailedClimb_PreservesPreviousSuccessfulClimbs()
    {
        StubHikingConditions conditions = new(true, true);
        HikingService service = new(conditions);
        Mountaineer mountaineer = new(
            "Maria",
            "M001",
            "Hoverla",
            service);

        mountaineer.TryGoHiking();
        mountaineer.TryGoHiking();
        conditions.IsGoodWeather = false;

        bool result = mountaineer.TryGoHiking();

        Assert.False(result);
        Assert.Equal(2, mountaineer.ClimbsCompleted);
    }

    private static HikingService CreateHikingService(
        bool hasEquipment = true,
        bool isGoodWeather = true)
    {
        return new HikingService(
            new StubHikingConditions(hasEquipment, isGoodWeather));
    }
}