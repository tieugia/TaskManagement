using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.DTOs;

public class RegisterRequestDto
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public UserRole Role { get; set; }
}
