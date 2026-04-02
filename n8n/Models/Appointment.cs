namespace n8n.Models
{
    public class Appointment
    {
        public int ID { get; set; }
        public string TelegramID { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Services { get; set; }
        public DateTime Date { get; set; }
        public string CalendarEventID { get; set; }
    }
}
