using OOP5.BLL;

namespace OOP5.PL;

public class HikingConditions : IHikingConditions
{
    public bool HasEquipment { get; }

    public bool IsGoodWeather { get; }

    public HikingConditions(bool hasEquipment, bool isGoodWeather)
    {
        HasEquipment = hasEquipment;
        IsGoodWeather = isGoodWeather;
    }
}