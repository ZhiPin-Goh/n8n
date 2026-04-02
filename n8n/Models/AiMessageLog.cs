namespace n8n.Models
{
    public class AiMessageLog
    {
        public int ID { get; set; }
        public string TelegramID { get; set; }
        public DateTime ResponseTime { get; set; }
        public string ResponseMessage { get; set; }
        public string Rules { get; set; }
    }
}
