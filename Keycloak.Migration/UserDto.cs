namespace Keycloak.Migration
{
    public record UserDto(string PortalUserName, string FirstName, string? LastName, string? Email)
    {
        public bool? UserCreatedByProcess { get; set; }
        public string? TemporaryGeneratedPassword { get; set; }
    }
}
