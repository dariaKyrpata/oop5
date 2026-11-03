using System.Text.Json;
using OOP5.BLL;

namespace OOP5.DAL;

public class JsonStudentRepository : IStudentRepository
{
    private readonly string filePath;

    private readonly JsonSerializerOptions options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public JsonStudentRepository(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException(
                "File path cannot be empty.",
                nameof(filePath));
        }

        this.filePath = Path.GetFullPath(filePath);
    }

    public IReadOnlyList<Student> Load()
    {
        string json = File.ReadAllText(filePath);

        List<StudentRecord?> records =
            JsonSerializer.Deserialize<List<StudentRecord?>>(json, options)
            ?? throw new InvalidDataException(
                "The file must contain a JSON array of students.");

        List<Student> students = new();

        foreach (StudentRecord? record in records)
        {
            if (record is null)
            {
                throw new InvalidDataException(
                    "The file contains a null student entry.");
            }

            Student student = new(
                record.LastName,
                record.FirstName,
                record.Course,
                record.StudentId,
                record.HasScholarship,
                record.PassportSeries,
                record.PassportNumber,
                record.AverageGrade,
                record.IsExpelled);

            students.Add(student);
        }

        return students.AsReadOnly();
    }

    public void Save(IReadOnlyList<Student> students)
    {
        ArgumentNullException.ThrowIfNull(students);

        List<StudentRecord> records = new();

        foreach (Student student in students)
        {
            if (student is null)
            {
                throw new ArgumentException(
                    "The student collection contains a null entry.",
                    nameof(students));
            }

            records.Add(new StudentRecord
            {
                LastName = student.LastName,
                FirstName = student.FirstName,
                Course = student.Course,
                StudentId = student.StudentId,
                HasScholarship = student.HasScholarship,
                PassportSeries = student.PassportSeries,
                PassportNumber = student.PassportNumber,
                AverageGrade = student.AverageGrade,
                IsExpelled = student.IsExpelled
            });
        }

        string json = JsonSerializer.Serialize(records, options);

        File.WriteAllText(filePath, json);
    }
}