namespace JournalyApiV2.Models;

public class Emotion
{
    public Guid UUID { get; set; }
    public string Name { get; set; }
    public string? Icon { get; set; }
    public Guid CategoryId { get; set; }
    public short Order { get; set; }
    public string? IconType { get; set; }
    public bool Deleted { get; set; }
}