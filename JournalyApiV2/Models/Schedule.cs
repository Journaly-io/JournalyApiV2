namespace JournalyApiV2.Models;

public class Schedule
{
    public Guid Uuid { get; set; }
    public Guid MedicationUuid { get; set; }
    public TimeOnly Time { get; set; }
    public bool EveryOtherDay { get; set; }
    public DayOfWeek[] Days { get; set; }
}