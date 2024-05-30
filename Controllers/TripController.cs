using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wycieczki.Models.Dto;
using Wycieczki.Serveices;
using Wycieczki.Serveices.IServices;

namespace Wycieczki.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripController : ControllerBase
    {
        private readonly ILogger<TripController> _logger;
        private readonly ITripService _tripService;

        public TripController(ILogger<TripController> logger ,ITripService tripService)
        {
            _logger = logger;
            _tripService = tripService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResultDto<TripDto>>> GetTrips([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _tripService.GetTripsAsync(page, pageSize);
            return Ok(result);
        }

        [HttpPost("{idTrip}/{idClient}")]
        public async Task<IActionResult> AddClientToTrip(int idClient, int idTrip)
        {
            try
            {
                _logger.LogInformation("Added Client to Trip");
                await _tripService.AddClientToTripAsync(idClient, idTrip);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.InnerException.Message}");
            }
        }
    }
}
