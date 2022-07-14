namespace MusicStream.Models
{
    public class Action
    {
        public string Name { get; set; }
        public bool Success { get; set; }

        public string Message { get; set; }
        public Action(string name, bool success)
        {
            Name = name;
            Success = success;
        }

        public Action(string name, bool success, string message) : this(name, success)
        {
            Message = message;
        }
    }
}
