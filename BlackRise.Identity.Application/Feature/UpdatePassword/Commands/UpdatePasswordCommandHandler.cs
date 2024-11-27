using BlackRise.Identity.Application.Contracts;
using MediatR;

namespace BlackRise.Identity.Application.Feature.UpdatePassword.Commands;

public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, UpdatePasswordDto>
{
    private readonly IAuthService _authService;
    public UpdatePasswordCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<UpdatePasswordDto> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.UpdateUserPasswordAsync(request.Email, request.Password);
        return new UpdatePasswordDto(result.Item2,result.Item1);
    }
}
