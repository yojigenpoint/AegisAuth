using YojigenPoint.Aegisauth.Domain.Entities;
using YojigenPoint.AegisAuth.Application.Abstractions;
using YojigenPoint.Security.Abstractions;

namespace YojigenPoint.AegisAuth.Application.Users.Commands;

public class RegisterUserCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    // The handler depends on abstractions, not concrete implementations.
    // This makes it highly testable.
    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        // 1. Check if a user with the given email already exists.
        var existingUser = await _userRepository.GetUserByEmailAsync(command.Email, cancellationToken);
        if (existingUser is not null)
        {
            // In a real application, you might throw a custom, more specific exception.
            throw new InvalidOperationException("A user with this email already exists.");
        }

        // 2. Hash the user's plain-text password using the injected service.
        var passwordHash = _passwordHasher.Hash(command.Password);

        // 3. Use the domain entity's factory method to create a new user object.
        var user = User.Create(command.Email, passwordHash);

        // 4. Add the new user to the repository. This marks it for insertion in the database.
        await _userRepository.AddUserAsync(user, cancellationToken);

        // 5. Commit the transaction. The Unit of Work saves all changes to the database.
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}