using BookStoreApi.Models;
using BookStoreApi.Models.DTOs.Requests;
using Microsoft.Extensions.Configuration; 
using Microsoft.IdentityModel.Tokens; 
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;  
using System.Security.Claims;  
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BookStoreApi.Interfaces;

namespace BookStoreApi.Services;

public class UsersService : MongoDbService<User>, IUsersService
{
    private readonly IConfiguration _configuration;

    public UsersService(IMongoDatabase database, IConfiguration configuration) : base(database)
    {
        _configuration = configuration;
    }

    public async Task<User?> GetByUsernameAsync(string username) =>
        await _collection.Find(x => x.Username == username).FirstOrDefaultAsync();

    public async Task<User> Register(RegisterRequest request)
    {
        CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

        var user = new User
        {
            Username = request.Username,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        await CreateAsync(user);
        return user;
    }

    public async Task<string?> Login(LoginRequest request)
    {
        var user = await GetByUsernameAsync(request.Username);
        if (user is null || !VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            return null;  
        }

        string token = CreateToken(user);
        return token;
    }

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512(passwordSalt))
        {
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id!),
            new Claim(ClaimTypes.Name, user.Username),
        };

        var appSettingsToken = _configuration.GetSection("AppSettings:Token").Value;
        if (appSettingsToken is null)
            throw new Exception("AppSettings Token is null!");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettingsToken));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}