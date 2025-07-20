using AuthService.Application.Commands;
using AuthService.Application.DTOs.Auth;
using AuthService.Application.Queries;
using AuthService.Domain.Identity;
using AuthService.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Web;

namespace AuthService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public AuthController(IMediator mediator,UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _mediator = mediator;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterCommand command)
        {
            var userId = await _mediator.Send(command);
            return Ok(new { UserId = userId });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var token = await _mediator.Send(command);
            return Ok(token);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _mediator.Send(new LogoutCommand());
            return Ok(new { Message = "Logged out successfully." });
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = User.Identity?.Name;

            return Ok(new
            {
                UserId = userId,
                Username = username,
                Role = User.FindFirstValue(ClaimTypes.Role)
            });
        }

        [HttpGet("check-user")]
        public async Task<IActionResult> CheckUserExists([FromQuery] string email, [FromQuery] string phoneNumber)
        {
            var result = await _mediator.Send(new CheckUserExistsQuery(email, phoneNumber));
            return Ok(result);
        }

        [HttpPost("send-confirmation-email")]
        public async Task<IActionResult> SendEmailConfirmationLink([FromBody] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("User not found.");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(token);
            var confirmationLink = $"https://your-client.com/confirm-email?userId={user.Id}&token={encodedToken}";

            var body = $"Click the link to confirm your email: <a href=\"{confirmationLink}\">Confirm Email</a>";
            await _emailSender.SendEmailAsync(email, "Confirm your email", body);

            return Ok("Confirmation email sent.");
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found.");

            var decodedToken = HttpUtility.UrlDecode(token);
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            return result.Succeeded ? Ok("Email confirmed successfully.") : BadRequest("Invalid token.");
        }
    }
}
