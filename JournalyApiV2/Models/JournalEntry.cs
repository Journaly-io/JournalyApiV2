namespace JournalyApiV2.Models;

public class JournalEntry
{
    public Guid UUID { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<Emotion> Emotions { get; set; } = new();
    public List<Activity> Activities { get; set; } = new();
    public List<CategoryValue> CategoryValues { get; set; } = new();
}