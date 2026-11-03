using System.Globalization;
using System.Text.Json;
using OOP5.BLL;
using OOP5.DAL;

namespace OOP5.PL;

public static class Program
{
    public static void Main()
    {
        string filePath = Path.Combine(
            AppContext.BaseDirectory,
            "students.json");

        IStudentRepository repository = new JsonStudentRepository(filePath);
        StudentService service = new(repository);

        Console.WriteLine("OOP LAB 5 / VARIANT 14");
        Console.WriteLine($"Data file: {filePath}");

        try
        {
            service.Load();
            Console.WriteLine("Students loaded successfully.");
        }
        catch (Exception exception) when (IsExpectedError(exception))
        {
            Console.WriteLine($"Loading error: {exception.Message}");
            Console.WriteLine("You can add students or try loading again.");
        }

        while (true)
        {
            PrintMenu();

            string choice = Console.ReadLine()?.Trim() ?? "0";

            if (choice == "0")
            {
                Console.WriteLine("Program completed.");
                return;
            }

            try
            {
                ExecuteChoice(choice, service);
            }
            catch (Exception exception) when (IsExpectedError(exception))
            {
                Console.WriteLine($"Error: {exception.Message}");
            }
        }
    }

    private static void PrintMenu()
    {
        Console.WriteLine();
        Console.WriteLine("1. Show students");
        Console.WriteLine("2. Add student");
        Console.WriteLine("3. Remove student");
        Console.WriteLine("4. Count fourth-year students without a scholarship");
        Console.WriteLine("5. Expel student for poor performance");
        Console.WriteLine("6. Load students from file");
        Console.WriteLine("7. Save students to file");
        Console.WriteLine("8. Draw");
        Console.WriteLine("9. Postman operations");
        Console.WriteLine("10. Mountaineer operations");
        Console.WriteLine("0. Exit");
        Console.WriteLine("Changes are saved only with option 7.");
        Console.Write("Your choice: ");
    }

    private static void ExecuteChoice(
        string choice,
        StudentService service)
    {
        switch (choice)
        {
            case "1":
                ShowStudents(service);
                break;

            case "2":
                AddStudent(service);
                break;

            case "3":
                RemoveStudent(service);
                break;

            case "4":
                int count =
                    service.CountFourthYearStudentsWithoutScholarship();

                Console.WriteLine(
                    $"Active fourth-year students without a scholarship: {count}");
                break;

            case "5":
                ExpelStudent(service);
                break;

            case "6":
                if (ReadBoolean("Replace current students with file data?"))
                {
                    service.Load();
                    Console.WriteLine("Students loaded successfully.");
                }
                break;

            case "7":
                service.Save();
                Console.WriteLine("Students saved successfully.");
                break;

            case "8":
                Draw(service);
                break;

            case "9":
                RunPostmanOperations();
                break;

            case "10":
                RunMountaineerOperations();
                break;

            default:
                Console.WriteLine("Invalid menu option.");
                break;
        }
    }

    private static void ShowStudents(StudentService service)
    {
        IReadOnlyList<Student> students = service.GetStudents();

        if (students.Count == 0)
        {
            Console.WriteLine("The student list is empty.");
            return;
        }

        foreach (Student student in students)
        {
            Console.WriteLine();
            Console.WriteLine($"ID: {student.StudentId}");
            Console.WriteLine(
                $"Name: {student.LastName} {student.FirstName}");
            Console.WriteLine($"Course: {student.Course}");
            Console.WriteLine(
                $"Scholarship: {student.HasScholarship}");
            Console.WriteLine(
                $"Passport: {student.PassportSeries} {student.PassportNumber}");
            Console.WriteLine(
                $"Average grade: {student.AverageGrade.ToString(CultureInfo.InvariantCulture)}");
            Console.WriteLine($"Expelled: {student.IsExpelled}");
        }
    }

    private static void AddStudent(StudentService service)
    {
        string lastName = ReadText("Last name: ");
        string firstName = ReadText("First name: ");
        int course = ReadInteger("Course (1-6): ");
        string studentId = ReadText("Student ID: ");
        bool hasScholarship = ReadBoolean("Receives a scholarship?");
        string passportSeries = ReadText("Passport series: ");
        string passportNumber = ReadText("Passport number: ");
        decimal averageGrade = ReadDecimal("Average grade (0-100): ");

        Student student = new(
            lastName,
            firstName,
            course,
            studentId,
            hasScholarship,
            passportSeries,
            passportNumber,
            averageGrade);

        service.AddStudent(student);

        Console.WriteLine("Student added successfully.");
    }

