namespace BlackRise.Identity.Application.Feature.User
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AppleId { get; set; }
        public bool IsProfileCreated { get; set; }
        public bool IsProfileCompleted { get; set; }
    }
}
