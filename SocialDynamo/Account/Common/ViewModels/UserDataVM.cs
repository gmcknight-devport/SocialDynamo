namespace Account.API.Common.ViewModels
{
    public record UserDataVM
    {
        public string UserId { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public byte[] ProfilePicture { get; set; }
    }
}
