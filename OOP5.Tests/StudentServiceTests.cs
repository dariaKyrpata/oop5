using OOP5.BLL;
using Xunit;

namespace OOP5.Tests;

public class StudentServiceTests
{
    [Fact]
    public void Constructor_NullRepository_ThrowsArgumentNullException()
    {
        Action action = () => new StudentService(null!);

        Assert.Throws<ArgumentNullException>(action);
    }

    [Fact]
    public void GetStudents_NewService_ReturnsEmptyCollection()
    {
        StudentService service = new(new StubStudentRepository());

        IReadOnlyList<Student> result = service.GetStudents();

        Assert.Empty(result);
    }

    [Fact]
    public void AddStudent_ValidStudent_AddsStudent()
    {
        StudentService service = new(new StubStudentRepository());
        Student student = CreateStudent();

        service.AddStudent(student);

        Assert.Same(student, Assert.Single(service.GetStudents()));
    }

    [Fact]
    public void AddStudent_NullStudent_ThrowsArgumentNullException()
    {
        StudentService service = new(new StubStudentRepository());

        Action action = () => service.AddStudent(null!);

        Assert.Throws<ArgumentNullException>(action);
        Assert.Empty(service.GetStudents());
    }

    [Theory]
    [InlineData("ST001")]
    [InlineData("st001")]
    [InlineData(" ST001 ")]
    public void AddStudent_DuplicateId_RejectsStudent(string duplicateId)
    {
        StudentService service = new(new StubStudentRepository());
        Student original = CreateStudent();
        service.AddStudent(original);
        Student duplicate = CreateStudent(studentId: duplicateId);

        Action action = () => service.AddStudent(duplicate);

        Assert.Throws<ArgumentException>(action);
        Assert.Same(original, Assert.Single(service.GetStudents()));
    }

    [Fact]
    public void GetStudents_ReturnedCollection_PreventsDirectAddition()
    {
        StudentService service = new(new StubStudentRepository());
        Student original = CreateStudent();
        service.AddStudent(original);

        IReadOnlyList<Student> result = service.GetStudents();

        IList<Student> collection = Assert.IsAssignableFrom<IList<Student>>(
            result);

        Assert.True(collection.IsReadOnly);

        Assert.Throws<NotSupportedException>(
            () => collection.Add(CreateStudent("ST002")));

        Assert.Same(original, Assert.Single(service.GetStudents()));
    }

    [Theory]
    [InlineData("ST001")]
    [InlineData("st001")]
    [InlineData(" ST001 ")]
    public void RemoveStudent_ExistingId_RemovesOnlyMatchingStudent(
        string studentId)
    {
        StudentService service = new(new StubStudentRepository());
        service.AddStudent(CreateStudent());
        Student remaining = CreateStudent("ST002");
        service.AddStudent(remaining);

        bool result = service.RemoveStudent(studentId);

        Assert.True(result);
        Assert.Same(remaining, Assert.Single(service.GetStudents()));
    }

