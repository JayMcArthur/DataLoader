namespace DataLoader.Repositories.Models
{
    internal class User
    {
        public string Username { get; set; } = string.Empty;
        public AuthToken AuthToken { get; set; } = new();
    }

    internal class AuthToken
    {
        public string Token { get; set;} = string.Empty;
        public long? EnvironmentId { get; set; }
        public string? EnvironmentName { get; set; }
    }
}
