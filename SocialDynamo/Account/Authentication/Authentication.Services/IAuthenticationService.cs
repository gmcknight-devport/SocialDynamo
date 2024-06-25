using Common.API.Commands;
using Common.API.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace Common.API.Services
{
    public interface IAuthenticationService
    {
        Task<IActionResult> HandleCommandAsync(LoginUserCommand command, HttpContext httpContext);
        Task HandleCommandAsync(LogoutUserCommand command);
        Task<IActionResult> HandleCommandAsync(RefreshJwtTokenCommand command, HttpContext httpContext);
        Task HandleCommandAsync(RegisterUserCommand command);
        string HashPassword(string password);
    }
}