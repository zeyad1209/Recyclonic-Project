namespace RecyclonicApi.Models.DTOs
{
    public class CachedUserData
    {
        public RegisterDto User { get; set; }
        public string Code { get; set; }
    }
    public class CachedUserresetpasswordData
    {
        public Guid UserId { get; set; }
        public string Code { get; set; }
    }
}