    [Fact]
    public void RemoveStudent_UnknownId_PreservesCollection()
    {
        StudentService service = new(new StubStudentRepository());
        Student original = CreateStudent();
        service.AddStudent(original);

        bool result = service.RemoveStudent("UNKNOWN");

        Assert.False(result);
        Assert.Same(original, Assert.Single(service.GetStudents()));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void StudentOperations_InvalidId_ThrowArgumentException(
        string? invalidId)
    {
        StudentService service = new(new StubStudentRepository());
        string value = invalidId!;

        Assert.Throws<ArgumentException>(
            () => service.RemoveStudent(value));

        Assert.Throws<ArgumentException>(
            () => service.ExpelStudentForPoorPerformance(value));
    }

    [Fact]
    public void CountFourthYearStudentsWithoutScholarship_EmptyList_ReturnsZero()
    {
        StudentService service = new(new StubStudentRepository());

        int result = service.CountFourthYearStudentsWithoutScholarship();

        Assert.Equal(0, result);
    }

    [Theory]
    [InlineData(4, false, false, 1)]
    [InlineData(4, true, false, 0)]
    [InlineData(4, false, true, 0)]
    [InlineData(1, false, false, 0)]
    [InlineData(2, false, false, 0)]
    [InlineData(3, false, false, 0)]
    [InlineData(5, false, false, 0)]
    [InlineData(6, false, false, 0)]
    public void CountFourthYearStudentsWithoutScholarship_FiltersCorrectly(
        int course,
        bool hasScholarship,
        bool isExpelled,
        int expected)
    {
        StudentService service = new(new StubStudentRepository());

        service.AddStudent(CreateStudent(
            course: course,
            hasScholarship: hasScholarship,
            isExpelled: isExpelled));

        int result = service.CountFourthYearStudentsWithoutScholarship();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CountFourthYearStudentsWithoutScholarship_MixedList_ReturnsTwo()
    {
        StudentService service = new(new StubStudentRepository());
        service.AddStudent(CreateStudent("ST001"));
        service.AddStudent(CreateStudent("ST002", averageGrade: 55m));
        service.AddStudent(CreateStudent("ST003", hasScholarship: true));
        service.AddStudent(CreateStudent("ST004", course: 3));
        service.AddStudent(CreateStudent("ST005", isExpelled: true));

        int result = service.CountFourthYearStudentsWithoutScholarship();

        Assert.Equal(2, result);
    }

    [Theory]
    [InlineData("ST001")]
    [InlineData("st001")]
    [InlineData(" ST001 ")]
    public void ExpelStudentForPoorPerformance_FailingStudent_ExpelsStudent(
        string studentId)
    {
        StudentService service = new(new StubStudentRepository());
        Student student = CreateStudent(
            hasScholarship: true,
            averageGrade: 40m);
        service.AddStudent(student);

        bool result = service.ExpelStudentForPoorPerformance(studentId);

        Assert.True(result);
        Assert.True(student.IsExpelled);
        Assert.False(student.HasScholarship);
        Assert.Equal(0, service.CountFourthYearStudentsWithoutScholarship());
    }

    [Fact]
    public void ExpelStudentForPoorPerformance_PassingStudent_ReturnsFalse()
    {
        StudentService service = new(new StubStudentRepository());
        Student student = CreateStudent(averageGrade: 60m);
        service.AddStudent(student);

        bool result = service.ExpelStudentForPoorPerformance("ST001");

        Assert.False(result);
        Assert.False(student.IsExpelled);
    }

    [Fact]
    public void ExpelStudentForPoorPerformance_UnknownId_ThrowsKeyNotFoundException()
    {
        StudentService service = new(new StubStudentRepository());

        Action action = () =>
            service.ExpelStudentForPoorPerformance("UNKNOWN");

        Assert.Throws<KeyNotFoundException>(action);
    }

    [Fact]
    public void Load_ValidCollection_ReplacesCurrentStudents()
    {
        Student loadedStudent = CreateStudent("ST002");
        StubStudentRepository repository = new()
        {
            StudentsToLoad = new[] { loadedStudent }
        };

        StudentService service = new(repository);
        service.AddStudent(CreateStudent("ST001"));

        service.Load();

        Assert.Equal(1, repository.LoadCallCount);
        Assert.Same(loadedStudent, Assert.Single(service.GetStudents()));
    }

    [Fact]
    public void Load_EmptyCollection_ClearsCurrentStudents()
    {
        StubStudentRepository repository = new();
        StudentService service = new(repository);
        service.AddStudent(CreateStudent());

        service.Load();

        Assert.Empty(service.GetStudents());
        Assert.Equal(1, repository.LoadCallCount);
    }

    [Fact]
    public void Load_NullCollection_RejectsDataAndPreservesCurrentStudents()
    {
        StubStudentRepository repository = new()
        {
            StudentsToLoad = null!
        };

        StudentService service = new(repository);
        Student original = CreateStudent();
        service.AddStudent(original);

        Action action = () => service.Load();

        Assert.Throws<InvalidOperationException>(action);
        Assert.Same(original, Assert.Single(service.GetStudents()));
    }

    [Fact]
    public void Load_NullEntry_RejectsDataAndPreservesCurrentStudents()
    {
        StubStudentRepository repository = new()
        {
            StudentsToLoad = new Student[]
            {
                CreateStudent("ST002"),
                null!
            }
        };

        StudentService service = new(repository);
        Student original = CreateStudent();
        service.AddStudent(original);

        Action action = () => service.Load();

        Assert.Throws<InvalidOperationException>(action);
        Assert.Same(original, Assert.Single(service.GetStudents()));
    }

    [Theory]
    [InlineData("ST002")]
    [InlineData("st002")]
    [InlineData(" ST002 ")]
    public void Load_DuplicateIds_RejectsDataAndPreservesCurrentStudents(
        string duplicateId)
    {
        StubStudentRepository repository = new()
        {
            StudentsToLoad = new[]
            {
                CreateStudent("ST002"),
                CreateStudent(duplicateId)
            }
        };

        StudentService service = new(repository);
        Student original = CreateStudent();
        service.AddStudent(original);

        Action action = () => service.Load();

        Assert.Throws<InvalidOperationException>(action);
        Assert.Same(original, Assert.Single(service.GetStudents()));
    }

    [Fact]
    public void Load_RepositoryFailure_PropagatesErrorAndPreservesStudents()
    {
        IOException expected = new("Cannot read student data.");

        StubStudentRepository repository = new()
        {
            LoadException = expected
        };

        StudentService service = new(repository);
        Student original = CreateStudent();
        service.AddStudent(original);

        IOException actual = Assert.Throws<IOException>(
            () => service.Load());

        Assert.Same(expected, actual);
        Assert.Equal(1, repository.LoadCallCount);
        Assert.Same(original, Assert.Single(service.GetStudents()));
    }

    [Fact]
    public void Save_ExistingStudents_PassesStudentsToRepository()
    {
        StubStudentRepository repository = new();
        StudentService service = new(repository);
        Student first = CreateStudent("ST001");
        Student second = CreateStudent("ST002");
        service.AddStudent(first);
        service.AddStudent(second);

        service.Save();

        Assert.Equal(1, repository.SaveCallCount);
        Assert.Equal(2, repository.SavedStudents.Count);
        Assert.Same(first, repository.SavedStudents[0]);
        Assert.Same(second, repository.SavedStudents[1]);
    }

    [Fact]
    public void Save_EmptyCollection_PassesEmptyCollectionToRepository()
    {
        StubStudentRepository repository = new();
        StudentService service = new(repository);

        service.Save();

        Assert.Equal(1, repository.SaveCallCount);
        Assert.Empty(repository.SavedStudents);
    }

    [Fact]
    public void Save_RepositoryFailure_PropagatesErrorAndPreservesStudents()
    {
        IOException expected = new("Cannot save student data.");

        StubStudentRepository repository = new()
        {
            SaveException = expected
        };

        StudentService service = new(repository);
        Student original = CreateStudent();
        service.AddStudent(original);

        IOException actual = Assert.Throws<IOException>(
            () => service.Save());

        Assert.Same(expected, actual);
        Assert.Equal(1, repository.SaveCallCount);
        Assert.Same(original, Assert.Single(service.GetStudents()));
    }

    private static Student CreateStudent(
        string studentId = "ST001",
        int course = 4,
        bool hasScholarship = false,
        decimal averageGrade = 75m,
        bool isExpelled = false)
    {
        return new Student(
            "Kovalenko",
            "Anna",
            course,
            studentId,
            hasScholarship,
            "AA",
            "123456",
            averageGrade,
            isExpelled);
    }
}