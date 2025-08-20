using MediatR;
using Microsoft.AspNetCore.Mvc;
using YojigenPoint.AegisAuth.Api.Contracts;
using YojigenPoint.AegisAuth.Application.Users.Commands;

namespace YojigenPoint.AegisAuth.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ISender _mediator;

        public AuthController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var command = new RegisterUserCommand
            {
                Email = request.Email,
                Password = request.Password
            };

            // Send the command to the Application layer via MediatR
            await _mediator.Send(command, cancellationToken);

            // Upon success, returns an HTTP 201 Created status.
            // In a real app, you might return the created user's ID or a location header.
            return Created();
        }
    }
}
