using BusService;
using Polly;

namespace AuthService.Messaging
{
    public class EventMessageBusService : MessageBusHostedService
    {
        public EventMessageBusService(IMessageBusService serviceBus, IServiceScopeFactory serviceScopeFactory) : base(serviceBus, serviceScopeFactory)
        {
        }

        protected override void ConfigureSubscribers()
        {
            var policy = BuildPolicy();
        }

        private Policy BuildPolicy()
        {
            return Policy
                    .Handle<Exception>()
                    .WaitAndRetry(5, _ => TimeSpan.FromSeconds(5), (exception, _, _, _) =>
                    {});
        }
    }
}