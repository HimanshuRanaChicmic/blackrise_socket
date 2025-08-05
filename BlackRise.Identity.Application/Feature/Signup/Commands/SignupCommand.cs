using MediatR;
using static BlackRise.Identity.Application.Utils.Enumerations;

namespace BlackRise.Identity.Application.Feature.Signup.Commands;

public class SignupCommand : IRequest<SignupDto>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string Email { get; set; }

    public DateOnly DateOfBirth { get; set; }
    public bool IsReceiveBlackRiseEmails { get; set; }
    public bool IsReconnectWithEmail {  get; set; }
    public ProfileTitle? Title { get; set; }
}