    private static void RemoveStudent(StudentService service)
    {
        string studentId = ReadText("Student ID: ");
        bool removed = service.RemoveStudent(studentId);

        Console.WriteLine(
            removed
                ? "Student removed successfully."
                : "Student not found.");
    }

    private static void ExpelStudent(StudentService service)
    {
        string studentId = ReadText("Student ID: ");

        bool expelled =
            service.ExpelStudentForPoorPerformance(studentId);

        Console.WriteLine(
            expelled
                ? "Student expelled. Scholarship cancelled."
                : "Student is already expelled or has a grade of at least 60.");
    }

    private static void Draw(StudentService service)
    {
        Console.WriteLine("1. Student");
        Console.WriteLine("2. Postman");
        Console.WriteLine("3. Mountaineer");

        string choice = ReadText("Your choice: ");
        IDrawable drawable;

        switch (choice)
        {
            case "1":
                string studentId = ReadText("Student ID: ");

                drawable = service.GetStudents().FirstOrDefault(student =>
                    string.Equals(
                        student.StudentId,
                        studentId,
                        StringComparison.OrdinalIgnoreCase))
                    ?? throw new KeyNotFoundException("Student not found.");
                break;

            case "2":
                drawable = new Postman(
                    "Alex",
                    "P001",
                    "Central district",
                    new HikingService(new HikingConditions(false, false)));
                break;

            case "3":
                drawable = new Mountaineer(
                    "Maria",
                    "M001",
                    "Hoverla",
                    new HikingService(new HikingConditions(false, false)));
                break;

            default:
                Console.WriteLine("Invalid drawing option.");
                return;
        }

        Console.WriteLine(drawable.Draw());
    }

    private static void RunPostmanOperations()
    {
        string name = ReadText("Postman name: ");
        string employeeId = ReadText("Employee ID: ");
        string deliveryArea = ReadText("Delivery area: ");
        HikingService hikingService = ReadHikingService();

        Postman postman = new(
            name,
            employeeId,
            deliveryArea,
            hikingService);

        if (ReadBoolean("Deliver one letter?"))
        {
            postman.DeliverLetter();
        }

        bool completed = postman.TryGoHiking();

        Console.WriteLine(
            completed
                ? "Hike completed successfully."
                : "Hike denied. Equipment and good weather are required.");

        Console.WriteLine(
            $"Delivered letters: {postman.DeliveredLetters}");
        Console.WriteLine(
            $"Completed hikes: {postman.HikesCompleted}");
    }

    private static void RunMountaineerOperations()
    {
        string name = ReadText("Mountaineer name: ");
        string climberId = ReadText("Climber ID: ");
        string mountain = ReadText("Mountain: ");
        HikingService hikingService = ReadHikingService();

        Mountaineer mountaineer = new(
            name,
            climberId,
            mountain,
            hikingService);

        bool completed = mountaineer.TryGoHiking();

        Console.WriteLine(
            completed
                ? "Climb completed successfully."
                : "Climb denied. Equipment and good weather are required.");

        Console.WriteLine(
            $"Completed climbs: {mountaineer.ClimbsCompleted}");
    }

    private static HikingService ReadHikingService()
    {
        bool hasEquipment = ReadBoolean("Has the required equipment?");
        bool isGoodWeather = ReadBoolean("Is the weather good?");

        IHikingConditions conditions = new HikingConditions(
            hasEquipment,
            isGoodWeather);

        return new HikingService(conditions);
    }

    private static string ReadText(string prompt)
    {
        Console.Write(prompt);

        return Console.ReadLine()?.Trim()
            ?? throw new EndOfStreamException("Input ended.");
    }

    private static int ReadInteger(string prompt)
    {
        string input = ReadText(prompt);

        if (!int.TryParse(
            input,
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out int value))
        {
            throw new ArgumentException("Enter a valid whole number.");
        }

        return value;
    }

    private static decimal ReadDecimal(string prompt)
    {
        string input = ReadText(prompt).Replace(',', '.');

        if (!decimal.TryParse(
            input,
            NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture,
            out decimal value))
        {
            throw new ArgumentException("Enter a valid decimal number.");
        }

        return value;
    }

    private static bool ReadBoolean(string prompt)
    {
        string input = ReadText($"{prompt} (y/n): ").ToLowerInvariant();

        return input switch
        {
            "y" or "yes" => true,
            "n" or "no" => false,
            _ => throw new ArgumentException("Enter y for yes or n for no.")
        };
    }

    private static bool IsExpectedError(Exception exception)
    {
        return exception is ArgumentException
            or IOException
            or UnauthorizedAccessException
            or JsonException
            or InvalidOperationException
            or KeyNotFoundException;
    }
}