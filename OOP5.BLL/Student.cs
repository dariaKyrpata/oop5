namespace OOP5.BLL;

public class Student : IDrawable
{
    public string LastName { get; }
    public string FirstName { get; }
    public int Course { get; }
    public string StudentId { get; }
    public bool HasScholarship { get; private set; }
    public string PassportSeries { get; }
    public string PassportNumber { get; }
    public decimal AverageGrade { get; }
    public bool IsExpelled { get; private set; }

    public Student(
        string lastName,
        string firstName,
        int course,
        string studentId,
        bool hasScholarship,
        string passportSeries,
        string passportNumber,
        decimal averageGrade,
        bool isExpelled = false)
    {
        LastName = ValidateText(lastName, nameof(lastName));
        FirstName = ValidateText(firstName, nameof(firstName));
        StudentId = ValidateText(studentId, nameof(studentId));
        PassportSeries = ValidateText(passportSeries, nameof(passportSeries));
        PassportNumber = ValidateText(passportNumber, nameof(passportNumber));

        if (course < 1 || course > 6)
        {
            throw new ArgumentOutOfRangeException(
                nameof(course),
                "Course must be between 1 and 6.");
        }

        if (averageGrade < 0 || averageGrade > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(averageGrade),
                "Average grade must be between 0 and 100.");
        }

        if (isExpelled && hasScholarship)
        {
            throw new ArgumentException(
                "An expelled student cannot receive a scholarship.",
                nameof(hasScholarship));
        }

        Course = course;
        HasScholarship = hasScholarship;
        AverageGrade = averageGrade;
        IsExpelled = isExpelled;
    }

    public bool ExpelForPoorPerformance()
    {
        if (IsExpelled || AverageGrade >= 60)
        {
            return false;
        }

        IsExpelled = true;
        HasScholarship = false;

        return true;
    }

    public string Draw()
    {
        return
            "   /\\\n" +
            "  /  \\\n" +
            " /____\\\n" +
            " |    |\n" +
            " |____|";
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