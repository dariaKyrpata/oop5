namespace OOP5.BLL;

public class HikingService
{
    private readonly IHikingConditions conditions;

    public HikingService(IHikingConditions conditions)
    {
        this.conditions = conditions
            ?? throw new ArgumentNullException(nameof(conditions));
    }

    public bool CanGoHiking()
    {
        return conditions.HasEquipment && conditions.IsGoodWeather;
    }
}