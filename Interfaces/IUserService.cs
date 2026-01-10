using server.DTOs;
using server.Models;

namespace server.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAll();
        Task<UserResponseDto> GetById(string id);
        Task<UserResponseDto> GetByUserName(string name);
        Task<UserResponseDto> CreateUser(UserCreatDto user);
        Task<UserResponseDto> UpdateUser(UserUpdateDto user);
        Task<bool> DeleteUser(string id);
        Task<bool> Exist(string id);
        Task<bool> EmailExist(string email);
    }
}
