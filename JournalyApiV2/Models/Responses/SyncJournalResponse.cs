namespace JournalyApiV2.Models.Responses;

public class SyncJournalResponse
{
    public JournalEntry[] JournalEntries { get; set; } = Array.Empty<JournalEntry>();
    public Emotion[] Emotions { get; set; } = Array.Empty<Emotion>();
    public Activity[] Activities { get; set; } = Array.Empty<Activity>();
    public EmotionCategory[] EmotionCategories { get; set; } = Array.Empty<EmotionCategory>();
    public int Remaining { get; set; }
}