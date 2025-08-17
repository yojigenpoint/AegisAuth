using YojigenPoint.Aegisauth.Domain.Entities;

namespace YojigenPoint.AegisAuth.Application.Abstractions;

public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email of the user to find.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The User entity if found; otherwise, null.</returns>
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new user to the data store.
    /// </summary>
    /// <param name="user">The user entity to add.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task AddUserAsync(User user, CancellationToken cancellationToken = default);
}