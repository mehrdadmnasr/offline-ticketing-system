using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfflineTicketingSystemAPI.Data;
using OfflineTicketingSystemAPI.DTOs.Ticket;
using OfflineTicketingSystemAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace OfflineTicketingSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // All endpoints in this controller require authentication
    public class TicketsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TicketsController(AppDbContext context)
        {
            _context = context;
        }

        // GET /tickets/my - List tickets created by the current user (Employee)
        [HttpGet("my")]
        [Authorize(Roles = "Employee")]
        public async Task<ActionResult<IEnumerable<TicketDto>>> GetMyTickets()
        {
            var userId = GetCurrentUserId();
            var tickets = await _context.Tickets
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .Where(t => t.CreatedByUserId == userId)
                .Select(t => new TicketDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    Priority = t.Priority,
                    CreatedAt = t.CreatedAt,
                    CreatedByUserName = t.CreatedByUser.FullName,
                    AssignedToAdminName = t.AssignedToUser.FullName
                })
                .ToListAsync();

            return Ok(tickets);
        }

        // POST /tickets - Create a new ticket (Employee only)
        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<ActionResult<TicketDto>> CreateTicket([FromBody] CreateTicketDto createDto)
        {
            var userId = GetCurrentUserId();
            var newTicket = new Ticket
            {
                Title = createDto.Title,
                Description = createDto.Description,
                CreatedByUserId = userId
            };

            _context.Tickets.Add(newTicket);
            await _context.SaveChangesAsync();

            // Return the created ticket with full details
            var ticketDto = new TicketDto
            {
                Id = newTicket.Id,
                Title = newTicket.Title,
                Description = newTicket.Description,
                Status = newTicket.Status,
                Priority = newTicket.Priority,
                CreatedAt = newTicket.CreatedAt,
                CreatedByUserName = (await _context.Users.FindAsync(userId)).FullName
            };

            return CreatedAtAction(nameof(GetTicketById), new { id = newTicket.Id }, ticketDto);
        }

        // GET /tickets - List all tickets (Admin only)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<TicketDto>>> GetAllTickets()
        {
            var tickets = await _context.Tickets
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .Select(t => new TicketDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    Priority = t.Priority,
                    CreatedAt = t.CreatedAt,
                    CreatedByUserName = t.CreatedByUser.FullName,
                    AssignedToAdminName = t.AssignedToUser != null ? t.AssignedToUser.FullName : null
                })
                .ToListAsync();

            return Ok(tickets);
        }

        // PUT /tickets/{id} - Update ticket status and assignment (Admin only)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTicket(Guid id, [FromBody] UpdateTicketDto updateDto)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            // Apply updates
            if (updateDto.Status.HasValue)
            {
                ticket.Status = updateDto.Status.Value;
            }
            if (updateDto.AssignedToUserId.HasValue)
            {
                // Check if the assigned user is an admin
                var assignedUser = await _context.Users.FindAsync(updateDto.AssignedToUserId.Value);
                if (assignedUser == null || assignedUser.Role != Role.Admin)
                {
                    return BadRequest("Assigned user must be a valid admin.");
                }
                ticket.AssignedToUserId = updateDto.AssignedToUserId.Value;
            }

            ticket.UpdatedAt = DateTime.UtcNow;
            _context.Entry(ticket).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET /tickets/stats - Show ticket counts by status (Admin only)
        [HttpGet("stats")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> GetTicketStats()
        {
            var stats = await _context.Tickets
                .GroupBy(t => t.Status)
                .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                .ToListAsync();

            return Ok(stats);
        }

        // Optional (Bonus): GET /tickets/{id} - Get a specific ticket's details
        [HttpGet("{id}")]
        public async Task<ActionResult<TicketDto>> GetTicketById(Guid id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();
            var userRole = GetCurrentUserRole();

            // Check if the user is the creator or the assigned admin
            if (ticket.CreatedByUserId != userId && ticket.AssignedToUserId != userId && userRole != Role.Admin)
            {
                return Forbid();
            }

            var ticketDto = new TicketDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Status = ticket.Status,
                Priority = ticket.Priority,
                CreatedAt = ticket.CreatedAt,
                CreatedByUserName = ticket.CreatedByUser.FullName,
                AssignedToAdminName = ticket.AssignedToUser?.FullName
            };

            return Ok(ticketDto);
        }

        // Helper method to get the current user's ID from JWT token
        private Guid GetCurrentUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userId);
        }

        // Helper method to get the current user's Role from JWT token
        private Role GetCurrentUserRole()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            return (Role)Enum.Parse(typeof(Role), role);
        }
    }

    // DTOs for Ticket Controller
    public class CreateTicketDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
    }

    public class UpdateTicketDto
    {
        public TicketStatus? Status { get; set; }
        public Guid? AssignedToUserId { get; set; }
    }
}
