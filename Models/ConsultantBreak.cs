using System.ComponentModel.DataAnnotations;

namespace BookingManagement.MVC.Models
{
    public class ConsultantBreak
    {
        public int Id { get; set; }

        public int ConsultantScheduleId { get; set; }

        [Required(ErrorMessage = "Break reason is required")]
        [StringLength(250)]
        public string Description { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public ConsultantSchedule ConsultantSchedule { get; set; } = null!;
    }
}
