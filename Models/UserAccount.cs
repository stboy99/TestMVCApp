namespace TestMVCApp.Models
{
    public class UserAccount
    {
        public int Id { get; set; }

        public required string Username { get; set; }

        public required string PasswordHash { get; set; }
    }
}