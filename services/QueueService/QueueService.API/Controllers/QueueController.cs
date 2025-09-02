using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QueueService.Application.Commands;
using QueueService.Application.Queries;

namespace QueueService.API.Controllers
{
    [Authorize(Policy = "UserPolicy")]
    [ApiController]
    [Route("api/[controller]")]
    public class QueueController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<QueueController> _logger;

        public QueueController(IMediator mediator, ILogger<QueueController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Creates a queue item from appointment, doctor, patient, etc.
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateQueue([FromBody] CreateQueueItemCommand command, CancellationToken cancellationToken)
        {
            if (command == null)
                return BadRequest("Command is null.");

            try
            {
                var result = await _mediator.Send(command, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = result }, new { QueueItemId = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create queue item.");
                return StatusCode(500, "An error occurred while creating the queue.");
            }
        }

        [HttpPut("{id}/call")]
        public async Task<IActionResult> MarkAsCalled(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var command = new MarkQueueItemAsCalledCommand(id, DateTime.UtcNow);
                await _mediator.Send(command, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark queue item as called.");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}/complete")]
        public async Task<IActionResult> MarkAsCompleted(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var command = new MarkQueueItemAsCompletedCommand(id, DateTime.UtcNow);
                await _mediator.Send(command, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark queue item as completed.");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}/skip")]
        public async Task<IActionResult> MarkAsSkipped(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var command = new MarkQueueItemAsSkippedCommand(id, DateTime.UtcNow);
                await _mediator.Send(command, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to skip queue item.");
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Cancel a queue item.
        /// </summary>
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var command = new CancelQueueItemCommand(id);

                await _mediator.Send(command, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cancel queue item.");
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// (Optional) Get queue item by ID (used in CreatedAtAction).
        /// This should be implemented if you return CreatedAtAction.
        /// </summary>
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetById(Guid id)
        //{
        //    var result = await _mediator.Send(new GetQueueDetailsQuery(id));
        //    return Ok(result);
        //}

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetQueueByIdQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllQueueDetailsQuery());
            return Ok(result);
        }

        [HttpGet("GetFilteredQueueItemsAsync")]
        public async Task<IActionResult> GetAll(
            [FromQuery] Guid hospitalId,
            [FromQuery] Guid departmentId,
            [FromQuery] Guid? doctorId = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var query = new GetAllQueueDetailsQuery
            {
                HospitalId = hospitalId,
                DepartmentId = departmentId,
                DoctorId = doctorId,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }
}
