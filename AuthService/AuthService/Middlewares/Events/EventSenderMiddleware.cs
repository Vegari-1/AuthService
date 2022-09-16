using System.Net;
using BusService.Contracts;
using Microsoft.Extensions.Options;
using AuthService.Service.Interface;
using AuthService.Service.Interface.Exceptions;

namespace AuthService.Middlewares.Events
{
    public class EventSenderMiddleware
	{
        private readonly RequestDelegate _next;
        private readonly AppConfig _appConfig;

        public EventSenderMiddleware(RequestDelegate next, IOptions<AppConfig> options)
        {
            _next = next;
            _appConfig = options.Value;
        }

        public async Task Invoke(HttpContext context, IEventSyncService eventSyncService)
        {
            try
            {
                await _next(context);

                if (context.Request.Method != "GET")
                {
                    int statusCode = context.Response.StatusCode;
                    var request = context.Request;
                    string actionName = request.RouteValues["action"].ToString();
                    string actionPath = request.Path.Value;
                    string message = String.Format("{0} at '{1}'", actionName, actionPath);
                    SendEvent(eventSyncService, context, statusCode, message);
                }
            }
            catch (BaseException ae)
            {
                SendEvent(eventSyncService, context, statusCode: ae.StatusCode, message: ae.Message);
                throw;
            }
            catch (Exception e)
            {
                SendEvent(eventSyncService, context, statusCode: 500,
                    message: "An unexpected error has occured: " + e.ToString());
                throw;
            }
        }

        private void SendEvent(IEventSyncService eventSyncService, 
            HttpContext context, int statusCode, string message)
        {
            EventContract contract = new EventContract(
                DateTime.Now,
                _appConfig.Name,
                context.Request.Method,
                message,
                statusCode,
                ((HttpStatusCode)statusCode).ToString()
            );
            eventSyncService.PublishAsync(contract, BusService.Events.Created);
        }
    }

    public static class EventSenderMiddlewareExtensions
    {
        public static IApplicationBuilder UseEventSenderMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<EventSenderMiddleware>();
        }
    }
}

