namespace MusicStream.Models
{
    public class Action
    {
        public string Name { get; set; }
        public bool Success { get; set; }

        public Action(string name, bool success)
        {
            Name = name;
            Success = success;
        }
    }
}
