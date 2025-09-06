using AppointmentService.Application.Commands;
using AppointmentService.Application.DTOs;
using AppointmentService.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentService.API.Controllers
{
    //[Authorize(Policy = "UserPolicy")]
    
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

        [HttpPost]
        [ProducesResponseType(typeof(CreateAppointmentResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CreateAppointmentResponse>> CreateAppointment(
            CreateAppointmentRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateAppointmentCommand(
                request.Title,
                request.Description,
                request.StartDateTime,
                request.EndDateTime,
                request.PatientId,
                request.DoctorId,
                request.HospitalId,
                request.Location,
                request.Type,
                request.Fee,
                request.SendReminder,
                request.ReminderMinutesBefore);

            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetAppointment), new { id = result.Id }, result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(GetAppointmentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetAppointmentResponse>> GetAppointment(
            Guid id,
            CancellationToken cancellationToken)
        {
            var query = new GetAppointmentQuery(id);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:guid}/details")]
        [ProducesResponseType(typeof(AppointmentWithDetailsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AppointmentWithDetailsResponse>> GetAppointmentWithDetails(
            Guid id,
            CancellationToken cancellationToken)
        {
            var query = new GetAppointmentWithDetailsQuery(id);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(UpdateAppointmentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UpdateAppointmentResponse>> UpdateAppointment(
            Guid id,
            UpdateAppointmentRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateAppointmentCommand(
                id,
                request.Title,
                request.Description,
                request.StartDateTime,
                request.EndDateTime,
                request.Location,
                request.Notes);

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAppointment(
            Guid id,
            CancellationToken cancellationToken)
        {
            var command = new DeleteAppointmentCommand(id);
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id:guid}/complete")]
        [ProducesResponseType(typeof(CompleteAppointmentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CompleteAppointmentResponse>> CompleteAppointment(
            Guid id,
            CompleteAppointmentRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CompleteAppointmentCommand(id, request.Notes);
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpGet("current")]
        [ProducesResponseType(typeof(CurrentAppointmentsResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CurrentAppointmentsResponse>> GetCurrentAppointments(
            CancellationToken cancellationToken)
        {
            var query = new GetCurrentAppointmentsQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("patient/{patientId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<AppointmentSummaryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AppointmentSummaryDto>>> GetPatientAppointments(
            Guid patientId,
            [FromQuery] bool upcomingOnly = false,
            CancellationToken cancellationToken = default)
        {
            var query = new GetPatientAppointmentsQuery(patientId, upcomingOnly);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("doctor/{doctorId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<AppointmentSummaryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AppointmentSummaryDto>>> GetDoctorAppointments(
            Guid doctorId,
            [FromQuery] bool upcomingOnly = false,
            CancellationToken cancellationToken = default)
        {
            var query = new GetDoctorAppointmentsQuery(doctorId, upcomingOnly);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("doctor/{doctorId:guid}/available-slots")]
        [ProducesResponseType(typeof(GetAvailableSlotsResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<GetAvailableSlotsResponse>> GetAvailableSlots(
            Guid doctorId,
            [FromQuery] DateTime date,
            CancellationToken cancellationToken)
        {
            var query = new GetAvailableSlotsQuery(doctorId, date);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get upcoming appointments between two dates.
        /// </summary>
        /// <param name="from">Start date (inclusive)</param>
        /// <param name="to">End date (inclusive)</param>
        /// <returns>List of appointments</returns>
        
        [Authorize(Policy = "InternalPolicy")]
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [Authorize(Policy = "InternalPolicy")]
        [HttpPut("{id}/UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] bool status)
        {
            await _mediator.Send(new UpdateAppointmentStatusCommand(id, status));
            return NoContent();
        }
    }
}
