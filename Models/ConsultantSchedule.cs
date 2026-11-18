using System.ComponentModel.DataAnnotations;

namespace BookingManagement.MVC.Models
{
    public class ConsultantSchedule
    {
        public int Id { get; set; }

        public int ConsultantId { get; set; }

        [Range(0, 6, ErrorMessage = "Day must be between 0 and 6")]
        public int Day { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsWorking { get; set; } = true;

        public Consultant Consultant { get; set; } = null!;
        public ICollection<ConsultantBreak> Breaks { get; set; } = [];
    }
}
