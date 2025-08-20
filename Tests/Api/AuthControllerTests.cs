using Microsoft.AspNetCore.Mvc.Testing;
using Yojigen.AegisAuth.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NSubstitute;
using Microsoft.Extensions.DependencyInjection;
using YojigenPoint.AegisAuth.Api.Contracts;
using System.Net.Http.Json;
using FluentAssertions;
using System.Net;
using YojigenPoint.AegisAuth.Application.Users.Commands;
using NSubstitute.ExceptionExtensions;

namespace Tests.Api
{
    public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _httpClient;
        private readonly ISender _mockMediator = Substitute.For<ISender>();

        public AuthControllerTests(WebApplicationFactory<Program> factory)
        {
            _httpClient = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton(_mockMediator);
                });
            }).CreateClient();
        }

        [Fact]
        public async Task Register_ShouldReturn201Created_ForValidRequest()
        {
            // Arrange
            var request = new RegisterUserRequest("test@example.com", "Password123!");

            // Act
            var response = await _httpClient.PostAsJsonAsync("/api/auth/register", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Register_ShouldReturn400BadRequest_ForInvalidModel()
        {
            // Arrange
            // Create an invalid request with a null email
            var request = new RegisterUserRequest(null!, "Password123!");

            // Act
            var response = await _httpClient.PostAsJsonAsync("/api/auth/register", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Register_ShouldReturn409Conflict_WhenEmailExists()
        {
            // Arrange
            var request = new RegisterUserRequest("test@example.com", "Password123!");

            //Set up the mock mediator to throw the specific exception our handler would
            _mockMediator.Send(Arg.Any<RegisterUserCommand>(), Arg.Any<CancellationToken>())
                         .ThrowsAsync(new InvalidOperationException("Email already exists."));

            // Act
            var response = await _httpClient.PostAsJsonAsync("/api/auth/register", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }
    }
}
