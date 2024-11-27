using BlackRise.Identity.Application.Feature.EmailConfirmation;
using BlackRise.Identity.Application.Feature.EmailConfirmation.Commands;
using BlackRise.Identity.Application.Feature.ForgotPassword;
using BlackRise.Identity.Application.Feature.ForgotPassword.Commands;
using BlackRise.Identity.Application.Feature.Login;
using BlackRise.Identity.Application.Feature.Login.Commands;
using BlackRise.Identity.Application.Feature.ResetPassword;
using BlackRise.Identity.Application.Feature.ResetPassword.Commands;
using BlackRise.Identity.Application.Feature.Signup;
using BlackRise.Identity.Application.Feature.Signup.Commands;
using BlackRise.Identity.Application.Feature.UpdatePassword;
using BlackRise.Identity.Application.Feature.UpdatePassword.Commands;
using BlackRise.Identity.Application.Feature.VerifyResetPasswordCode;
using BlackRise.Identity.Application.Feature.VerifyResetPasswordCode.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlackRise.Identity.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginDto>> Login([FromBody] LoginCommand loginCommand)
        {
            return await _mediator.Send(loginCommand);
        }

        [HttpPost("signup")]
        public async Task<ActionResult<SignupDto>> Signup([FromBody] SignupCommand signupCommand)
        {
            return await _mediator.Send(signupCommand);
        }

        [HttpPost("forgot")]
        public async Task<ActionResult<ForgotPasswordDto>> Forgot([FromBody] ForgotPasswordCommand forgotPasswordCommand)
        {
            return await _mediator.Send(forgotPasswordCommand);
        }

        [HttpPost("email-confirmation")]
        public async Task<ActionResult<EmailConfirmationDto>> ConfirmEmail([FromBody] EmailConfirmationCommand emailConfirmationCommand)
        {
            return await _mediator.Send(emailConfirmationCommand);
        }

        [HttpPost("update-password")]
        public async Task<ActionResult<UpdatePasswordDto>> UpdatePasword([FromBody] UpdatePasswordCommand updatePasswordCommand)
        {
            return await _mediator.Send(updatePasswordCommand);
        }

        [HttpPost("verify-reset-code")]
        public async Task<ActionResult<VerifyResetPasswordCodeDto>> VerifyResetPasswordCode([FromBody] VerifyResetPasswordCodeCommand verifyResetPasswordCodeCommand)
        {
            return await _mediator.Send(verifyResetPasswordCodeCommand);
        }

        [HttpPost("reset")]
        public async Task<ActionResult<ResetPasswordDto>> ResetPassword([FromBody] ResetPasswordCommand resetPasswordCommand)
        {
            return await _mediator.Send(resetPasswordCommand);
        }
    }
}
