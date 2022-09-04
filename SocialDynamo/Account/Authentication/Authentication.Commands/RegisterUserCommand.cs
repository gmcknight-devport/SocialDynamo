namespace Account.API.Commands
{
    public class RegisterUserCommand
    {        
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
    }
}
