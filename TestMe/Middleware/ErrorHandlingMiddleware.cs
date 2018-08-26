using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TestMe.Exceptions;

namespace TestMe.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError; 
            var result = "";
            if (exception is TestNotFoundException)
            {
                code = HttpStatusCode.NotFound;
            }
            else if (exception is QuestionNotFoundException ||
                exception is AnswerNotFoundException ||
                exception is UserNameNotFoundException ||
                exception is UserAnswersException)
            {
                code = HttpStatusCode.BadRequest;
                result = JsonConvert.SerializeObject(new { error = exception.Message });
            }

            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
