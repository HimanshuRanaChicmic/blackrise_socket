using MediatR;

namespace BlackRise.Identity.Application.Feature.User.Queries.UserDetail
{
    public class GetUserQuery : IRequest<UserDto>
    {
        public Guid UserId { get; set; }
    }
}
