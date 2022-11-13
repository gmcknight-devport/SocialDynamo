namespace Account.API.ViewModels
{
    public record ProfileInformationVM
    {
        public string Forename { get; set; }

        public string Surname { get; set; }

        public string ProfileDescription { get; set; }

        public int NumberOfFollowers { get; set; }

        public byte[] ProfilePicture { get; set; }
    }
}
