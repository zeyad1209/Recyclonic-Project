namespace RecyclonicApi.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Token { get; set; }

        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime? RevokedAt { get; set; }

        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
