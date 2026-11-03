using OOP5.BLL;
using Xunit;

namespace OOP5.Tests;

public class DrawingTests
{
    [Fact]
    public void Student_Draw_ReturnsHouse()
    {
        IDrawable drawable = new Student(
            "Kovalenko",
            "Anna",
            4,
            "ST001",
            false,
            "AA",
            "123456",
            75m);

        string expected =
            "   /\\\n" +
            "  /  \\\n" +
            " /____\\\n" +
            " |    |\n" +
            " |____|";

        string result = drawable.Draw();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Postman_Draw_ReturnsMailbox()
    {
        StubHikingConditions conditions = new(false, false);
        HikingService hikingService = new(conditions);

        IDrawable drawable = new Postman(
            "Alex",
            "P001",
            "Center",
            hikingService);

        string expected =
            "  _________\n" +
            " |         |\n" +
            " |  MAIL   |\n" +
            " |_________|\n" +
            "     ||\n" +
            "     ||";

        string result = drawable.Draw();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Mountaineer_Draw_ReturnsMountains()
    {
        StubHikingConditions conditions = new(false, false);
        HikingService hikingService = new(conditions);

        IDrawable drawable = new Mountaineer(
            "Maria",
            "M001",
            "Hoverla",
            hikingService);

        string expected =
            "       /\\\n" +
            "      /  \\\n" +
            "     / /\\ \\\n" +
            "    / /  \\ \\\n" +
            "   /_/____\\_\\\n" +
            "      ||";

        string result = drawable.Draw();

        Assert.Equal(expected, result);
    }
}