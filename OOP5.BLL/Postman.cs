namespace OOP5.BLL;

public class Postman : IDrawable
{
    private readonly HikingService hikingService;

    public string Name { get; }
    public string EmployeeId { get; }
    public string DeliveryArea { get; }
    public int DeliveredLetters { get; private set; }
    public int HikesCompleted { get; private set; }

    public Postman(
        string name,
        string employeeId,
        string deliveryArea,
        HikingService hikingService)
    {
        Name = ValidateText(name, nameof(name));
        EmployeeId = ValidateText(employeeId, nameof(employeeId));
        DeliveryArea = ValidateText(deliveryArea, nameof(deliveryArea));

        this.hikingService = hikingService
            ?? throw new ArgumentNullException(nameof(hikingService));
    }

    public void DeliverLetter()
    {
        DeliveredLetters++;
    }

    public bool TryGoHiking()
    {
        if (!hikingService.CanGoHiking())
        {
            return false;
        }

        HikesCompleted++;

        return true;
    }

    public string Draw()
    {
        return
            "  _________\n" +
            " |         |\n" +
            " |  MAIL   |\n" +
            " |_________|\n" +
            "     ||\n" +
            "     ||";
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