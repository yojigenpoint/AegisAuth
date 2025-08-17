namespace YojigenPoint.AegisAuth.Application.Users.Commands;

public class RegisterUserCommand
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}