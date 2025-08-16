using YojigenPoint.Aegisauth.Domain.Entities;
using FluentAssertions;

namespace Tests.Domain
{
    public class EntityTests
    {
        [Fact]
        public void User_Creation_ShouldSuccess_ForValidInputs()
        {
            // Arrange
            string email = "test@example.com";
            string passwordHash = "a_valid_password_hash";

            // Act
            User user = User.Create(email, passwordHash);

            // Assert
            user.Should().NotBeNull(); // Ensure user was created
            user.Email.Should().Be(email); // Check email was set correctly
            user.PasswordHash.Should().Be(passwordHash); // Check password hash was set correctly
            user.Id.Should().NotBe(Guid.Empty); // Check a new Guid was generated
        }

        [Theory]
        [InlineData(null)] // Null email
        [InlineData("   ")] // Whitespace email
        [InlineData("")] // Empty email
        public void User_Creation_ShouldThrowException_ForInvalidEmail(string? email)
        {
            // Arrange
            string passwordHash = "a_valid_password_hash@123";

            // Act
            var act = () => User.Create(email, passwordHash);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Email cannot be null or empty.*")
                .WithParameterName(nameof(email));
        }

        [Theory]
        [InlineData(null)] // Null password hash
        [InlineData("   ")] // Whitespace password hash
        [InlineData("")] // Empty password hash
        public void User_Creation_ShouldThrowException_ForInvalidPasswordHash(string? passwordHash)
        {
            // Arrange
            string email = "test@example.com";

            // Act
            var act = () => User.Create(email, passwordHash);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Password hash cannot be null or empty.*")
                .WithParameterName(nameof(passwordHash));
        }
    }
}
