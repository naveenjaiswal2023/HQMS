using AuthService.Application.Commands;
using AuthService.Application.DTOs.Auth;
using AuthService.Application.Queries;
using AuthService.Domain.Identity;
using AuthService.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SharedInfrastructure.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IMediator mediator,UserManager<ApplicationUser> userManager, IEmailSender emailSender, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _userManager = userManager;
            _emailSender = emailSender;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new user based on the provided registration details.
        /// </summary>
        /// <remarks>This method processes the user registration asynchronously. The provided  <paramref
        /// name="command"/> must include all required user details for successful registration.</remarks>
        /// <param name="command">The user registration command containing the necessary details to create a new user. This parameter must not
        /// be <see langword="null" />.</param>
        /// <returns>An <see cref="IActionResult"/> containing the result of the registration operation. On success, returns an
        /// HTTP 200 response with the newly created user's ID.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterCommand command)
        {
            var userId = await _mediator.Send(command);
            return Ok(new { UserId = userId });
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var token = await _mediator.Send(command);
            return Ok(token);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _mediator.Send(new LogoutCommand());
            return Ok(new { Message = "Logged out successfully." });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        [HttpGet("check-user")]
        public async Task<IActionResult> CheckUserExists([FromQuery] string email, [FromQuery] string phoneNumber)
        {
            var result = await _mediator.Send(new CheckUserExistsQuery(email, phoneNumber));
            return Ok(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found.");

            var decodedToken = HttpUtility.UrlDecode(token);
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            return result.Succeeded ? Ok("Email confirmed successfully.") : BadRequest("Invalid token.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        
        [HttpPost("internal-token")]
        public IActionResult GetInternalToken([FromBody] ClientCredentialsRequest request)
        {
            var clients = _configuration.GetSection("InternalClients").Get<List<ClientCredentialDto>>() ?? new();

            var matchedClient = clients.FirstOrDefault(x =>
                x.ClientId == request.ClientId && x.ClientSecret == request.ClientSecret);

            if (matchedClient == null)
                return Unauthorized("Invalid client credentials.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpiryMinutes"] ?? "60"));

            var claims = new[]
            {
                new Claim("client_id", matchedClient.ClientId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: creds);

            return Ok(new InternalTokenResponse
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            });
        }

    }
}
