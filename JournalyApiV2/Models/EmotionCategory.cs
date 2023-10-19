namespace JournalyApiV2.Models;

public class EmotionCategory
{
    public Guid UUID { get; set; }
    public string Name { get; set; }
    public bool AllowMultiple { get; set; }
    public bool Default { get; set; }
    public short Order { get; set; }
    public bool Deleted { get; set; } = false;
}