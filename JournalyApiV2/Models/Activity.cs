namespace JournalyApiV2.Models;

public class Activity
{
    public Guid UUID { get; set; }
    public string Name { get; set; }
    public string? Icon { get; set; }
    public string? IconType { get; set; }
    public short Order { get; set; }
}