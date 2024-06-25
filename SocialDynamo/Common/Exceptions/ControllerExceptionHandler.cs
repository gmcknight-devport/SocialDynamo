using System.Security;
using Microsoft.AspNetCore.Mvc;

namespace Common.Exceptions
{
    public static class ControllerExceptionHandler
    {
        private static readonly List<Type> exceptions = new()
        {
            typeof(NullReferenceException),
            typeof(ArgumentNullException),
            typeof(SecurityException),
            typeof(InvalidUserStateException),
            typeof(HttpRequestException),

        };
      
        public static IActionResult HandleException(Exception ex)
        {
            foreach (Type x in exceptions)
            {
                if (ex.GetType() == x)
                {
                    return new BadRequestObjectResult(ex.Message);
                }
            }

            return new StatusCodeResult(500);
        }
    }
}
