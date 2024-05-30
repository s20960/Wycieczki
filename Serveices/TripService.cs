using Microsoft.EntityFrameworkCore;
using Wycieczki.Context;
using Wycieczki.Models.Dto;
using Wycieczki.Models;
using Wycieczki.Serveices.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Wycieczki.Serveices
{
    public class TripService : ITripService
    {
        public readonly ApbdCwicz9Context _context;

        public TripService(ApbdCwicz9Context apbdCwicz9Context)
        {
            _context = apbdCwicz9Context;
        }

        //rozwiązanie przez Dto
        public async Task<PagedResultDto<TripDto>> GetTripsAsync(int pageNumber = 1, int pageSize = 10)
        {
            var totalTrips = await _context.Trips.CountAsync();
            var allPages = (int)Math.Ceiling(totalTrips / (double)pageSize);

            var trips = await _context.Trips
                .Include(x => x.IdCountries)
                .Include(x => x.ClientTrips)
                    .ThenInclude(x => x.IdClientNavigation)
                .OrderByDescending(x => x.DateFrom)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new TripDto
                {
                    Name = x.Name,
                    Description = x.Description,
                    DateFrom = x.DateFrom,
                    DateTo = x.DateTo,
                    MaxPeople = x.MaxPeople,
                    Countries = x.IdCountries
                        .Select(y => new CountryDto
                        {
                            Name = y.Name
                        })
                        .ToList(),
                    Clients = x.ClientTrips
                        .Select(z => new ClientDto
                        {
                            FirstName = z.IdClientNavigation.FirstName,
                            LastName = z.IdClientNavigation.LastName,
                        })
                        .ToList()
                })
                .ToListAsync();

            var result = new PagedResultDto<TripDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                AllPages = allPages,
                Items = trips
            };

            return result;
        }

        //w tym zadaniu trzeba stworzyć klienta w momencie dodawania go do wycieczki,
        //co uważam za nierozsądne, powinno się najpierw stworzyć klienta a dopiero potem dodać go do wycieczki,
        //w ten sposób nie musimy podawać wszystkich danych klienta za każdym razem jak chcemy uczestniczyć,
        //w kolejnej wycieczdze, mam nadzieję że takie rozwiązanie zostanie uznane za poprawne
        public async Task AddClientToTripAsync(int clientId, int tripId)
        {
            var client1 = await _context.Clients
                .FirstAsync(x => x.IdClient == clientId)
                ?? throw new ArgumentException($"Client with ID: {clientId} not found");

            var client2 = await _context.Clients
               .Include(x => x.ClientTrips)
               .FirstOrDefaultAsync(x => x.IdClient == clientId);

            if (client2 == null)
            {
                var trip = await _context.Trips
                    .FirstAsync(x => x.IdTrip == tripId)
                    ?? throw new ArgumentException($"Trip with ID: {tripId} not found");

                if(trip.DateFrom>DateTime.Now)
                    throw new ArgumentException($"Trip with ID: {tripId} not found");

                var clientTrip = new ClientTrip
                {
                    IdClient = clientId,
                    IdTrip = tripId,
                    RegisteredAt = DateTime.Now,
                    PaymentDate = null
                };

                _context.ClientTrips.Add(clientTrip);
                await _context.SaveChangesAsync();
            }
            else
                throw new ArgumentException($"Client with Id: {clientId} already assigned to trip with Id: {tripId}.");
        }

        //rozwiązanie przez klasy anonimowe
        //public async Task<object> GetTripAsyncAnonim(int pageNumber = 1, int pageSize = 10)
        //{
        //    var totalTrips = await _context.Trips.CountAsync();
        //    var allPages = (int)Math.Ceiling(totalTrips / (double)pageSize);

        //    var trips = await _context.Trips
        //        .Include(x => x.IdCountries)
        //        .Include(x => x.ClientTrips)
        //            .ThenInclude(x => x.IdClientNavigation)
        //        .OrderByDescending(x => x.DateFrom)
        //        .Skip((pageNumber - 1) * pageSize)
        //        .Take(pageSize)
        //        .Select(x=> new 
        //        {
        //            x.Name,
        //            x.Description,
        //            DateFrom = x.DateFrom.ToString("yyyy-MM-dd"),
        //            DateTo = x.DateTo.ToString("yyyy-MM-dd"),
        //            x.MaxPeople,
        //            Countries = x.IdCountries
        //                .Select(y => new 
        //                {
        //                    y.Name
        //                }),
        //            Clients = x.ClientTrips
        //                .Select(z => new
        //                {
        //                    z.IdClientNavigation.FirstName,
        //                    z.IdClientNavigation.LastName
        //                })

        //        })
        //        .ToListAsync();

        //    var result = new PagedResultDto<TripDto>
        //    {
        //        pageNum = pageNumber,
        //        pageSize = pageSize,
        //        allPages = allPages,
        //        trips = trips
        //    };

        //    return result;
        //}
    }
}
