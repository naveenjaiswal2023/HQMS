using AuthService.API.Controllers;
using AuthService.Application.Commands;
using AuthService.Application.Common.Models;
using AuthService.Application.DTOs.Auth;
using AuthService.Application.Queries;
using AuthService.Domain.Identity;
using AuthService.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using SharedInfrastructure.DTO;
using System.Security.Claims;
using Xunit;

namespace AuthService.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<IEmailSender> _emailSenderMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<AuthController>> _loggerMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();

            // Mock UserManager dependencies
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            _emailSenderMock = new Mock<IEmailSender>();
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<AuthController>>();

            _controller = new AuthController(
                _mediatorMock.Object,
                _userManagerMock.Object,
                _emailSenderMock.Object,
                _configurationMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Register_ValidCommand_ReturnsOkWithUserId()
        {
            // Arrange
            var command = new UserRegisterCommand();
            var expectedUserId = Guid.NewGuid().ToString();
            var result = Result<string>.Success(expectedUserId);

            _mediatorMock.Setup(m => m.Send(It.IsAny<UserRegisterCommand>(), default))
                        .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.Register(command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            dynamic value = okResult.Value;
            Assert.Equal(expectedUserId, value.GetType().GetProperty("UserId").GetValue(value));
        }

        [Fact]
        public async Task Login_ValidCommand_ReturnsOkWithToken()
        {
            // Arrange
            var command = new LoginCommand();
            var expectedToken = new TokenDto
            {
                Token = "test-jwt-token",
                RefreshToken = "test-refresh-token",
                Expiration = DateTime.UtcNow.AddHours(1)
            };
            var result = Result<TokenDto>.Success(expectedToken);

            _mediatorMock.Setup(m => m.Send(It.IsAny<LoginCommand>(), default))
                        .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.Login(command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(result, okResult.Value);
        }

        [Fact]
        public async Task Logout_AuthorizedUser_ReturnsOkWithMessage()
        {
            // Arrange
            var result = Result<bool>.Success(true);
            _mediatorMock.Setup(m => m.Send(It.IsAny<LogoutCommand>(), default))
                        .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.Logout();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            dynamic value = okResult.Value;
            Assert.Equal("Logged out successfully.", value.GetType().GetProperty("Message").GetValue(value));
        }

        [Fact]
        public void GetCurrentUser_AuthenticatedUser_ReturnsUserInfo()
        {
            // Arrange
            var userId = "test-user-id";
            var username = "testuser";
            var role = "User";

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, "test");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            // Act
            var result = _controller.GetCurrentUser();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.Equal(userId, value.GetType().GetProperty("UserId").GetValue(value));
            Assert.Equal(username, value.GetType().GetProperty("Username").GetValue(value));
            Assert.Equal(role, value.GetType().GetProperty("Role").GetValue(value));
        }

        [Fact]
        public async Task CheckUserExists_ValidQuery_ReturnsOkWithResult()
        {
            // Arrange
            var email = "test@example.com";
            var phoneNumber = "1234567890";
            var expectedResult = new CheckUserExistsDto
            {
                Email = email,
                PhoneNumber = phoneNumber
            };

            _mediatorMock.Setup(m => m.Send(It.Is<CheckUserExistsQuery>(q =>
                q.Email == email && q.PhoneNumber == phoneNumber), default))
                        .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.CheckUserExists(email, phoneNumber);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDto = Assert.IsType<CheckUserExistsDto>(okResult.Value);
            Assert.Equal(email, returnedDto.Email);
            Assert.Equal(phoneNumber, returnedDto.PhoneNumber);
        }

        [Fact]
        public async Task SendEmailConfirmationLink_ValidEmail_ReturnsOkMessage()
        {
            // Arrange
            var email = "test@example.com";
            var user = new ApplicationUser { Id = "user-id", Email = email };
            var token = "confirmation-token";

            _userManagerMock.Setup(um => um.FindByEmailAsync(email))
                           .ReturnsAsync(user);
            _userManagerMock.Setup(um => um.GenerateEmailConfirmationTokenAsync(user))
                           .ReturnsAsync(token);
            _emailSenderMock.Setup(es => es.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                           .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SendEmailConfirmationLink(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Confirmation email sent.", okResult.Value);

            _emailSenderMock.Verify(es => es.SendEmailAsync(
                email,
                "Confirm your email",
                It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task SendEmailConfirmationLink_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var email = "nonexistent@example.com";
            _userManagerMock.Setup(um => um.FindByEmailAsync(email))
                           .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _controller.SendEmailConfirmationLink(email);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task ConfirmEmail_ValidToken_ReturnsOkMessage()
        {
            // Arrange
            var userId = "user-id";
            var token = "valid-token";
            var user = new ApplicationUser { Id = userId };

            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                           .ReturnsAsync(user);
            _userManagerMock.Setup(um => um.ConfirmEmailAsync(user, token))
                           .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.ConfirmEmail(userId, token);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Email confirmed successfully.", okResult.Value);
        }

        [Fact]
        public async Task ConfirmEmail_InvalidToken_ReturnsBadRequest()
        {
            // Arrange
            var userId = "user-id";
            var token = "invalid-token";
            var user = new ApplicationUser { Id = userId };

            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                           .ReturnsAsync(user);
            _userManagerMock.Setup(um => um.ConfirmEmailAsync(user, token))
                           .ReturnsAsync(IdentityResult.Failed());

            // Act
            var result = await _controller.ConfirmEmail(userId, token);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid token.", badRequestResult.Value);
        }

        [Fact]
        public async Task ConfirmEmail_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var userId = "nonexistent-user-id";
            var token = "token";

            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                           .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _controller.ConfirmEmail(userId, token);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found.", notFoundResult.Value);
        }

        //[Fact]
        //public void GetInternalToken_ValidCredentials_ReturnsTokenResponse()
        //{
        //    // Arrange
        //    var request = new ClientCredentialsRequest
        //    {
        //        ClientId = "test-client",
        //        ClientSecret = "test-secret"
        //    };

        //    var clients = new List<ClientCredentialDto>
        //    {
        //        new ClientCredentialDto { ClientId = "test-client", ClientSecret = "test-secret" }
        //    };

        //    var configSectionMock = new Mock<IConfigurationSection>();
        //    configSectionMock.Setup(cs => cs.Get<List<ClientCredentialDto>>()).Returns(clients);

        //    _configurationMock.Setup(c => c.GetSection("InternalClients")).Returns(configSectionMock.Object);
        //    _configurationMock.Setup(c => c["JwtSettings:Key"]).Returns("super-secret-key-for-jwt-token-generation-minimum-256-bits");
        //    _configurationMock.Setup(c => c["JwtSettings:Issuer"]).Returns("AuthService");
        //    _configurationMock.Setup(c => c["JwtSettings:Audience"]).Returns("AuthService");
        //    _configurationMock.Setup(c => c["JwtSettings:ExpiryMinutes"]).Returns("60");

        //    // Act
        //    var result = _controller.GetInternalToken(request);

        //    // Assert
        //    var okResult = Assert.IsType<OkObjectResult>(result);
        //    var tokenResponse = Assert.IsType<InternalTokenResponse>(okResult.Value);
        //    Assert.NotNull(tokenResponse.AccessToken);
        //    Assert.True(tokenResponse.Expiration > DateTime.UtcNow);
        //}

        //[Fact]
        //public void GetInternalToken_InvalidCredentials_ReturnsUnauthorized()
        //{
        //    // Arrange
        //    var request = new ClientCredentialsRequest
        //    {
        //        ClientId = "invalid-client",
        //        ClientSecret = "invalid-secret"
        //    };

        //    var clients = new List<ClientCredentialDto>
        //    {
        //        new ClientCredentialDto { ClientId = "test-client", ClientSecret = "test-secret" }
        //    };

        //    var configSectionMock = new Mock<IConfigurationSection>();
        //    configSectionMock.Setup(cs => cs.Get<List<ClientCredentialDto>>()).Returns(clients);

        //    _configurationMock.Setup(c => c.GetSection("InternalClients")).Returns(configSectionMock.Object);

        //    // Act
        //    var result = _controller.GetInternalToken(request);

        //    // Assert
        //    var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        //    Assert.Equal("Invalid client credentials.", unauthorizedResult.Value);
        //}

        [Fact]
        public async Task Register_FailedCommand_ReturnsBadRequestWithError()
        {
            // Arrange
            var command = new UserRegisterCommand();
            var errorMessage = "Registration failed";
            var errorResult = Result<string>.Failure(errorMessage);

            _mediatorMock.Setup(m => m.Send(It.IsAny<UserRegisterCommand>(), default))
                        .ReturnsAsync(errorResult);

            // Act
            var actionResult = await _controller.Register(command);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
            Assert.Equal(errorMessage, badRequestResult.Value);
        }

        //[Fact]
        //public void GetInternalToken_NoClientsConfigured_ReturnsUnauthorized()
        //{
        //    // Arrange
        //    var request = new ClientCredentialsRequest
        //    {
        //        ClientId = "any-client",
        //        ClientSecret = "any-secret"
        //    };

        //    var configSectionMock = new Mock<IConfigurationSection>();
        //    configSectionMock.Setup(cs => cs.Get<List<ClientCredentialDto>>()).Returns((List<ClientCredentialDto>)null);

        //    _configurationMock.Setup(c => c.GetSection("InternalClients")).Returns(configSectionMock.Object);

        //    // Act
        //    var result = _controller.GetInternalToken(request);

        //    // Assert
        //    var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        //    Assert.Equal("Invalid client credentials.", unauthorizedResult.Value);
        //}
    }
}

// Additional test classes for integration testing
namespace AuthService.Tests.Integration
{
    public class AuthControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public AuthControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Register_ValidRequest_ReturnsSuccess()
        {
            // This would require setting up a test database and proper configuration
            // Implement based on your test infrastructure setup
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Integration test for login endpoint
            // Implement based on your test infrastructure setup
        }
    }
}

// Test data builders for complex objects
namespace AuthService.Tests.Builders
{
    public class UserRegisterCommandBuilder
    {
        private UserRegisterCommand _command = new UserRegisterCommand();

        public UserRegisterCommandBuilder WithEmail(string email)
        {
            // Set email property
            return this;
        }

        public UserRegisterCommandBuilder WithPassword(string password)
        {
            // Set password property  
            return this;
        }

        public UserRegisterCommand Build() => _command;
    }

    public class ApplicationUserBuilder
    {
        private ApplicationUser _user = new ApplicationUser();

        public ApplicationUserBuilder WithId(string id)
        {
            _user.Id = id;
            return this;
        }

        public ApplicationUserBuilder WithEmail(string email)
        {
            _user.Email = email;
            _user.UserName = email;
            return this;
        }

        public ApplicationUserBuilder WithEmailConfirmed(bool confirmed = true)
        {
            _user.EmailConfirmed = confirmed;
            return this;
        }

        public ApplicationUser Build() => _user;
    }
}