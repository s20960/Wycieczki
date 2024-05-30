using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wycieczki.Serveices.IServices;

namespace Wycieczki.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly ILogger<ClientController> _logger;
        private readonly IClientService _clientService;

        public ClientController(ILogger<ClientController> logger,IClientService clientService)
        {
            _logger = logger;
            _clientService = clientService;
        }

        [HttpDelete("{idClient}")]
        public async Task<IActionResult> DeleteClient(int idClient)
        {
            try
            {
                _logger.LogInformation("Asked moderator for intervention");
                await _clientService.DeleteClientAsync(idClient);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.InnerException.Message}");
            }
        }

    }
}
