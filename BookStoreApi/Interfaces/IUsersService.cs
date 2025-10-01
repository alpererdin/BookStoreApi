using BookStoreApi.Models;
using BookStoreApi.Models.DTOs.Requests;
using System.Threading.Tasks;

namespace BookStoreApi.Interfaces;

public interface IUsersService : IMongoDbService<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User> Register(RegisterRequest request);
    Task<string?> Login(LoginRequest request);
}