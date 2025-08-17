using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using QueueService.API.Controllers;
using QueueService.Application.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueService.Tests.Integration
{
    public class QueueControllerIntegrationTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly ILogger<QueueController> _logger;
        private readonly QueueController _controller;

        public QueueControllerIntegrationTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _logger = new LoggerFactory().CreateLogger<QueueController>();
            _controller = new QueueController(_mediatorMock.Object, _logger);
        }

        [Fact]
        public async Task CreateQueue_Integration_ReturnsCreatedAtAction()
        {
            // Arrange
            var command = new CreateQueueItemCommand(
                Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var expectedId = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateQueueItemCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId);

            // Act
            var result = await _controller.CreateQueue(command, CancellationToken.None);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetById), createdResult.ActionName);

            var queueItemIdProperty = createdResult.Value.GetType().GetProperty("QueueItemId");
            var actualId = (Guid)queueItemIdProperty.GetValue(createdResult.Value);
            Assert.Equal(expectedId, actualId);
        }

        [Fact]
        public async Task MarkAsCalled_Integration_ReturnsNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<MarkQueueItemAsCalledCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.Empty);

            // Act
            var result = await _controller.MarkAsCalled(id, CancellationToken.None);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task MarkAsCompleted_Integration_ReturnsNoContent()
        {
            var id = Guid.NewGuid();
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<MarkQueueItemAsCompletedCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.Empty);

            var result = await _controller.MarkAsCompleted(id, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task MarkAsSkipped_Integration_ReturnsNoContent()
        {
            var id = Guid.NewGuid();
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<MarkQueueItemAsSkippedCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.Empty);

            var result = await _controller.MarkAsSkipped(id, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Cancel_Integration_ReturnsNoContent()
        {
            var id = Guid.NewGuid();
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CancelQueueItemCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.Empty);

            var result = await _controller.Cancel(id, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetById_Integration_ReturnsOk()
        {
            var id = Guid.NewGuid();

            var result = await _controller.GetById(id, CancellationToken.None);

            Assert.IsType<OkResult>(result);
        }
    }
}
