using JournalyApiV2.Data.Enums;

namespace JournalyApiV2.Models;

public class Medication
{
    public Guid Uuid { get; set; }
    public string Name { get; set; }
    public string Notes { get; set; }
    public MedUnit Unit { get; set; }
    public short DefaultDose { get; set; }
    public DateOnly From { get; set; }
    public DateOnly Until { get; set; }
    public bool Forever { get; set; }
    public bool Deleted { get; set; }
    public Schedule[] Schedules { get; set; }
}