using BusService;
using BusService.Contracts;

namespace AuthService.Service.Interface
{
    public interface IEventSyncService : ISyncService<EventContract, EventContract>
    {
    }
}
