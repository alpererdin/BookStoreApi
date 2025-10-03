using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using BookStoreApi.Controllers;
using BookStoreApi.Interfaces;
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
    public async Task Register_WhenUsernameIsAvailable_ReturnsOkResult()
    {
        // Arrange
        var request = new RegisterRequest { Username = "newuser", Password = "password" };
        _usersServiceMock.Setup(s => s.GetByUsernameAsync(request.Username)).ReturnsAsync((User?)null);
        _usersServiceMock.Setup(s => s.Register(request)).ReturnsAsync(new User { Id = "1", Username = "newuser", PasswordHash = new byte[0], PasswordSalt = new byte[0] });

        // Act
        var result = await _controller.Register(request);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Register_WhenUsernameIsTaken_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterRequest { Username = "existinguser", Password = "password" };
        _usersServiceMock.Setup(s => s.GetByUsernameAsync(request.Username)).ReturnsAsync(new User { Id = "1", Username = "existinguser", PasswordHash = new byte[0], PasswordSalt = new byte[0] });

        // Act
        var result = await _controller.Register(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Username already exists.", badRequestResult.Value);
    }

 
    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkResultWithToken()
    {
        // Arrange
        var request = new LoginRequest { Username = "testuser", Password = "password" };
        _usersServiceMock.Setup(s => s.Login(request)).ReturnsAsync("a-valid-jwt-token");

        // Act
        var result = await _controller.Login(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);  
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var request = new LoginRequest { Username = "wronguser", Password = "wrongpassword" };
        _usersServiceMock.Setup(s => s.Login(request)).ReturnsAsync((string?)null);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Invalid username or password.", unauthorizedResult.Value);
    }
}