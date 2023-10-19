using JournalyApiV2.Data.Enums;

namespace JournalyApiV2.Models;

public class RecordSync
{
    public Guid DeviceId { get; set; }
    public Guid RecordId { get; set; }
    public RecordType RecordType { get; set; }
}