using server.Models;

namespace server.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAll();
        Task<User> GetById(string id);
        Task<User> GetByUserName(string name);
        Task<User> CreateUser(User user);
        Task<User> UpdateUser(User user);
        Task<bool> DeleteUser(string id);   
        Task<bool> Exist(string id);
        Task<bool> EmailExist(string email);
    }
}
