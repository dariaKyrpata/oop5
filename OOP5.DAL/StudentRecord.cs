namespace OOP5.DAL;

public class StudentRecord
{
    public string LastName { get; set; } = "";

    public string FirstName { get; set; } = "";

    public int Course { get; set; }

    public string StudentId { get; set; } = "";

    public bool HasScholarship { get; set; }

    public string PassportSeries { get; set; } = "";

    public string PassportNumber { get; set; } = "";

    public decimal AverageGrade { get; set; }

    public bool IsExpelled { get; set; }
}