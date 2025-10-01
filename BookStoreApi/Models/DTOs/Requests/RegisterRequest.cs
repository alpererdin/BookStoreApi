namespace BookStoreApi.Models.DTOs.Requests;

public class RegisterRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}