namespace Jobber_Server.Models.Contractors.OperatingHours
{
    /*
        Empty string for StartTime indicates that the business was already open before midnight.
        Similarly, empty string for EndTime indicates that the business remains open after 23:59.
    */
    public class StartCloseTime 
    {
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
    }
    public class OperatingHoursWeek
    {
        public ICollection<StartCloseTime> Sunday { get; set; } = new List<StartCloseTime>();
        public ICollection<StartCloseTime> Monday { get; set; } = new List<StartCloseTime>();
        public ICollection<StartCloseTime> Tuesday { get; set; } = new List<StartCloseTime>();
        public ICollection<StartCloseTime> Wednesday { get; set; } = new List<StartCloseTime>();
        public ICollection<StartCloseTime> Thursday { get; set; } = new List<StartCloseTime>();
        public ICollection<StartCloseTime> Friday { get; set; } = new List<StartCloseTime>();
        public ICollection<StartCloseTime> Saturday { get; set; } = new List<StartCloseTime>();
    }
}