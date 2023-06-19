using System.ComponentModel.DataAnnotations;

namespace SocialDynamoAPI.BaseAggregator.ViewModels
{
    public class UserDataVM
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Forename { get; set; }
        [Required]
        public string Surname { get; set; }
        [MaxLength(15 * 1024 * 1024)]
        public byte[] ProfilePicture { get; set; } = new byte[1];
    }
}
