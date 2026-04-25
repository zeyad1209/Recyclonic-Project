using System.ComponentModel.DataAnnotations;

namespace RecyclonicApi.Models.DTOs
{
    public class Change_Password_Dto
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
    public class Reset_Password_Dto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string NewPassword { get; set; }

    }
}
