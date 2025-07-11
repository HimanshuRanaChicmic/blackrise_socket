using BlackRise.Identity.Application.Feature.EmailConfirmation;
using BlackRise.Identity.Application.Feature.EmailConfirmation.Commands;
using BlackRise.Identity.Application.Feature.ForgotPassword;
using BlackRise.Identity.Application.Feature.ForgotPassword.Commands;
using BlackRise.Identity.Application.Feature.Login;
using BlackRise.Identity.Application.Feature.Login.Commands.Apple;
using BlackRise.Identity.Application.Feature.Login.Commands.EmailPassword;
using BlackRise.Identity.Application.Feature.Login.Commands.Google;
using BlackRise.Identity.Application.Feature.Login.Commands.LinkedIn;
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
using Microsoft.Extensions.Options;

namespace BlackRise.Identity.Controllers
{
    [Route("security-service/api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ClientUrlSetting _clienturlSettings;
        public AuthController(IMediator mediator, IOptions<ClientUrlSetting> clienturlSettings)
        {
            _mediator = mediator;
            _clienturlSettings = clienturlSettings.Value;

        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginDto>> Login([FromBody] LoginCommand loginCommand)
        {
            return await _mediator.Send(loginCommand);
        }

        [HttpGet("login/linkedin/callback")]
        public async Task<IActionResult> Callback([FromQuery] string code)
        {
            try
            {
                var linkedInCommand = new LinkedInCommand { AccessToken = code };
                var result = await _mediator.Send(linkedInCommand);
                var isprofileCreated = result.IsProfileCreated == true ? "true" : "false";
                Console.WriteLine($"isprofileCreated... : {isprofileCreated}");
                return Redirect($"{_clienturlSettings.LoginRedirect}?token={result.Token}&userId={result.UserId}&email={result.Email ?? ""}&firstname={result.FirstName ?? ""}&lastname={result.LastName ?? ""}&isProfileCreated={isprofileCreated}");
            }
            catch (Exception ex)
            {
                return Redirect($"{_clienturlSettings.LoginRedirect}?errorMessage={Uri.EscapeDataString(ex.Message)}");
            }
            
        }

        [HttpPost("login/google")]
        public async Task<LoginDto> GoogleLogin([FromBody] GoogleCommand googleCommand)
        {
            return await _mediator.Send(googleCommand);
        }
        [HttpPost("login/apple")]
        public async Task<LoginDto> AppleLogin([FromBody] AppleCommand appleCommand)
        {
            return await _mediator.Send(appleCommand);
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
