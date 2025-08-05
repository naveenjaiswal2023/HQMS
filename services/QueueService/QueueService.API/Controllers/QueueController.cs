using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QueueService.Application.Commands;

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
                var command = new CancelQueueItemCommand { QueueItemId = id };
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            // This should call a Query via MediatR to return the item.
            // Implement only if needed for UI or CreatedAtAction.
            return Ok(); // placeholder
        }
    }
}
