using AppointmentService.Application.Commands;
using AppointmentService.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentService.API.Controllers
{
    [Authorize(Policy = "InternalPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(IMediator mediator, ILogger<AppointmentsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Get upcoming appointments between two dates.
        /// </summary>
        /// <param name="from">Start date (inclusive)</param>
        /// <param name="to">End date (inclusive)</param>
        /// <returns>List of appointments</returns>
        [HttpGet("GetUpcomingAppointments")]
        public async Task<IActionResult> GetUpcomingAppointments([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            try
            {
                if (from > to)
                    return BadRequest("From date must be before To date");

                var query = new GetUpcomingAppointmentsQuery(from, to);
                var result = await _mediator.Send(query);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting upcoming appointments.");
                return StatusCode(500, "Internal server error occurred.");
            }
            
        }

        [HttpPut("{id}/UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] bool status)
        {
            await _mediator.Send(new UpdateAppointmentStatusCommand(id, status));
            return NoContent();
        }
    }
}
