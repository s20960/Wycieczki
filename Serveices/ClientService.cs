using Microsoft.EntityFrameworkCore;
using Wycieczki.Context;
using Wycieczki.Serveices.IServices;

namespace Wycieczki.Serveices
{
    public class ClientService : IClientService
    {
        public readonly ApbdCwicz9Context _context;

        public ClientService(ApbdCwicz9Context apbdCwicz9Context)
        {
            _context = apbdCwicz9Context;
        }

        public async Task DeleteClientAsync(int clientId)
        {
            var client1 = await _context.Clients
                .FirstAsync(x => x.IdClient == clientId)
                ?? throw new ArgumentException($"Client with ID: {clientId} not found");

            var client2 = await _context.Clients
                .Include(x => x.ClientTrips)
                .FirstAsync(x => x.IdClient == clientId);

            if (client2 != null)
                throw new ArgumentException($"Cannot delete client with Id: {clientId} with assigned trips.");

            _context.Clients.Remove(client2);
            await _context.SaveChangesAsync();
        }


    }
}
