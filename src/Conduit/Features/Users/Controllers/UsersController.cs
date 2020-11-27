using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Conduit.Features.Users
{
    [Route("users")]
    public class UsersController
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<UserEnvelope> Create([FromBody] Create.Command command)
        {
            return await _mediator.Send(command);
        }


        [HttpPost("login")]
        public async Task<UserEnvelope> Login([FromBody] Login.Command command)
        {
            return await _mediator.Send(command);
        }

        [HttpPut("confirmAccount/{email}/{code}")]
        public async Task ConfirmAccount(string email, string code)
        {
            await _mediator.Send(new ConfirmAccount.Query(email, code));
        }

        [HttpGet("remindPassword/{email}")]
        public async Task RemindPassword(string email)
        {
            await _mediator.Send(new RemindPassword.Query(email));
        }

        [HttpGet("verifyCodeToChangePassword/{email}/{code}")]
        public async Task VerifyCodeToChangePassword(string email, string code)
        {
            await _mediator.Send(new VerifyCodeToChangePassword.Query(email, code));
        }

        [HttpPut("changePassword/{email}/{newPassword}")]
        public async Task ChangePassword(string email, string newPassword)
        {
            await _mediator.Send(new ChangePassword.Query(email, newPassword));
        }
    }
}