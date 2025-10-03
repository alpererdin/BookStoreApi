using Xunit;
using Moq;
using BookStoreApi.Controllers;
using BookStoreApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BookStoreApi.Models.DTOs.Requests;
using BookStoreApi.Models;

namespace BookStoreApi.Tests;

public class AuthControllerTests
{
    private readonly Mock<IUsersService> _usersServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _usersServiceMock = new Mock<IUsersService>();
        _controller = new AuthController(_usersServiceMock.Object);
    }

    [Fact]
    public async Task Register_WithNewUsername_ReturnsOkResultWithUser()
    {
        // Arrange 
        var request = new RegisterRequest
        {
            Username = "testuser",
            Password = "password123"
        };
        var createdUser = new User
        {
            Id = "user-123",
            Username = "testuser",
            PasswordHash = new byte[0],
            PasswordSalt = new byte[0]
        };

        _usersServiceMock.Setup(s => s.GetByUsernameAsync(request.Username)).ReturnsAsync((User?)null);
        _usersServiceMock.Setup(s => s.Register(request)).ReturnsAsync(createdUser);

        // Act  
        var result = await _controller.Register(request);

        // Assert 
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUser = Assert.IsType<User>(okResult.Value);
        Assert.Equal("testuser", returnedUser.Username);
        Assert.Equal("user-123", returnedUser.Id);
    }
}