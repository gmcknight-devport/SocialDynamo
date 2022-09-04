namespace Account.API.Authentication.Authentication.Commands
{
    public interface IAuthCommand<in T, TResult>
    {
        Task<TResult> HandleCommandAsync(T command);
    }
}
