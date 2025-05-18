using System.Threading.Tasks;

namespace Video_Library_Api.Repositories
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
    }
}