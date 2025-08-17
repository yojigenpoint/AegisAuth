using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using YojigenPoint.Aegisauth.Domain.Entities;
using YojigenPoint.AegisAuth.Infrastructure.Persistence;
using YojigenPoint.AegisAuth.Infrastructure.Persistence.Repositories;

namespace Tests.Infrastructure
{
    public class AppDbContextTests : DatabaseTestBase
    {
        [Fact]
        public async Task UserRepository_AdddUserAsync_ShouldSaveUser()
        {
            // Arrange
            var userRepository = new UserRepository(DbContext);
            var newUser = User.Create("test@example.com", "hashed_password");

            // Act
            await userRepository.AddUserAsync(newUser);
            await DbContext.SaveChangesAsync(); // The Unit of work commit

            // Assert
            var savedUser = await DbContext.Users.FindAsync(newUser.Id);
            savedUser.Should().NotBeNull();
            savedUser.Email.Should().Be("test@example.com");
        }

        [Fact]
        public async Task DbContext_ShouldSetAudiFields_OnCreate()
        {
            // Arrange
            var newUser = User.Create("audit@example.com", "hashed_password");

            // Act
            await DbContext.Users.AddAsync(newUser);
            await DbContext.SaveChangesAsync(); // The Unit of work commit

            // Assert
            var savedUser = await DbContext.Users.FindAsync(newUser.Id);
            savedUser.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            savedUser.ModifiedAtUtc.Should().Be(savedUser.CreatedAtUtc);
        }

        [Fact]
        public async Task DbContext_ShouldUpdateModifiedAt_OnUpdate()
        {
            // Arrange
            var user = User.Create("update@example.com", "hashed_password");
            await DbContext.Users.AddAsync(user);
            await DbContext.SaveChangesAsync(); // The Unit of work commit

            var initialCreationTime = user.CreatedAtUtc;
            await Task.Delay(10); // Ensure the timestamp will be different

            // Act
            user.Email = "updated@example.com";
            DbContext.Users.Update(user);
            await DbContext.SaveChangesAsync(); // The Unit of work commit

            // Assert
            var updatedUser = await DbContext.Users.FindAsync(user.Id);
            updatedUser.ModifiedAtUtc.Should().BeAfter(initialCreationTime);
            updatedUser.CreatedAtUtc.Should().Be(initialCreationTime);
        }

        [Fact]
        public async Task DbContext_ShouldSoftDeleteUser()
        {
            // Arrange
            var user = User.Create("delete@example.com", "hashed_password");
            await DbContext.Users.AddAsync(user);
            await DbContext.SaveChangesAsync(); // The Unit of work commit

            // Act
            DbContext.Users.Remove(user);
            await DbContext.SaveChangesAsync(); // The Unit of work commit

            // Assert
            using (var assertContext = new AppDbContext(Options))
            {
                var userFromStandardQuery = await assertContext.Users.FindAsync(user.Id);
                userFromStandardQuery.Should().BeNull();
            }

            var userFromDirectQuery = await DbContext.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == user.Id);
            userFromDirectQuery.Should().NotBeNull();
            userFromDirectQuery.DeletedAtUtc.Should().NotBeNull();
            userFromDirectQuery.DeletedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }
    }
}
