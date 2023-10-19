namespace JournalyApiV2.Models;

public class CategoryValue
{
    public Guid JournalEntryUuid { get; set; }
    public Guid CategoryUuid { get; set; }
    public double Value { get; set; }
}