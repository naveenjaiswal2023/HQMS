using MediatR;
using Microsoft.AspNetCore.Mvc;
using PatientService.Application.Commands;
using PatientService.Application.DTOs;
using PatientService.Application.Exceptions;
using PatientService.Application.Queries;

namespace PatientService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PatientsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register-with-payment")]
        public async Task<IActionResult> RegisterWithPayment([FromBody] RegisterPatientWithPaymentCommand command, CancellationToken cancellationToken)
        {
            if (command == null)
                return BadRequest("Invalid request payload.");

            try
            {
                var result = await _mediator.Send(command, cancellationToken);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("{patientId}/complete-registration")]
        public async Task<IActionResult> CompleteRegistration(Guid patientId, CancellationToken cancellationToken)
        {
            var command = new CompletePatientRegistrationCommand(patientId);

            try
            {
                var result = await _mediator.Send(command, cancellationToken);

                if (!result.Succeeded)
                {
                    var errorMessage = result.Errors?.FirstOrDefault() ?? "Failed to complete registration.";
                    return StatusCode(500, new { success = false, message = errorMessage });

                }

                return Ok(new { success = true, data = result.Data });
            }
            catch (PatientService.Application.Exceptions.NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPatientById(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetPatientByIdQuery(id);
            try
            {
                var patient = await _mediator.Send(query, cancellationToken);
                return Ok(patient);
            }
            catch (PatientService.Application.Exceptions.NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetAllPatients()
        {
            var query = new GetAllPatientsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
