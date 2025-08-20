namespace YojigenPoint.AegisAuth.Api.Contracts
{
    // Using a 'record' provides immutability and value-based equality for simplicity.
    public record RegisterUserRequest(string Email, string Password);
}
