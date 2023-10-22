using JournalyApiV2.Models.Requests;

namespace JournalyApiV2.Services.DAL;

public interface IMedDbService
{
    Task SyncMeds(PatchMedsRequest.MedPatch[] patches, Guid owner, Guid deviceId);
    Task SyncMedInstances(PatchMedsRequest.MedInstancePatch[] patches, Guid owner, Guid deviceId);
    Task SyncSchedules(PatchMedsRequest.SchedulePatch[] patches, Guid owner, Guid deviceId);
}