using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DulcesDuendesApp.Common.Exceptions
{
    public class AppExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is not AppException ex) return;

            context.Result = new BadRequestObjectResult(new { mensaje = ex.Message });
            context.ExceptionHandled = true;
        }
    }
}
