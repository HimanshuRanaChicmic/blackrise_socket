using BlackRise.Identity.Application.Feature.EmailConfirmation;
using BlackRise.Identity.Application.Feature.EmailConfirmation.Commands;
using BlackRise.Identity.Application.Feature.ForgotPassword;
using BlackRise.Identity.Application.Feature.ForgotPassword.Commands;
using BlackRise.Identity.Application.Feature.Login;
using BlackRise.Identity.Application.Feature.Login.Commands.EmailPassword;
using BlackRise.Identity.Application.Feature.Login.Commands.LinkedIn;
using BlackRise.Identity.Application.Feature.Login.Queries.LinkedIn;
using BlackRise.Identity.Application.Feature.ResendEmailConfirmation;
using BlackRise.Identity.Application.Feature.ResendEmailConfirmation.Commands;
using BlackRise.Identity.Application.Feature.ResendResetPassword;
using BlackRise.Identity.Application.Feature.ResendResetPassword.Commands;
using BlackRise.Identity.Application.Feature.ResetPassword;
using BlackRise.Identity.Application.Feature.ResetPassword.Commands;
using BlackRise.Identity.Application.Feature.Signup;
using BlackRise.Identity.Application.Feature.Signup.Commands;
using BlackRise.Identity.Application.Feature.UpdatePassword;
using BlackRise.Identity.Application.Feature.UpdatePassword.Commands;
using BlackRise.Identity.Application.Feature.VerifyResetPasswordCode;
using BlackRise.Identity.Application.Feature.VerifyResetPasswordCode.Commands;
using BlackRise.Identity.Persistence.Settings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlackRise.Identity.Controllers
{
    [Route("security-service/api/auth")]
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

        [HttpGet("linkedIn/callback")]
        public async Task<IActionResult> LinkedInCallback([FromQuery] string code, [FromQuery] string state)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Authorization code not found.");
            }

            try
            {
                
                return Ok(new { accessToken = "" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("linkedin-url")]
        public async Task<ActionResult> GetRecommendedGroups()
        {
            string response = await _mediator.Send(new GetLinkedInQuery {});
            return Ok(response);
        }


        [HttpPost("login/linkedin")]
        public async Task<ActionResult<LoginDto>> LoginLinkedin([FromBody] LinkedInCommand linkedInCommand)
        {
            return await _mediator.Send(linkedInCommand);
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

        [HttpPost("resend-email-confirmation")]
        public async Task<ActionResult<ResendEmailConfirmationDto>> ResendConfirmEmail([FromBody] ResendEmailConfirmationCommand resendEmailConfirmationCommand)
        {
            return await _mediator.Send(resendEmailConfirmationCommand);
        }

        [HttpPost("resend-reset-password-code")]
        public async Task<ActionResult<ResendResetPasswordDto>> ResendResetPasswordCode([FromBody] ResendResetPasswordCommand resendResetPasswordCommand)
        {
            return await _mediator.Send(resendResetPasswordCommand);
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
