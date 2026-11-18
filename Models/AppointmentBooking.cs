using System.ComponentModel.DataAnnotations;

namespace BookingManagement.MVC.Models
{
    public class AppointmentBooking
    {
        public int Id { get; set; }
        public int ConsultantId { get; set; }
        public int CustomerId { get; set; }
        public DateTime AppointmentDate { get; set; }

        [Required]
        [StringLength(50)]
        public string TimeSlot { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public int Status { get; set; } = 1;

        public Consultant Consultant { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
    }
}
