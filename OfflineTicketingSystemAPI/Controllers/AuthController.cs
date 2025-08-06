using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfflineTicketingSystemAPI.Data;
using OfflineTicketingSystemAPI.DTOs.User;
using OfflineTicketingSystemAPI.Helpers;

namespace OfflineTicketingSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtHelper _jwtHelper;

        public AuthController(AppDbContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null || !PasswordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid email or password.");
            }

            var token = _jwtHelper.GenerateJwtToken(user);
            return Ok(new { Token = token, Role = user.Role.ToString() });
        }
    }
}
