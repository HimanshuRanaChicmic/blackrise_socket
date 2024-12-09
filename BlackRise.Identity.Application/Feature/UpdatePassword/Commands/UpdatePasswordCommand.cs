using MediatR;

namespace BlackRise.Identity.Application.Feature.UpdatePassword.Commands;

public class UpdatePasswordCommand : IRequest<UpdatePasswordDto>
{
    public string Email { get; set; }
    public string Password { get; set; }
}
