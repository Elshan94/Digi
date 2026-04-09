using DigitalSalaryService.Models.Common;
using FluentValidation;
using Serilog;
using System.Net;
using System.Text.Json;

namespace DigitalSalaryService.Middlewares
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            Log.Error(exception, "Global exception occured");
            HttpStatusCode statusCode;
            object response;

            using (var scope = _serviceProvider.CreateScope())
            {
                switch (exception)
                {
                    case ValidationException validationException:
                        statusCode = HttpStatusCode.BadRequest;

                        Dictionary<string, HashSet<string>> validationErros = new Dictionary<string, HashSet<string>>();

                        foreach (var errGroup in validationException.Errors.GroupBy(m => m.PropertyName))
                        {
                            var propertyErrorMessages = new HashSet<string>();

                            foreach (var item in errGroup)
                                propertyErrorMessages.Add(item.ErrorMessage);

                            validationErros.Add(errGroup.Key, propertyErrorMessages);
                        }

                        response = new ErrorModel("Validation failed", validationErros);

                        break;
                    case ApplicationException applicaitonException:
                        {
                            statusCode = HttpStatusCode.BadRequest;
                            response = new ErrorModel(applicaitonException.Message);
                            break;
                        }
                    default:
                        statusCode = HttpStatusCode.InternalServerError;
                        response = new ErrorModel("WeвЂ™re unable to create your deposit account at the moment. Please try again later!");
                        break;
                }
            }



            context.Response.StatusCode = (int)statusCode;
            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
