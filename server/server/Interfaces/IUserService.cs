using server.DTOs;
using server.Models;

namespace server.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAll();
        Task<UserResponseDto> GetById(string id);
        //Task<UserResponseDto> GetByUserName(string name);
        Task<UserResponseDto> AddUser(UserCreateDto user);
        Task<UserResponseDto> UpdateUser(string id, UserUpdateDto user);
        Task<bool> DeleteUser(string id);
        Task<LoginResponseDto> Authenticate(string userName, string password);
    }
}
