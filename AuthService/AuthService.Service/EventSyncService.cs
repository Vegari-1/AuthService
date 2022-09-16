using BusService;
using BusService.Contracts;
using BusService.Routing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AuthService.Service.Interface;
using System.Text;

namespace AuthService.Service
{
    public class EventSyncService : ConsumerBase<EventContract, EventContract>, IEventSyncService
    {
        private readonly IMessageBusService _messageBusService;

        public EventSyncService(IMessageBusService messageBusService, ILogger<EventSyncService> logger) : base(logger)
        {
            _messageBusService = messageBusService;
        }

        public override Task PublishAsync(EventContract entity, string action)
        {
            var serialized = JsonConvert.SerializeObject(entity);
            var bData = Encoding.UTF8.GetBytes(serialized);
            _messageBusService.PublishEvent(SubjectBuilder.Build(Topics.Event, action), bData);
            return Task.CompletedTask;
        }

        public override Task SynchronizeAsync(EventContract entity, string action)
        {
            throw new NotImplementedException();
        }
    }
}
