using server.Models;

namespace server.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAll();
        Task <Category> GetById(int id);
        Task<Category> AddCategory(Category category);
        Task<bool> DeleteCategory(int id);
        Task<Category> UpdateCategory(Category category);
    }
}
