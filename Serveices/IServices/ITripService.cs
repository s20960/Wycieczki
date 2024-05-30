using Wycieczki.Models.Dto;

namespace Wycieczki.Serveices.IServices
{
    public interface ITripService
    {
        public Task<PagedResultDto<TripDto>> GetTripsAsync(int pageNumber = 1, int pageSize = 10);
        public Task AddClientToTripAsync(int clientId, int tripId);
    }
}
