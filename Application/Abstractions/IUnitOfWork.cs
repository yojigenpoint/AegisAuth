namespace YojigenPoint.AegisAuth.Application.Abstractions;

public interface IUnitOfWork
{
    /// <summary>
    /// Saves all changes made in this context to the underlying database.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the saving operation.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}