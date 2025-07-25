using MediatR;
using Microsoft.AspNetCore.Mvc;
using QueueService.Application.Commands;

namespace QueueService.API.Controllers
{
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
        public async Task<IActionResult> CreateQueue([FromBody] CreateQueueItemCommand command)
        {
            if (command == null)
                return BadRequest("Command is null.");

            try
            {
                var result = await _mediator.Send(command);
                return Ok(new { QueueItemId = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create queue item.");
                return StatusCode(500, "An error occurred while creating the queue.");
            }
        }
    }
}
