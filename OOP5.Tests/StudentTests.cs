using OOP5.BLL;
using Xunit;

namespace OOP5.Tests;

public class StudentTests
{
    [Fact]
    public void Constructor_ValidData_SetsAllProperties()
    {
        Student student = CreateStudent();

        Assert.Equal("Kovalenko", student.LastName);
        Assert.Equal("Anna", student.FirstName);
        Assert.Equal(4, student.Course);
        Assert.Equal("ST001", student.StudentId);
        Assert.True(student.HasScholarship);
        Assert.Equal("AA", student.PassportSeries);
        Assert.Equal("123456", student.PassportNumber);
        Assert.Equal(75m, student.AverageGrade);
        Assert.False(student.IsExpelled);
    }

    [Fact]
    public void Constructor_TextWithSpaces_TrimsAllTextFields()
    {
        Student student = CreateStudent(
            lastName: " Kovalenko ",
            firstName: " Anna ",
            studentId: " ST001 ",
            passportSeries: " AA ",
            passportNumber: " 123456 ");

        Assert.Equal("Kovalenko", student.LastName);
        Assert.Equal("Anna", student.FirstName);
        Assert.Equal("ST001", student.StudentId);
        Assert.Equal("AA", student.PassportSeries);
        Assert.Equal("123456", student.PassportNumber);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_InvalidText_ThrowsArgumentException(
        string? invalidText)
    {
        string value = invalidText!;

        Assert.Throws<ArgumentException>(
            () => CreateStudent(lastName: value));

        Assert.Throws<ArgumentException>(
            () => CreateStudent(firstName: value));

        Assert.Throws<ArgumentException>(
            () => CreateStudent(studentId: value));

        Assert.Throws<ArgumentException>(
            () => CreateStudent(passportSeries: value));

        Assert.Throws<ArgumentException>(
            () => CreateStudent(passportNumber: value));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(7)]
    public void Constructor_InvalidCourse_ThrowsArgumentOutOfRangeException(
        int course)
    {
        Action action = () => CreateStudent(course: course);

        Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(6)]
    public void Constructor_BoundaryCourse_AcceptsValue(int course)
    {
        Student student = CreateStudent(course: course);

        Assert.Equal(course, student.Course);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Constructor_InvalidGrade_ThrowsArgumentOutOfRangeException(
        int grade)
    {
        Action action = () => CreateStudent(averageGrade: grade);

        Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    public void Constructor_BoundaryGrade_AcceptsValue(int grade)
    {
        Student student = CreateStudent(averageGrade: grade);

        Assert.Equal((decimal)grade, student.AverageGrade);
    }

    [Fact]
    public void Constructor_ExpelledStudentWithScholarship_ThrowsArgumentException()
    {
        Action action = () => CreateStudent(
            hasScholarship: true,
            isExpelled: true);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Constructor_ExpelledStudentWithoutScholarship_PreservesStatus()
    {
        Student student = CreateStudent(
            hasScholarship: false,
            isExpelled: true);

        Assert.True(student.IsExpelled);
        Assert.False(student.HasScholarship);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ExpelForPoorPerformance_GradeBelowThreshold_ExpelsStudent(
        bool hasScholarship)
    {
        Student student = CreateStudent(
            hasScholarship: hasScholarship,
            averageGrade: 59.99m);

        bool result = student.ExpelForPoorPerformance();

        Assert.True(result);
        Assert.True(student.IsExpelled);
        Assert.False(student.HasScholarship);
    }

    [Fact]
    public void ExpelForPoorPerformance_ZeroGrade_ExpelsStudent()
    {
        Student student = CreateStudent(averageGrade: 0m);

        bool result = student.ExpelForPoorPerformance();

        Assert.True(result);
        Assert.True(student.IsExpelled);
        Assert.False(student.HasScholarship);
    }

    [Theory]
    [InlineData(60, true)]
    [InlineData(60, false)]
    [InlineData(100, true)]
    [InlineData(100, false)]
    public void ExpelForPoorPerformance_PassingGrade_DoesNotChangeStudent(
        int grade,
        bool hasScholarship)
    {
        Student student = CreateStudent(
            hasScholarship: hasScholarship,
            averageGrade: grade);

        bool result = student.ExpelForPoorPerformance();

        Assert.False(result);
        Assert.False(student.IsExpelled);
        Assert.Equal(hasScholarship, student.HasScholarship);
    }

    [Fact]
    public void ExpelForPoorPerformance_AlreadyExpelled_ReturnsFalse()
    {
        Student student = CreateStudent(
            hasScholarship: false,
            averageGrade: 40m,
            isExpelled: true);

        bool result = student.ExpelForPoorPerformance();

        Assert.False(result);
        Assert.True(student.IsExpelled);
        Assert.False(student.HasScholarship);
    }

    [Fact]
    public void ExpelForPoorPerformance_RepeatedCall_ReturnsFalse()
    {
        Student student = CreateStudent(averageGrade: 40m);
        student.ExpelForPoorPerformance();

        bool result = student.ExpelForPoorPerformance();

        Assert.False(result);
        Assert.True(student.IsExpelled);
        Assert.False(student.HasScholarship);
    }

    private static Student CreateStudent(
        string lastName = "Kovalenko",
        string firstName = "Anna",
        int course = 4,
        string studentId = "ST001",
        bool hasScholarship = true,
        string passportSeries = "AA",
        string passportNumber = "123456",
        decimal averageGrade = 75m,
        bool isExpelled = false)
    {
        return new Student(
            lastName,
            firstName,
            course,
            studentId,
            hasScholarship,
            passportSeries,
            passportNumber,
            averageGrade,
            isExpelled);
    }
}
