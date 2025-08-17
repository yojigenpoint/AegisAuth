using NSubstitute;
using YojigenPoint.Aegisauth.Domain.Entities;
using YojigenPoint.AegisAuth.Application.Abstractions;
using YojigenPoint.AegisAuth.Application.Users.Commands;
using YojigenPoint.Security.Abstractions;

namespace Tests.Application
{
    public class RegisterUserCommandHandlerTests
    {
        // The mock dependencies
        private readonly IUserRepository _mockUserRepository;
        private readonly IPasswordHasher _mockPasswordHasher;
        private readonly IUnitOfWork _mockUnitOfWork;

        // The handler to be tested
        private readonly RegisterUserCommandHandler _handler;

        public RegisterUserCommandHandlerTests()
        {
            // Initialize the mock dependencies
            _mockUserRepository = Substitute.For<IUserRepository>();
            _mockPasswordHasher = Substitute.For<IPasswordHasher>();
            _mockUnitOfWork = Substitute.For<IUnitOfWork>();

            // Create the handler with the mocked dependencies
            _handler = new RegisterUserCommandHandler(
                _mockUserRepository,
                _mockPasswordHasher,
                _mockUnitOfWork);
        }

        [Fact]
        public  async Task Handle_ShouldSucceed_WhenEmailIsUnique()
        {
            // Arrange
            var command = new RegisterUserCommand
            {
                Email = "test@example.com",
                Password = "Password123"
            };

            _mockUserRepository.GetUserByEmailAsync(command.Email, default)
                .Returns((User?)null);

            _mockPasswordHasher.Hash(command.Password).Returns("hashed_password");

            // Act
            await _handler.Handle(command, default);

            // Assert
            await _mockUserRepository.Received(1).AddUserAsync(Arg.Any<User>(), default);
            await _mockUnitOfWork.Received(1).SaveChangesAsync(default);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenEmailIsNotUnique()
        {
            // Arrange
            var command = new RegisterUserCommand
            {
                Email = "test@example.com",
                Password = "Password123"
            };

            var existingUser = User.Create(command.Email, "some_hash");

            _mockUserRepository.GetUserByEmailAsync(command.Email, default)
                .Returns(existingUser);

            // Act
            Func<Task> act = () => _handler.Handle(command, default);

            // Assert
            await _mockUserRepository.DidNotReceive().AddUserAsync(Arg.Any<User>(), default);
            await _mockUnitOfWork.DidNotReceive().SaveChangesAsync(default);
        }
    }
}
