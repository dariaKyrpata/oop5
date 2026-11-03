namespace OOP5.BLL;

public interface IStudentRepository
{
    IReadOnlyList<Student> Load();

    void Save(IReadOnlyList<Student> students);
}