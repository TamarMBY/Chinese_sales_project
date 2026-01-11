using server.Models;

namespace server.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAll();
        Task<User> GetById(string id);
        Task<User> GetByUserName(string name);
        Task<User> AddUser(User user);
        Task<User> UpdateUser(User user);
        Task<bool> DeleteUser(string id);
        Task<bool> Exists(string id);
        Task<bool> UserNameExists(string userName);
    }
}
