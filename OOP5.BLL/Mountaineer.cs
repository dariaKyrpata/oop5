namespace OOP5.BLL;

public class Mountaineer : IDrawable
{
    private readonly HikingService hikingService;

    public string Name { get; }
    public string ClimberId { get; }
    public string Mountain { get; }
    public int ClimbsCompleted { get; private set; }

    public Mountaineer(
        string name,
        string climberId,
        string mountain,
        HikingService hikingService)
    {
        Name = ValidateText(name, nameof(name));
        ClimberId = ValidateText(climberId, nameof(climberId));
        Mountain = ValidateText(mountain, nameof(mountain));

        this.hikingService = hikingService
            ?? throw new ArgumentNullException(nameof(hikingService));
    }

    public bool TryGoHiking()
    {
        if (!hikingService.CanGoHiking())
        {
            return false;
        }

        ClimbsCompleted++;

        return true;
    }

    public string Draw()
    {
        return
            "       /\\\n" +
            "      /  \\\n" +
            "     / /\\ \\\n" +
            "    / /  \\ \\\n" +
            "   /_/____\\_\\\n" +
            "      ||";
    }

    private static string ValidateText(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "Value cannot be empty.",
                parameterName);
        }

        return value.Trim();
    }
}