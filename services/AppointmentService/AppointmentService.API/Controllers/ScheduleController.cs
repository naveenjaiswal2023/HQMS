using AppointmentService.Application.Commands;
using AppointmentService.Application.DTOs;
using AppointmentService.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ScheduleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ScheduleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("doctor/{doctorId:guid}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateDoctorSchedule(
            Guid doctorId,
            CreateDoctorScheduleRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateDoctorScheduleCommand(
                doctorId,
                request.DayOfWeek,
                request.StartTime,
                request.EndTime,
                request.SlotDurationMinutes);

            await _mediator.Send(command, cancellationToken);
            return Created("", null);
        }

        [HttpGet("doctor/{doctorId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<DoctorScheduleDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DoctorScheduleDto>>> GetDoctorSchedule(
            Guid doctorId,
            CancellationToken cancellationToken)
        {
            var query = new GetDoctorScheduleQuery(doctorId);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }
}
