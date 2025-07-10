using BlackRise.Identity.Application.Feature.User;
using BlackRise.Identity.Application.Feature.User.Commands.ProfileStatus;
using BlackRise.Identity.Application.Feature.User.Queries.UserDetail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlackRise.Identity.Controllers
{
    [Route("security-service/api/user")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser([FromRoute] Guid id)
        {
            return Ok(await _mediator.Send(new GetUserQuery { UserId = id }));
        }

        [HttpPost("profile-status")]
        public async Task<ActionResult<UserDto>> UpdateProfileStatus([FromBody] ProfileStatusCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
    }
}
