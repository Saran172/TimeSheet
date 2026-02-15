using Task_1.Entities;

namespace Task_1.Interface
{
    public interface IWorkService
    {
        Task<List<Work>> GetWorkToday(int employeeId);
    }
}
