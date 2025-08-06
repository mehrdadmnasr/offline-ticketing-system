using OfflineTicketingSystemAPI.Models;

namespace OfflineTicketingSystemAPI.DTOs.Ticket
{
    public class TicketDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TicketStatus Status { get; set; }
        public TicketPriority Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedByUserName { get; set; }
        public string? AssignedToAdminName { get; set; }
    }
}
