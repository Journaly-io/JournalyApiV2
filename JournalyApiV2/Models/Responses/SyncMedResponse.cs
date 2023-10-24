namespace JournalyApiV2.Models.Responses;

public class SyncMedResponse
{
    public Medication[] Medications { get; set; }
    public Schedule[] Schedules { get; set; }
    public MedInstance[] MedInstances { get; set; }
}