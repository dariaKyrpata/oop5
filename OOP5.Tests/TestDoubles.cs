using OOP5.BLL;

namespace OOP5.Tests;

internal class StubStudentRepository : IStudentRepository
{
    public IReadOnlyList<Student> StudentsToLoad { get; set; }
        = Array.Empty<Student>();

    public IReadOnlyList<Student> SavedStudents { get; private set; }
        = Array.Empty<Student>();

    public Exception? LoadException { get; set; }

    public Exception? SaveException { get; set; }

    public int LoadCallCount { get; private set; }

    public int SaveCallCount { get; private set; }

    public IReadOnlyList<Student> Load()
    {
        LoadCallCount++;

        if (LoadException is not null)
        {
            throw LoadException;
        }

        return StudentsToLoad;
    }

    public void Save(IReadOnlyList<Student> students)
    {
        SaveCallCount++;

        if (SaveException is not null)
        {
            throw SaveException;
        }

        SavedStudents = students.ToArray();
    }
}

internal class StubHikingConditions : IHikingConditions
{
    public bool HasEquipment { get; set; }

    public bool IsGoodWeather { get; set; }

    public StubHikingConditions(
        bool hasEquipment,
        bool isGoodWeather)
    {
        HasEquipment = hasEquipment;
        IsGoodWeather = isGoodWeather;
    }
}