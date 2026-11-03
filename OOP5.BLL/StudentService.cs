namespace OOP5.BLL;

public class StudentService
{
    private readonly IStudentRepository repository;
    private List<Student> students = new();

    public StudentService(IStudentRepository repository)
    {
        this.repository = repository
            ?? throw new ArgumentNullException(nameof(repository));
    }

    public IReadOnlyList<Student> GetStudents()
    {
        return students.AsReadOnly();
    }

    public void AddStudent(Student student)
    {
        ArgumentNullException.ThrowIfNull(student);

        if (students.Any(existing =>
            string.Equals(
                existing.StudentId,
                student.StudentId,
                StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException(
                "A student with this ID already exists.",
                nameof(student));
        }

        students.Add(student);
    }

    public bool RemoveStudent(string studentId)
    {
        Student? student = FindStudent(studentId);

        if (student is null)
        {
            return false;
        }

        students.Remove(student);

        return true;
    }

    public int CountFourthYearStudentsWithoutScholarship()
    {
        return students.Count(student =>
            student.Course == 4 &&
            !student.HasScholarship &&
            !student.IsExpelled);
    }

    public bool ExpelStudentForPoorPerformance(string studentId)
    {
        Student? student = FindStudent(studentId);

        if (student is null)
        {
            throw new KeyNotFoundException("Student not found.");
        }

        return student.ExpelForPoorPerformance();
    }

    public void Load()
    {
        IReadOnlyList<Student> loadedStudents = repository.Load()
            ?? throw new InvalidOperationException(
                "The repository returned no student collection.");

        List<Student> validatedStudents = new();
        HashSet<string> studentIds = new(StringComparer.OrdinalIgnoreCase);

        foreach (Student student in loadedStudents)
        {
            if (student is null)
            {
                throw new InvalidOperationException(
                    "The student collection contains a null entry.");
            }

            if (!studentIds.Add(student.StudentId))
            {
                throw new InvalidOperationException(
                    "The student collection contains duplicate IDs.");
            }

            validatedStudents.Add(student);
        }

        students = validatedStudents;
    }

    public void Save()
    {
        repository.Save(students.AsReadOnly());
    }

    private Student? FindStudent(string studentId)
    {
        if (string.IsNullOrWhiteSpace(studentId))
        {
            throw new ArgumentException(
                "Student ID cannot be empty.",
                nameof(studentId));
        }

        string normalizedId = studentId.Trim();

        return students.FirstOrDefault(student =>
            string.Equals(
                student.StudentId,
                normalizedId,
                StringComparison.OrdinalIgnoreCase));
    }
}