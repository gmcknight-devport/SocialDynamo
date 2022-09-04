using Account.Exceptions;

namespace Account.ValueObjects
{
    public record EmailAddress
    {
        public string Email { get; init; }

        internal EmailAddress(string value)
        {
            Email = value;
        }

        public static EmailAddress Create(string value)
        {
            Validate(value);
            return new EmailAddress(value);
        }

        private static void Validate(string value)
        {
            if(!value.Contains("@") && !value.Contains(".")){
                throw new InvalidUserStateException("Email address must contain '@' and '.' characters");

            }else if (value.Split("@")[0].Length > 64)
            {
                throw new InvalidUserStateException("Username section must not exceed 64 characters");

            }else if (value.Split("@")[1].Length > 255)
            {
                throw new InvalidUserStateException("Domain part of address must not exceed 255 characters");
            }
        }
    }
}
