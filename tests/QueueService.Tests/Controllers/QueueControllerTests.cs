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

namespace QueueService.Tests.Controllers
{
    public class QueueControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<QueueController>> _loggerMock;
        private readonly QueueController _controller;

        public QueueControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<QueueController>>();
            _controller = new QueueController(_mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateQueue_ReturnsCreatedAtAction_WhenSuccessful()
        {
            // Arrange
            var command = new CreateQueueItemCommand(
                Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var expectedId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId);

            // Act
            var result = await _controller.CreateQueue(command, CancellationToken.None);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            //Assert.Equal(nameof(_controller.GetById), createdResult.ActionName);
            //Assert.Equal(expectedId, ((dynamic)createdResult.Value).QueueItemId);

            var queueItemIdProperty = createdResult.Value.GetType().GetProperty("QueueItemId");
            var actualId = (Guid)queueItemIdProperty.GetValue(createdResult.Value);
            Assert.Equal(expectedId, actualId);
        }

        [Fact]
        public async Task CreateQueue_ReturnsBadRequest_WhenCommandIsNull()
        {
            // Act
            var result = await _controller.CreateQueue(null, CancellationToken.None);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Command is null.", badRequest.Value);
        }

        [Fact]
        public async Task CreateQueue_ReturnsServerError_WhenExceptionThrown()
        {
            // Arrange
            var command = new CreateQueueItemCommand(
                Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test error"));

            // Act
            var result = await _controller.CreateQueue(command, CancellationToken.None);

            // Assert
            var errorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, errorResult.StatusCode);
            Assert.Equal("An error occurred while creating the queue.", errorResult.Value);
        }

        [Fact]
        public async Task MarkAsCalled_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<MarkQueueItemAsCalledCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.Empty);

            // Act
            var result = await _controller.MarkAsCalled(id, CancellationToken.None);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task MarkAsCalled_ReturnsServerError_WhenExceptionThrown()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<MarkQueueItemAsCalledCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test error"));

            // Act
            var result = await _controller.MarkAsCalled(id, CancellationToken.None);

            // Assert
            var errorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, errorResult.StatusCode);
            Assert.Equal("Test error", errorResult.Value);
        }

        [Fact]
        public async Task MarkAsCompleted_ReturnsNoContent_WhenSuccessful()
        {
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<MarkQueueItemAsCompletedCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.Empty);

            var result = await _controller.MarkAsCompleted(id, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task MarkAsSkipped_ReturnsNoContent_WhenSuccessful()
        {
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<MarkQueueItemAsSkippedCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.Empty);

            var result = await _controller.MarkAsSkipped(id, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Cancel_ReturnsNoContent_WhenSuccessful()
        {
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<CancelQueueItemCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.Empty);

            var result = await _controller.Cancel(id, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetById_ReturnsOk()
        {
            var id = Guid.NewGuid();

            var result = await _controller.GetById(id);

            Assert.IsType<OkResult>(result);
        }
    }
}
