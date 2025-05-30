namespace ChatApp.Models
{
    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
        public string Model { get; set; } = "llama2";
        public bool Stream { get; set; } = false;
    }

    public class ChatResponse
    {
        public string Model { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public long ProcessingTime { get; set; }
    }
}
