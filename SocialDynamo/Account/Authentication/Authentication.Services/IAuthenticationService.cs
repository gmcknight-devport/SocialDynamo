using Account.API.Commands;
using Account.API.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace Account.API.Services
{
    public interface IAuthenticationService
    {
        Task<IActionResult> HandleCommandAsync(LoginUserCommand command);
        Task HandleCommandAsync(LogoutUserCommand command);
        Task<IActionResult> HandleCommandAsync(RefreshJwtTokenCommand command);
        Task HandleCommandAsync(RegisterUserCommand command);
        string HashPassword(string password);
    }
}